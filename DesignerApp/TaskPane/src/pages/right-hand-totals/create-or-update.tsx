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
import RightHandTotalsRowList from './components/RightHandTotalsRowList';
import {
    RightHandTotalsDefinitionWithId,
    useRightHandTotalsTemplateState,
} from './states/RightHandTotalsState';
import { AppContext } from '../../taskpane/contexts/AppContext';
import useSetRightHandTotalsTemplate from './hooks/useSetRightHandTotalsTemplate';
import useAddRightHandTotalsRow from './hooks/useAddRightHandTotalsRow';
import useSetRightHandTotalsName from './hooks/useSetRightHandTotalsName';
import Dialog, { ButtonType } from '../../components/dialogs/Dialog';
import useGoto from '../../hooks/useGoto';
import { commonApi, totalsTemplateApi } from '../../lib/api';
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
import { Action, Module, SubModule } from '../../enums/event.enum';
import { OmniIcon as OmniIcon } from '../../components/form/icon';
import { ErrorMessage } from '../../components/form/error-message';
import { useDebounce } from '../../hooks/useDebounce';
import { TotalsTemplateDetailsDTO } from '@omniflow/omni-webapi';

const RightHandTotalsCreateOrUpdate = () => {
    const isSubscribed = useRef(false);
    const pushNotification = useNotification();
    const { [UrlAndQueryParamKey.RIGHT_HAND_TOTALS_TEMPLATE_ID]: Id } =
        useParams();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const setRightHandTotalsTemplate = useSetRightHandTotalsTemplate();
    const { Name, Definition } = useRightHandTotalsTemplateState();
    const [initialDefinition, setInitialDefinition] =
        useState<RightHandTotalsDefinitionWithId>(Definition);
    const [initialName, setInitialName] = useState<string>(Name || '');
    const { flowchartTemplateDefinition, setCurrentTotalsDetails, currentTotalsDetails, setTemplateChanged, setUnsavedChanges, invalidCharacters } =
        useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const [isSaving, setIsSaving] = useState<boolean>(false);
    const [showNameDialog, setShowNameDialog] = useState<boolean>(true);
    const [tmpName, setTmpName] = useState<string>(Name || '');
    const addRow = useAddRightHandTotalsRow();
    const setName = useSetRightHandTotalsName();
    const goto = useGoto();
    const { render } = useRender();
    const [invalidName, setInvalidName] = useState(false);
    const [invalidLength, setInvalidLength] = useState(false);
    const maxLength: number = 100;   
    const [isDuplicateName, setDuplicateName] = useState(false);
    const [processing, setProcessing] = useState(false);
    const searchQuery = useDebounce(tmpName, 1000);
    // Todo: This is just a hotfix, need to clean up
    let stateTracked = JSON.stringify(Definition);
    stateTracked = '';
    const configPrev = currentTotalsDetails;
    const previousState = useRef<TotalsTemplateDetailsDTO | null>(configPrev);
    const { logEvent } = useEventLogger(Module.RIGHTHANDTOTALS);

    const [leaveDialog, setLeaveDialog] = useState(false);
    const loadRightHandTotalsTemplate = useCallback(async () => {
        if (isSubscribed.current) setIsLoading(true);

        try {
            const { data } = await totalsTemplateApi.totalsTemplateGet(Id);

            if (data && isSubscribed.current) {
                const rightHandTotalsDefinition =
                    setRightHandTotalsTemplate(data);
                setInitialDefinition(rightHandTotalsDefinition);
                setInitialName(data?.Name || '');
                setIsLoading(false);
            }
        } catch (e) {
            pushNotification(
                `Right Hand Totals Component "${Id}" could not be loaded!`,
                NotificationType.DANGER
            );
            if (isSubscribed.current) setIsLoading(false);
            console.error(e);
        }
    }, [Id]);

    useEffect(() => {
        isSubscribed.current = true;
        if (Id && isSubscribed.current) setShowNameDialog(false);
        if (Id) loadRightHandTotalsTemplate().then();

        return () => {
            isSubscribed.current = false;
        };
    }, [Id, loadRightHandTotalsTemplate]);


    const handleOk = async () => {
        try {
            // checking for duplicate name if the name has actually changed
            if (tmpName !== Name) {
                const { data: isDuplicate } = await commonApi.commonCheckDuplicateComponentName(
                    plainToClass(API.DuplicateNameCheckDto, {
                        OmniClientId: OmniClientId,
                        Name: tmpName,
                        ComponentName: API.FlowChartComponent.RightHandTotals
                    })
                );

                if (isDuplicate) {
                    setDuplicateName(true);
                    return;
                }
            }

            logEvent({ action: Action.OK, label: "New Right Hand Totals Component" });
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
        logEvent({ action: Action.CANCEL, label: "New Right Hand Totals Component" })
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
                            ComponentName: API.FlowChartComponent.RightHandTotals
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

        checkName();

        return () => controller.abort();
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
        logEvent({ action: Action.EDITTEMPLATENAME});
        setTmpName(Name);
        setShowNameDialog(true);
    };
    const changed = !!(
        !Id ||
        (Id &&
            !isLoading &&
            (!isEqual(Definition, initialDefinition) || Name !== initialName))
    );

    const createOrUpdateRightHandTotalsTemplate = useCallback(async () => {
        if (OmniClientId) {
            setIsSaving(true);

            const rightHandTotalsTemplate = {
                Id,
                OmniClientId: OmniClientId,
                Definition,
                Name,
            } as API.TotalsTemplateCreateDTO & API.TotalsTemplateUpdateDTO;

            try {
                const save = (template) =>
                    Id
                        ? totalsTemplateApi.totalsTemplateUpdate(template)
                        : totalsTemplateApi.totalsTemplateCreate(template);
                const { data } = await save(rightHandTotalsTemplate);
                setCurrentTotalsDetails(data);
                logEvent({ action: Id ? Action.UPDATE : Action.CREATE });
                pushNotification(
                    `Right Hand Totals Component "${Name}" was successfully ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.SUCCESS
                );
                setTemplateChanged(true);
                goto(routes.list);
            } catch (e) {
                pushNotification(
                    `Right Hand Totals Component "${Name}" could not be ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.DANGER
                );
                setIsSaving(false);
                console.error(e);
            }
        }
        if (changed) {
            setUnsavedChanges((prevState) => ({ ...prevState, rightHandTotals: changed }));
        }
    }, [Id, OmniClientId, Definition, Name]);

    const handleComponentCancel = async () => {
        if (previousState.current) {
            setCurrentTotalsDetails(
                plainToClass(TotalsTemplateDetailsDTO, previousState.current)
            );
        }
    };
    useEffect(() => {
        if (Definition?.Columns?.length && Name && !Id) {
            setCurrentTotalsDetails(plainToClass(API.TotalsTemplateDetailsDTO, { Definition: Definition, Id: flowchartTemplateDefinition.Id }));
        }
    }, [Definition])
    return (
        <>
            <Tile>
                <Toolbar slot="header">
                    Right Hand Totals Selection
                    <RenderButton
                        definitionName="totalsDefinition"
                        definition={Definition as API.TotalsDefinition}
                        disabled={!Name || !Definition?.Columns?.length}
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
                            className="secondary"
                            disabled={!Name}
                            onClick={() => {
                                logEvent({ action: Action.ADDROW, subModule: SubModule.RIGHTHANDTOTAL });
                                addRow()
                            }}>
                            Add Row
                        </Button>
                    </div>
                    <div slot="end">
                        <Button className="icon" onClick={() => logEvent({ action: Action.CANCEL, subModule: SubModule.RIGHTHANDTOTAL })} to={routes.list}>
                            <Icon icon-id="omni:interactive:back"></Icon>
                        </Button>
                    </div>
                </Toolbar>
                {isLoading ? (
                    <Spinner />
                ) : (
                    <>
                        <RightHandTotalsRowList />
                        <div className="d-flex is-justify-content-end">
                                <Button
                                    onClick={(e) => { e.preventDefault(); setLeaveDialog(true); logEvent({ action: Action.CANCEL })}} 
                                className="button tertiary large"
                                >
                                Cancel
                            </Button>
                            <SaveTemplateButton
                                onClick={createOrUpdateRightHandTotalsTemplate}
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
                    title={`${Name ? 'Update' : 'New'} right hand totals component`}
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
                                    Please enter a name for the totals
                                    component.
                            </span>
                            <p className="input-label-top">* Component name</p>
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
            <OnLeaveDialog when={!!Name && leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={routes.list} handleOk={handleComponentCancel} definitionName={'totalsDefinition'} definition={(previousState?.current?.Definition) ?? (flowchartTemplateDefinition?.Definition?.TotalsDefinition?.Definition)} />
        </>
    );
};

export default RightHandTotalsCreateOrUpdate;
