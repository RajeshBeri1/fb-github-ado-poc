import {
    RightHandTotalsColumnWithId,
    useRightHandTotalsTemplateState,
} from '../states/RightHandTotalsState';
 
const useRightHandTotalsRows = (): RightHandTotalsColumnWithId[] => {
    const { Definition: state } = useRightHandTotalsTemplateState();

    if (!Array.isArray(state.Columns)) return [];

    // Ensure that tracked values trigger an update (patch for arrays)
    let tracked = [];
    state.Columns.forEach((column) => {
        tracked.push({ ...column });
    });
    tracked = [];

    return state.Columns || [];
};

export default useRightHandTotalsRows;
