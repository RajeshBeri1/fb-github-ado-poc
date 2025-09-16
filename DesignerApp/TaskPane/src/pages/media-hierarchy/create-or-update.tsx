import React, {
    ChangeEvent,
    KeyboardEvent,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
    useMemo,
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
import MediaHierarchyLevelList from './components/MediaHierarchyLevelList';
import {
    MediaHierarchyDefinitionWithId,
    useMediaHierarchyTemplateState,
} from './states/MediaHierarchyState';
import { AppContext } from '../../taskpane/contexts/AppContext';
import useSetMediaHierarchyTemplate from './hooks/useSetMediaHierarchyTemplate';
import useAddMediaHierarchyLevel from './hooks/useAddMediaHierarchyLevel';
import useSetMediaHierarchyName from './hooks/useSetMediaHierarchyName';
import Dialog, { ButtonType } from '../../components/dialogs/Dialog';
import useGoto from '../../hooks/useGoto';
import { commonApi, dataDictionaryApi, mediaHierarchyTemplateApi } from '../../lib/api';
import useNotification, {
    NotificationType,
} from '../../components/notification/useNotification';
import OnLeaveDialog from '../../components/dialogs/OnLeaveDialog';
import { UrlAndQueryParamKey } from '../../enums/url-and-query-param-key.enum';
import Spinner from '../../components/spinner/Spinner';
import SaveTemplateButton from '../../components/buttons/SaveTemplateButton';
import { plainToClass } from 'class-transformer';
import useEventLogger from '../../hooks/useEventLogger';
import { Action, Module, SubModule } from '../../enums/event.enum'
import { OmniIcon as OmniIcon } from '../../components/form/icon';
import { ErrorMessage } from '../../components/form/error-message';
import { useDebounce } from '../../hooks/useDebounce';
import '../../pages/common-styles.css';
import { OmniCheckBoxInput } from '../../omni/checkbox';
import { MediaHierarchyTemplateDetailsDTO } from '@omniflow/omni-webapi';
import { flushSync } from 'react-dom';

const MediaHierarchyCreateOrUpdate = () => {
    const { logEvent } = useEventLogger(Module.MEDIAHIERARCHY);
    const isSubscribed = useRef(false);
    const loadExecution = useRef(false);
    const pushNotification = useNotification();
    const { [UrlAndQueryParamKey.MEDIA_HIERARCHY_TEMPLATE_ID]: Id } =
        useParams();
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [levelsLoading, setLevelsLoading] = useState<boolean>(true);
    const setMediaHierarchyTemplate = useSetMediaHierarchyTemplate();
    const { Name, Definition, Currency, DisplaySource, ShowSubTotalsAtBottom } = useMediaHierarchyTemplateState();
    const [initialDefinition, setInitialDefinition] =
        useState<MediaHierarchyDefinitionWithId>(Definition);
    const [initialName, setInitialName] = useState<string>(Name || '');
    const { flowchartTemplateDefinition, setCurrentMediaHierarchyDetails, currentCalendarTemplateDetails, setTemplateChanged, setUnsavedChanges, invalidCharacters, currentMediaHierarchyDetails, levelLoading } =
        useContext(AppContext);
    const { OmniClientId } = flowchartTemplateDefinition;
    const [isSaving, setIsSaving] = useState<boolean>(false);
    const [showNameDialog, setShowNameDialog] = useState<boolean>(true);
    const [tmpName, setTmpName] = useState<string>(Name || '');
    const [tmpCurrency, setTmpCurrency] = useState<string>(Currency);
    const [tmpShowSubTotalAtBottom, setTmpShowSubTotalAtBottom] = useState<boolean | null>(ShowSubTotalsAtBottom);
    const [tmpDisplaySource, setTmpDisplaySource] = useState<string>(DisplaySource);


    //const addLevel = useAddMediaHierarchyLevel();
    const setName = useSetMediaHierarchyName();
    const goto = useGoto();
    const [invalidName, setInvalidName] = useState(false);
    const [invalidLength, setInvalidLength] = useState(false);
    const maxLength: number = 100; const searchQuery = useDebounce(tmpName, 1000);
    const controllerRef = useRef(new AbortController());
    const [isDuplicateName, setDuplicateName] = useState(false);
    const [processing, setProcessing] = useState(false);
    const [initialCurrency, setInitialCurrency] = useState(Currency);
    const [initialSource, setInitialSource] = useState(DisplaySource);
    const [initialSubTotalAtBottom, setInitialSubTotalAtBottom] = useState(ShowSubTotalsAtBottom);
    let stateTracked = JSON.stringify(Definition);
    stateTracked = '';

    let initialLoad = false;
    const configPrev = currentMediaHierarchyDetails;
    const previousState = useRef<MediaHierarchyTemplateDetailsDTO | null>(configPrev);

    const [leaveDialog, setLeaveDialog] = useState(false);
    const loadMediaHierarchyTemplate = useCallback(async () => {
        if (isSubscribed.current) setIsLoading(true);

        try {
            const { data } =
                await mediaHierarchyTemplateApi.mediaHierarchyTemplateGet(Id);

            if (data && isSubscribed.current) {
                const mediaHierarchyDefinition =
                    setMediaHierarchyTemplate(data);
                setInitialDefinition(mediaHierarchyDefinition);
                setInitialName(data?.Name || '');
                setInitialCurrency(data?.Currency);
                setInitialSource(data?.DisplaySource);
                setInitialSubTotalAtBottom(data?.ShowSubTotalsAtBottom);
                setIsLoading(false);
            }
        } catch (e) {
            pushNotification(
                `Media Hierarchy Component "${Id}" could not be loaded!`,
                NotificationType.DANGER
            );
            if (isSubscribed.current) setIsLoading(false);
            console.error(e);
        }
    }, [Id]);

    useEffect(() => {
        isSubscribed.current = true;
        if (Id && isSubscribed.current) setShowNameDialog(false);
        if (Id) loadMediaHierarchyTemplate().then();

        return () => {
            isSubscribed.current = false;
        };
    }, [Id, loadMediaHierarchyTemplate]);

    const handleOk = async () => {
        try {
            // checking for duplicate name if the name has actually changed
            if (tmpName !== Name) {
                const { data: isDuplicate } = await commonApi.commonCheckDuplicateComponentName(
                    plainToClass(API.DuplicateNameCheckDto, {
                        OmniClientId: OmniClientId,
                        Name: tmpName,
                        ComponentName: API.FlowChartComponent.MediaHierarchy
                    })
                );

                if (isDuplicate) {
                    setDuplicateName(true);
                    return;
                }
            }

            logEvent({ action: Action.OK });
            flushSync(() => {
                const updatedDetails = {
                    ...currentMediaHierarchyDetails,
                    Name: tmpName.trim(),
                    Currency: tmpCurrency,
                    ShowSubTotalsAtBottom: tmpShowSubTotalAtBottom,
                    DisplaySource: tmpDisplaySource
                };
                setCurrentMediaHierarchyDetails(updatedDetails);
            });
            // Update context
            setName(tmpName, tmpCurrency, tmpShowSubTotalAtBottom, tmpDisplaySource);
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
        logEvent({ action: Action.CANCEL });
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

    useEffect(() => {
        const checkName = async () => {
            // Handle both new components and edits
            if ((Id && searchQuery !== Name) || (!Name && searchQuery)) {
                try {
                    const { data } = await commonApi.commonCheckDuplicateComponentName(
                        plainToClass(API.DuplicateNameCheckDto, {
                            OmniClientId: OmniClientId,
                            Name: searchQuery,
                            ComponentName: API.FlowChartComponent.MediaHierarchy
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
        setTmpName(Name);
        setTmpCurrency(Currency);
        setTmpDisplaySource(DisplaySource);
        setTmpShowSubTotalAtBottom(ShowSubTotalsAtBottom);
        logEvent({ action: Action.EDITMEDIAHIERARCHYNAME })
        setShowNameDialog(true);
    };
    const currencies = [
        {
            name: 'Local Currency',
            value: 'LLL',
        },
        {
            name: 'US Dollars',
            value: 'USD',
        },

    ]

    const sources = [
        {
            name: 'Planning System Field Name',
            value: API.DisplaySource.PlanitField,
        },
        {
            name: 'Planning System Client Alias Name',
            value: API.DisplaySource.PlanitClientAlias,
        },
    ]

    const changed = useMemo(() => {
        return !!(
            !Id ||
            (Id &&
                !isLoading &&
                !levelsLoading &&
                (!isEqual(Definition, initialDefinition) ||
                    Name !== initialName ||
                    initialCurrency !== currentMediaHierarchyDetails?.Currency ||
                    initialSource !== currentMediaHierarchyDetails?.DisplaySource ||
                    initialSubTotalAtBottom !== currentMediaHierarchyDetails?.ShowSubTotalsAtBottom)
            )
        );
    },
        [
            Id,
            isLoading,
            levelsLoading,
            Definition,
            initialDefinition,
            Name,
            initialName,
            initialCurrency,
            currentMediaHierarchyDetails?.Currency,
            initialSource,
            currentMediaHierarchyDetails?.DisplaySource,
            initialSubTotalAtBottom,
            currentMediaHierarchyDetails?.ShowSubTotalsAtBottom
        ]);

    const createOrUpdateMediaHierarchyTemplate = useCallback(async () => {
        if (OmniClientId) {
            setIsSaving(true);

            const mediaHierarchyTemplate = {
                Id,
                OmniClientId: OmniClientId,
                Definition,
                Name,
                Currency: currentMediaHierarchyDetails?.Currency || "LLL",
                ShowSubTotalsAtBottom: currentMediaHierarchyDetails?.ShowSubTotalsAtBottom,
                DisplaySource: currentMediaHierarchyDetails?.DisplaySource || API.DisplaySource.PlanitClientAlias,
            } as API.MediaHierarchyTemplateCreateDTO &
                API.MediaHierarchyTemplateUpdateDTO;

            try {
                const save = (template) =>
                    Id
                        ? mediaHierarchyTemplateApi.mediaHierarchyTemplateUpdate(
                              template
                          )
                        : mediaHierarchyTemplateApi.mediaHierarchyTemplateCreate(
                              template
                          );
                const { data } = await save(mediaHierarchyTemplate);
                setCurrentMediaHierarchyDetails(data);
                logEvent({ action: Id ? Action.UPDATE : Action.CREATE });
                pushNotification(
                    `Media Hierarchy Component "${Name}" was successfully ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.SUCCESS
                );
                setTemplateChanged(true);
                goto(routes.list);
            } catch (e) {
                pushNotification(
                    `Media Hierarchy Component "${Name}" could not be ${
                        Id ? 'updated' : 'created'
                    }!`,
                    NotificationType.DANGER
                );
                setIsSaving(false);
                console.error(e);
            }
        }
        if (changed) {
            setUnsavedChanges((prevState) => ({ ...prevState, mediaHierarchy: changed }));
        }
    }, [Id, OmniClientId, Definition, Name, currentMediaHierarchyDetails?.Currency, currentMediaHierarchyDetails?.DisplaySource, currentMediaHierarchyDetails?.ShowSubTotalsAtBottom, changed]);

    const handleLevelsLoaded = useCallback((NewDefinition) => {
        if (!loadExecution.current) {
            setInitialDefinition(NewDefinition);
            loadExecution.current = true;
        }
        setLevelsLoading(false);
    }, []);

    const handleShowSubtotalAtBottom = (event) => {
        setTmpShowSubTotalAtBottom(event?.currentTarget?.checked);
    }

    const handleDisplaySourceChange = (event) => {
        setTmpDisplaySource(event?.currentTarget?.value);
    }
    const handleComponentCancel = async () => {
        if (previousState.current) {
            setCurrentMediaHierarchyDetails(
                plainToClass(MediaHierarchyTemplateDetailsDTO, previousState.current)
            );
        }
    };

    useEffect(() => {
        if (showNameDialog && ShowSubTotalsAtBottom !== tmpShowSubTotalAtBottom) {
            setTmpShowSubTotalAtBottom(ShowSubTotalsAtBottom);
        }
    }, [showNameDialog, ShowSubTotalsAtBottom]);
    useEffect(() => {
        if (Definition?.Levels?.length && Name && !Id) {
            setCurrentMediaHierarchyDetails(plainToClass(API.MediaHierarchyTemplateDetailsDTO, { Definition: Definition, Id: flowchartTemplateDefinition.Id, Currency: currentMediaHierarchyDetails?.Currency, ShowSubTotalsAtBottom: currentMediaHierarchyDetails?.ShowSubTotalsAtBottom, DisplaySource: currentMediaHierarchyDetails?.DisplaySource }));
        }
    }, [Definition, currentMediaHierarchyDetails?.Currency, currentMediaHierarchyDetails?.DisplaySource, currentMediaHierarchyDetails?.ShowSubTotalsAtBottom, flowchartTemplateDefinition.Id, Id, Name]);
    return (
        <>
            <Tile>
                <Toolbar slot="header">
                    <p className="component-title">Level Selection</p>
                    <RenderButton
                        definitionName="mediaHierarchyDefinition"
                        definition={Definition as API.MediaHierarchyDefinition}
                        disabled={!Name || !Definition.Levels.length}
                    />
                    {Name && (
                        <div className="template-name-container">
                            <p className="component-title template-name" title={Name}>{Name}</p>
                            <Button onClick={handleEditName} tooltip="Edit" className="icon ml-2">
                                <Icon icon-id="omni:interactive:edit"></Icon>
                            </Button>
                        </div>
                    )}
                   
                    <div slot="end">
                        <Button to={routes.list} className="button icon" tooltip="Back" >
                            <Icon className="is-size-2" icon-id="omni:interactive:back" onClick={() => logEvent({ action: Action.CANCEL })}></Icon>
                        </Button>
                    </div>
                </Toolbar>
                {isLoading ? (
                    <Spinner />
                ) : (
                    <>
                        <MediaHierarchyLevelList
                                onLevelsLoaded={handleLevelsLoaded}
                        />
                       
                    </>
                )}
            </Tile>
            <div className="d-flex is-justify-content-end">
                <Button
                    className="button tertiary large"
                    onClick={(e) => { e.preventDefault(); setLeaveDialog(true); logEvent({ action: Action.CANCEL })}}>
                    Cancel
                </Button>
                <SaveTemplateButton
                    onClick={createOrUpdateMediaHierarchyTemplate}
                    loading={isSaving} className="large">
                    Save template
                </SaveTemplateButton>
            </div>
            {showNameDialog && (
                <Dialog
                    title={`${Name ? 'Update' : 'New'} media hierarchy component`}
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
                                Please configure settings for your new media hierarchy component
                            </span>
                            <p className="input-label-top">Component name</p>
                            <input
                                type="text"
                                className={"input text " + (invalidName || invalidLength || isDuplicateName ? 'error' : '')}
                                autoFocus
                                placeholder="Enter component name"
                                value={tmpName}
                                onChange={handleNameChange}
                                onKeyUp={handleEnter}
                            />
                            {invalidName && <ErrorMessage message='A component name can only contain one of the following characters: A-Z a-z 0-9'></ErrorMessage>}
                            {invalidLength && <ErrorMessage message='The component name must be 100 characters or less'></ErrorMessage>}
                            {isDuplicateName && <ErrorMessage message='The component name has already been used / unavailable'></ErrorMessage>}
                            <p className="input-label-top">Currency Switch</p>
                            <select
                                className="select input w-100"
                                onChange={(e) => {
                                    setTmpCurrency(e.target.value);

                                }}
                                value={tmpCurrency}
                            >
                                {currencies.map((curr) => (
                                    <option value={curr.value} key={curr.value}>
                                        {curr.name}
                                    </option>
                                ))}

                            </select>

                            <p className="input-label-top">Display Source</p>
                            <select
                                className="select input w-100"
                                onChange={handleDisplaySourceChange}
                                value={tmpDisplaySource}
                            >
                                {sources.map((source) => (
                                    <option value={source.value} key={source.value}>
                                        {source.name}
                                    </option>
                                ))}

                            </select>

                            <div className="field mt-5">
                                <div className="control"></div>
                                <OmniCheckBoxInput checked={tmpShowSubTotalAtBottom} onClick={(e) => handleShowSubtotalAtBottom(e)} className="breifctc-checkbox"> <label className="mb-0 text-core-dark">Show subtotals at bottom.</label></OmniCheckBoxInput>
                            </div>
                        </label>
                    </div>

                </Dialog>
            )}
            <OnLeaveDialog when={!!Name && leaveDialog} setDialog={(e) => { setLeaveDialog(e); }} navigatePath={routes.list} handleOk={handleComponentCancel} definitionName={'mediaHierarchyDefinition'} definition={(previousState?.current?.Definition) ?? (flowchartTemplateDefinition?.Definition?.MediaHierarchyDefinition?.Definition)} />
        </>
    );
};

export default MediaHierarchyCreateOrUpdate;
