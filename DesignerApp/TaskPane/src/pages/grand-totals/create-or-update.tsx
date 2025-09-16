import React, {
    ChangeEvent,
    KeyboardEvent,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from 'react';
import { useParams } from 'react-router';
import * as API from '@omniflow/omni-webapi';
import { isEqual } from 'lodash';

import { Toolbar } from '../../omni/toolbar';
import RenderButton from '../../components/buttons/RenderButton';
import Button from '../../components/buttons/Button';
import routes from './routes';
import { Tile } from '../../omni/tile';
import { Icon } from '../../omni/icon';
import GrandTotalsRowList from './components/GrandTotalsRowList';
import {
    GrandTotalsDefinitionWithId,
    useGrandTotalsTemplateState,
} from './states/GrandTotalsState';
import { AppContext } from '../../taskpane/contexts/AppContext';
import useSetGrandTotalsTemplate from './hooks/useSetGrandTotalsTemplate';
import useAddGrandTotalsRow from './hooks/useAddGrandTotalsRow';
import useSetGrandTotalsName from './hooks/useSetGrandTotalsName';
import Dialog, { ButtonType } from '../../components/dialogs/Dialog';
import useGoto from '../../hooks/useGoto';
import { commonApi, grandTotalTemplateApi } from '../../lib/api';
import useNotification, {
    NotificationType,
} from '../../components/notification/useNotification';
import OnLeaveDialog from '../../components/dialogs/OnLeaveDialog';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';
import Spinner from '../../components/spinner/Spinner';
import SaveTemplateButton from '../../components/buttons/SaveTemplateButton';
import useRender from '../../hooks/useRender';
import { plainToClass } from 'class-transformer';
import useEventLogger from '../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'
import { OmniIcon as OmniIcon } from '../../components/form/icon';
import { ErrorMessage } from '../../components/form/error-message';
import { useDebounce } from '../../hooks/useDebounce';
import { GrandTotalTemplateDetailsDTO } from '@omniflow/omni-webapi';

const GrandTotalsCreateOrUpdate = () => {
    const isSubscribed = useRef(false);
    const pushNotification = useNotification();
    const { [UrlAndQueryParamKey.GRAND_TOTALS_TEMPLATE_ID]: Id } = useParams();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const setGrandTotalsTemplate = useSetGrandTotalsTemplate();
    const { Name, Definition } = useGrandTotalsTemplateState();
    const [initialDefinition, setInitialDefinition] =
        useState<GrandTotalsDefinitionWithId>(Definition);
    const [initialName, setInitialName] = useState<string>(Name || '');
    const { flowchartTemplateDefinition, setCurrentGrandTotalsDetails, currentGrandTotalsDetails, setTemplateChanged, setUnsavedChanges, invalidCharacters } =
        useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const [isSaving, setIsSaving] = useState<boolean>(false);
    const [showNameDialog, setShowNameDialog] = useState<boolean>(true);
    const [tmpName, setTmpName] = useState<string>(Name || '');
    const addRow = useAddGrandTotalsRow();
    const setName = useSetGrandTotalsName();
    const goto = useGoto();
    const { render } = useRender();
    // Todo: This is just a hotfix, need to clean up
    let stateTracked = JSON.stringify(Definition);
    stateTracked = '';
    const { logEvent } = useEventLogger(Module.GRANDTOTALS);
    const [invalidName, setInvalidName] = useState(false);
    const [invalidLength, setInvalidLength] = useState(false);
    const [isDuplicateName, setDuplicateName] = useState(false);
    const [processing, setProcessing] = useState(false);
    const maxLength: number = 100;
    const searchQuery = useDebounce(tmpName, 1000);
    const controllerRef = useRef(new AbortController());
    const configPrev = currentGrandTotalsDetails;
    const previousState = useRef<GrandTotalTemplateDetailsDTO | null>(configPrev);
    const [leaveDialog, setLeaveDialog] = useState(false);
    const loadGrandTotalsTemplate = useCallback(async () => {
        if (isSubscribed.current) setIsLoading(true);

        try {
            const { data } = await grandTotalTemplateApi.grandTotalTemplateGet(
                Id
            );

            if (data && isSubscribed.current) {
                const grandTotalsDefinition = setGrandTotalsTemplate(data);
                setInitialDefinition(grandTotalsDefinition);
                setInitialName(data?.Name || '');
                setIsLoading(false);
            }
        } catch (e) {
            pushNotification(
                `Grand Totals Component "${Id}" could not be loaded!`,
                NotificationType.DANGER
            );
            if (isSubscribed.current) setIsLoading(false);
            console.error(e);
        }
    }, [Id]);

    useEffect(() => {
        isSubscribed.current = true;
        if (Id && isSubscribed.current) setShowNameDialog(false);
        if (Id) loadGrandTotalsTemplate().then();

        return () => {
            isSubscribed.current = false;
        };
    }, [Id, loadGrandTotalsTemplate]);


    const handleOk = async () => {
        try {
            // checking for duplicate name if the name has actually changed
            if (tmpName !== Name) {
                const { data: isDuplicate } = await commonApi.commonCheckDuplicateComponentName(
                    plainToClass(API.DuplicateNameCheckDto, {
                        OmniClientId: OmniClientId,
                        Name: tmpName,
                        ComponentName: API.FlowChartComponent.GrandTotals
                    })
                );

                if (isDuplicate) {
                    setDuplicateName(true);
                    return;
                }
            }

            logEvent({ action: Action.OK, label: "New Grand Totals Template" });
            setName(tmpName);
            setShowNameDialog(false);
        } catch (e) {
            console.error(e);
        }
    };

    const handleCancel = () => {
        setInvalidName(false);
        setInvalidLength(false);
        setProcessing(false);
        setDuplicateName(false);
        logEvent({ action: Action.CANCEL, label: "New Grand Totals Template" })
        if (!Name) {
            goto(routes.list);
        } else {
            setShowNameDialog(false);
        }
    };

    const handleNameChange = (event: ChangeEvent<HTMLInputElement>) => {
        setInvalidName(invalidCharacters.test(event.currentTarget.value));
        setInvalidLength(event.currentTarget.value && event.currentTarget.value.length > maxLength);
        setTmpName(event.target.value);
        setDuplicateName(false);
        setProcessing(true);
    };

    // Updated useEffect for name validation
    useEffect(() => {
        const checkName = async () => {
            if ((Id && searchQuery !== Name) || (!Name && searchQuery)) {
                try {
                    const { data } = await commonApi.commonCheckDuplicateComponentName(
                        plainToClass(API.DuplicateNameCheckDto, {
                            OmniClientId: OmniClientId,
                            Name: searchQuery,
                            ComponentName: API.FlowChartComponent.GrandTotals
                        })
                    );
                    setDuplicateName(data);
                    setProcessing(false);
                } catch (e) {
                    console.error(e);
                    setDuplicateName(false);
                    setProcessing(false);
                }
            } else {
                setDuplicateName(false);
                setProcessing(false);
            }
        };

        const controller = new AbortController();
        controllerRef.current = controller;

        checkName();

        return () => controllerRef.current.abort();
    }, [searchQuery, Name, Id, OmniClientId]);


    const handleEnter = (event: KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            event.preventDefault(); // Prevent default Enter behavior

            // Only proceed if name is valid
            if (tmpName &&
                !invalidName &&
                !invalidLength &&
                !processing &&
                !isDuplicateName) {
                handleOk();
            }
        }
    };

    const handleEditName = () => {
        logEvent({ action: Action.EDITTEMPLATENAME })
        setTmpName(Name);
        setShowNameDialog(true);
    };
    const changed = !!(
        !Id ||
        (Id &&
            !isLoading &&
            (!isEqual(Definition, initialDefinition) || Name !== initialName))
    );

    const createOrUpdateGrandTotalsTemplate = useCallback(async () => {
        if (OmniClientId) {
            setIsSaving(true);

            const grandTotalsTemplate = {
                Id,
                OmniClientId: OmniClientId,
                Definition,
                Name,
            } as API.GrandTotalTemplateCreateDTO &
                API.GrandTotalTemplateUpdateDTO;

            try {
                const save = (template) =>
                    Id
                        ? grandTotalTemplateApi.grandTotalTemplateUpdate(
                              template
                          )
                        : grandTotalTemplateApi.grandTotalTemplateCreate(
                              template
                          );
                const { data } = await save(grandTotalsTemplate);
                setCurrentGrandTotalsDetails(data);
                logEvent({ action: Id ? Action.UPDATE : Action.CREATE });
                pushNotification(
                    `Grand Totals Component "${Name}" was successfully ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.SUCCESS
                );
                setTemplateChanged(true);
                goto(routes.list);
            } catch (e) {
                pushNotification(
                    `Grand Totals Component "${Name}" could not be ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.DANGER
                );
                setIsSaving(false);
                console.error(e);
            }
        }

        if (changed) {
            setUnsavedChanges((prevState) => ({ ...prevState, grandTotals: changed }));
        }
    }, [Id, OmniClientId, Definition, Name]);  

    const handleComponentCancel = async () => {
        if (previousState.current) {
            setCurrentGrandTotalsDetails(
                plainToClass(GrandTotalTemplateDetailsDTO, previousState.current)
            );
        }
    };

    useEffect(() => {
        if (Definition?.Selections?.length && Name && !Id) {
            setCurrentGrandTotalsDetails(plainToClass(API.GrandTotalTemplateDetailsDTO, { Definition: Definition, Id: flowchartTemplateDefinition.Id }));
        } 
    }, [Definition])
    return (
        <>
            <Tile>
                <Toolbar slot="header">
                    Grand Totals Selection
                    <RenderButton
                        definitionName="grandTotalsDefinition"
                        definition={Definition as API.GrandTotalDefinition}
                        disabled={!Name || !Definition?.Selections?.length}
                        
                    />
                    {Name && (
                        <>
                            <div className="template-name-container">
                                <span className="template-name" title={Name}>{Name}</span>
                                <Button className="icon" onClick={handleEditName} tooltip="Edit">
                                    <Icon icon-id="omni:interactive:edit"></Icon>
                                </Button>
                            </div>
                        </>
                    )}
                    <div slot="end" className="toolbar-divider"></div>
                    <div slot="end">
                        <Button
                            className="secondary small"
                            disabled={!Name}
                            onClick={() => {
                                logEvent({ action: Action.ADDROW, subModule: SubModule.GRANDTOTAL });
                                addRow()
                            }}>
                            Add row
                        </Button>
                    </div>
                    <div slot="end">
                        <Button className="icon" to={routes.list} tooltip="Back" onClick={() => logEvent({ action: Action.CANCEL, subModule: SubModule.GRANDTOTAL })}>
                            <Icon icon-id="omni:interactive:back"></Icon>
                        </Button>
                    </div>
                </Toolbar>
                {isLoading ? (
                    <Spinner />
                ) : (
                    <>
                        <GrandTotalsRowList />
                        <div className="d-flex is-justify-content-end">
                            <Button
                                    onClick={(e) => { e.preventDefault(); setLeaveDialog(true); logEvent({ action: Action.CANCEL })}} 
                                className="button tertiary large"
                                >
                                Cancel
                            </Button>
                            <SaveTemplateButton
                                onClick={createOrUpdateGrandTotalsTemplate}
                                loading={isSaving}
                                disabled={isSaving || !changed} className="large">
                                Save template
                            </SaveTemplateButton>
                        </div>
                    </>
                )}
            </Tile>
            {showNameDialog && (
                <Dialog
                    title={`${Name ? 'Update' : 'New'} grand totals component`}
                    icon="interactive:add"
                    okDisabled={!tmpName || invalidName || invalidLength || processing || isDuplicateName}
                    onOk={handleOk}
                    okText={`${Name ? 'Update component' : 'Create component'}`}
                    onCancel={handleCancel}
                    buttonType={ButtonType.PRIMARY}
                    isTemplateDialog={true}
                    showDialog={showNameDialog}
                >
                    <div className="d-flex">
                        <label className="w-100 no-capital">
                            <span>
                                    Please enter a name for the new grand totals
                                    component.
                            </span>
                            <p className="input-label-top">* component name</p>
                            <input
                                type="text"
                                className={"input text " + (invalidName || invalidLength || isDuplicateName ? 'error' : '')}
                                autoFocus
                                placeholder="Enter component name"
                                value={tmpName}
                                onChange={handleNameChange}
                                onKeyUp={handleEnter}
                            />
                        </label>
                    </div>
                    {invalidName && <ErrorMessage message='A component name can only contain one of the following characters: A-Z a-z 0-9'></ErrorMessage>}
                    {invalidLength && <ErrorMessage message='The component name must be 100 characters or less'></ErrorMessage>}
                    {isDuplicateName && <ErrorMessage message='The component name has already been used / unavailable'></ErrorMessage>}
                </Dialog>
            )}
            <OnLeaveDialog when={!!Name && leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={routes.list}  handleOk={handleComponentCancel} definitionName={'grandTotalsDefinition'} definition={(previousState?.current?.Definition) ?? (flowchartTemplateDefinition?.Definition?.GrandTotalDefinition?.Definition)} />
        </>
    );
};

export default GrandTotalsCreateOrUpdate;
