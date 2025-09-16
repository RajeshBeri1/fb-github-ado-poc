import { CustomPropertyKey } from '../enums/custom-property-key.enum';
import { CustomProperty } from '../models/custom-property';

/**
 * https://raw.githubusercontent.com/OfficeDev/office-js-snippets/prod/samples/excel/26-document/custom-properties.yaml
 * Load the Flowchart template id via custom properties. See link above!
 */
export class CustomPropertyService {
    private static instance: CustomPropertyService;

    private customProperties: Map<CustomPropertyKey, CustomProperty> = new Map();

    private constructor() {}

    public static getInstance(): CustomPropertyService {
        if (!CustomPropertyService.instance) {
            CustomPropertyService.instance = new CustomPropertyService();
        }

        return CustomPropertyService.instance;
    }

    load = async (
        accessor: CustomPropertyKey = CustomPropertyKey.FlowchartTemplateId,
        callback: (customProperty: CustomProperty) => void = () => null
    ) => {
        await Excel.run(async (context) => {
            const customDocProperties = context.workbook.properties.custom;
            const customProperty = customDocProperties.getItemOrNullObject(accessor);

            customProperty.load();

            await context.sync();

            if (customProperty.isNullObject) {
                console.log(`The custom property ${accessor} could not be loaded.`);
            } else {
                this.customProperties.set(customProperty.key as CustomPropertyKey, {
                    key: customProperty.key,
                    value: customProperty.value,
                    type: customProperty.type,
                });

                callback(this.customProperties.get(customProperty.key as CustomPropertyKey));
            }
        });
    };

    loadAll = async () => {
        await Excel.run(async (context) => {
            // Load the keys and values of all custom properties.
            const customDocProperties = context.workbook.properties.custom;
            customDocProperties.load(['key', 'value']);

            // Log each custom property to the console.
            // Note that your document may have more properties than those you have set using this snippet.
            customDocProperties.items.forEach((customProperty) => {
                this.customProperties.set(customProperty.key as CustomPropertyKey, {
                    key: customProperty.key,
                    value: customProperty.value,
                    type: customProperty.type,
                });
                console.log(`${customProperty.key}:${customProperty.value}`);
            });
        });
    };

    /**
     * Call load or loadAll before this method gets called.
     * @param accessor Custom property identifier.
     * @param callback callback func with custom property as param.
     */
    get = (accessor: string = 'FlowchartTemplateId', callback: (customProperty: CustomProperty) => void = () => null) => {
        const customProperty = this.customProperties.get(accessor as CustomPropertyKey);

        if (customProperty) {
            callback(customProperty);
        } else {
            console.log(`The custom property ${accessor} was not loaded yet.`);
        }
    };
}
