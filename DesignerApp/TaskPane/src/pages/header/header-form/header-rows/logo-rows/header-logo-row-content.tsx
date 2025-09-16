import React, { useContext, useEffect, useState } from 'react';
import { HeaderLogoAlignment } from '@omniflow/omni-webapi';
import { TLogoRow } from '../shared/header-rows.type';
import { ImageUpload } from '../../../../../omni/image-upload';
import { ClickableIcon } from '../../../../../components/utils/clickable-icon';
import { ImageTypeMap } from '../../../../../business/image-type-map';
import { Select } from '../../../../../components/form/select';
import '../shared/header-rows.css';
import '../../../../../pages/common-styles.css';
import { TextField } from '@mui/material';
import { OmniDropDownInput } from '../../../../../omni/dropdown';

interface IHeaderLogoRowContentProps {
    row: TLogoRow;
    index: number;
    availableAlignments: HeaderLogoAlignment[];
    updateRows: (index: number, updatedRow: TLogoRow) => void;
    removeRow: (index: number) => void;
}

export const HeaderLogoRowContent: React.FC<IHeaderLogoRowContentProps> = ({
    row,
    index,
    availableAlignments,
    updateRows,
    removeRow,
}) => {
    const [imgPreview, setImgPreview] = useState('');
    const [widthPercentage, setWidthPercentage] = useState<number>(row.Width || 4); // default state value
    const [heightPercentage, setHeightPercentage] = useState<number>(row.Height || 4); // default state value

    useEffect(() => {
        if (row.Image?.Content && row.Image?.Type) {
            setImgPreview(
                `data:image/${row.Image.Type };base64,${row.Image.Content}`
            );
        } 
        if (!row.Width || !row.Height) { 
            updateRows(index, {
                ...row,
                Width: 4,
                ScaleWidth: 0.04, // 4/100 to get in percentage
                Height: 4,
                ScaleHeight: 0.04,
            });
            }
    }, []);

const changeImage = (e) => {
    const file = e.detail.file as File;
    if (!file) {
        return;
    }

    const reader = new FileReader();
    reader.addEventListener('load', () => {
        const uploadedImage = reader.result;
        if (typeof uploadedImage !== 'string') return;
        setImgPreview(uploadedImage);

        var imagestr = uploadedImage.split(',')[1];

        updateRows(index, {
            ...row,
            Image: {
                Content: imagestr,
                Type: ImageTypeMap(file.type),
            },
        });
    });
    reader.readAsDataURL(file);
};

const handleWidthChange = (e) => {
    let width = parseInt(e.target.value);
    if (isNaN(width) || width < 0) {
        width = 4;
    }
    const scaleWidth = width/100;
    setWidthPercentage(width);
    updateRows(index, {
        ...row,
        Width: width,
        ScaleWidth: scaleWidth,
    });
};

const handleHeightChange = (e) => {
    let height = parseInt(e.target.value);
    if (isNaN(height) || height < 0) {
        height = 4;
    }
    const scaleHeight = height/100;
    setHeightPercentage(height);
    updateRows(index, {
        ...row,
        Height: height,
        ScaleHeight: scaleHeight,
    });
};

return (
    <>
        <tr key={row.id}>
            <td>
                <div className="restrictedWidth">
                    <ImageUpload onImageChange={changeImage}>
                        <p slot="placeholder"></p>
                    </ImageUpload>
                </div>
                {imgPreview && (
                    <>
                        <br />
                        <img
                            className="restrictedHeight mt-4"
                            alt="logo preview"
                            id="pic"
                            src={imgPreview}
                            />
                    </>
                )}
            </td>
            <td>
                <OmniDropDownInput
                    label="Logo Alignment"
                    value={
                        // OmniDropDownInput expects an array of selected option objects
                        row.Alignment
                            ? [{
                                id: row.Alignment,
                                value: row.Alignment
                            }]
                            : []
                    }
                    required
                    onValueChange={(e: CustomEvent) => {
                        // e.detail is the selected option object or null
                        const selected = e.detail ? e.detail.value : "";
                        updateRows(index, {
                            ...row,
                            Alignment: selected as HeaderLogoAlignment,
                        });
                    }}
                    options={[
                        {
                            id: "",
                            value: "Select Alignment",
                            disabled: true,
                        },
                        // Ensure unique and non-empty alignments
                        ...Array.from(new Set([...availableAlignments, row.Alignment].filter(Boolean))).map(alignment => ({
                            id: alignment,
                            value: alignment,
                        }))
                    ]}
                    placeholder="Select Alignment"
                    hidefooter
                    className="w-100"
                />

                <table className="w-100 bg-white">
                    <tbody>
                        <tr className="no-shadow">
                            <td className="pl-0">
                                <p>Width%</p>
                                <TextField
                                    value={widthPercentage}
                                    type="number"
                                    onChange={handleWidthChange}
                                />
                            </td>
                            <td className="pr-0">
                                <p>Height%</p>
                                <TextField
                                    value={heightPercentage}
                                    type="number"
                                    onChange={handleHeightChange}
                                />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
            <td className="smallPadding">
                <ClickableIcon
                    className="custom-width-height"
                    iconId="omni:interactive:trash"
                    onClick={() => removeRow(index)}
                />
            </td>
        </tr>
    </>
);
};