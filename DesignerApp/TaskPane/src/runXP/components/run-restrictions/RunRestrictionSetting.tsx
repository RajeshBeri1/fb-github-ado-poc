import React, { JSX, memo, ChangeEvent, useContext, MouseEvent } from 'react';

import Button from '../../../components/buttons/Button';
import { Icon } from '../../../omni/icon';
import { AppContext } from '../../../taskpane/contexts/AppContext';
import useRender from '../../../hooks/useRender';
import { Settings } from '../../../models/settings';
import { OmniCheckBoxInput } from '../../../omni/checkbox';

export type RunRestrictionSettingProps = {
    levelId: number;
    setting: Settings;
    onChange: (checked: boolean, subLevelId: number) => void;
    subLevelId: number;
};

const RunRestrictionSetting = ({
    levelId,
    setting,
    onChange,
    subLevelId
}: RunRestrictionSettingProps): JSX.Element => {
    return (
        <>
            <tr className="d-flex flex-row w-100">
                <td className="d-flex is-align-items-center p-3" style={{ padding: '10px' }}>
                    <OmniCheckBoxInput
                        checked={setting.Enabled}
                        onClick={(e) => { onChange(Boolean(e?.currentTarget?.checked), subLevelId) }}
                    />
                </td>
                <td className="d-flex is-align-items-center p-1 is-size-6 w-100" style={{ padding: '10px' }}>
                    {setting.Name}
                </td>
                <td>
                </td>
            </tr>
        </>
    );
};

export default memo(RunRestrictionSetting);
