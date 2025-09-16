import {
    GrandTotalsSelectionWithId,
    useGrandTotalsTemplateState,
} from '../states/GrandTotalsState';

const useGrandTotalsRows = (): GrandTotalsSelectionWithId[] => {
    const { Definition: state } = useGrandTotalsTemplateState();

    if (!Array.isArray(state.Selections)) return [];

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    state.Selections.forEach((selection) => {
        tracked.push({ ...selection });
    });
    tracked = [];

    return state.Selections || [];
};

export default useGrandTotalsRows;
