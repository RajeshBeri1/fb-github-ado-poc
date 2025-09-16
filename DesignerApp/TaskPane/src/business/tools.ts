import { Color } from "@omniflow/omni-webapi";

export default class Tools {
    constructor() {}


    /**
     * Get rgb array from hex string
     * @param hexString rgb array e.g. [100, 50, 255]
     * @returns 
     */
    static hexToRgb = (hexString: string): Array<number> =>
    {
        return hexString.replace('#', '').match(/.{1,2}/g).map(x => parseInt(x, 16));
    }

    /**
     * Get hex code from rgb
     * @param r red
     * @param g green
     * @param b blue
     * @returns 
     */
    static rgbToHex = (r, g, b) => '#' + [r, g, b].map(x =>
    {
        const hex = (x === null || x === undefined || x.length === 0) ? '' : x.toString(16);
        return hex.length === 1 ? '0' + hex : hex
    }).join('')


    /**
     * Get hex code from color model
     * @param color color model
     * @returns 
     */
    static rgbColorToHex(color: Color): string {
        try {

            if(color.Red === -1 || color.Green === - 1 || color.Blue === -1) {
                return null;
            }

            return this.rgbToHex(color.Red, color.Green, color.Blue);
        } catch(e) {
            return null;
        }
    }


    /**
     * Create a deep copy of anything
     * @param obj any Object
     */
    static deepCopy<T>(obj: T): T {
        if (!obj) {
            return obj;
        }
        return JSON.parse(JSON.stringify(obj, this.getCircularReplacer())) as T;
    }
    /**
     * Merge the current object with a new object.
     * Properties of the new object overrides properties of the current object
     * @param current current  object
     * @param target target object
     */
    static mergeDeep<T>(current: T, ...target): T {
        if (!target.length) {
            return current;
        }
        const source = target.shift();
        if (this.isObject(current) && this.isObject(source)) {
            for (const key in source) {
                if (this.isObject(source[key])) {
                    if (!current[key]) {
                        Object.assign(current, { [key]: {} });
                    }
                    this.mergeDeep<T>(current[key], source[key]);
                } else {
                    Object.assign(current, { [key]: source[key] });
                }
            }
        }
        return this.mergeDeep<T>(current, ...target);
    }

    /**
     * Merge values from source to target object.
     * @param from Source object
     * @param to Target object
     */
    static mergeDeepValues<T>(from, to: T): T {
        if (!from) {
            return to;
        }
        if (this.isObject(from) && this.isObject(to)) {
            Object.keys(from).forEach((key) => {
                if (this.isObject(from[key]) || this.isObject(to[key])) {
                    this.mergeDeepValues<T>(from[key], to[key]);
                } else {
                    to[key] = from[key] === null ? to[key] : from[key];
                }
            });
            return to;
        }
        return from;
    }

    static mergeObjects(obj1, obj2) {
        const result = {};
      
        function merge(target, source) {
          for (const key in source) {
            if (source.hasOwnProperty(key)) {
              if (source[key] != null && typeof source[key] === 'object') {
                if (!target[key]) {
                  target[key] = source[key];
                } else {
                  merge(target[key], source[key]);
                }
              } else {
                if(!!source[key] && source[key] !== -1 ) {
                    target[key] = source[key];
                }
              }
            }
          }
        }
      
        merge(result, obj1);
        merge(result, obj2);
      
        return result;
      }
      

    /**
     * Filter circular dependencies
     */
    static getCircularReplacer() {
        const seen = new WeakSet();
        return (key, value) => {
            if (typeof value === 'object' && value !== null) {
                if (seen.has(value)) {
                    return;
                }
                seen.add(value);
            }
            return value;
        };
    }

    /**
     * Parse a given value to boolen
     * @param val val
     */
    static getBool(val: boolean | string | number | undefined): boolean {
        try {
            return !!JSON.parse(String(val).toLowerCase());
        } catch (err) {
            return false;
        }
    }

    /**
     * Escape regular expression
     * @param expression expression
     */
    static escapeRegExp(expression: string) {
        if (expression) {
            return expression.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'); // $& means the whole matched string
        }
        return expression;
    }

    /**
     * Check if item is an object
     * @param item item
     */
    static isObject(item) {
        return item && typeof item === 'object' && !Array.isArray(item);
    }

    /**
     * Check if str is json
     */
    static isJson(str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    }

    /**
     * Get UUID - by underlying v4 conventions.
     */
    static uuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (Math.random() * 16) | 0,
                v = c == 'x' ? r : (r & 0x3) | 0x8;
            return v.toString(16);
        });
    }

    static uuidEmpty() {
        return 'xxxxxxxx-xxxx-xxxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/, '0');
    }

    /**
     * Check if uuid is valid.
     * @param uuid uuid
     * @returns boolean
     */
    static isUuid(uuid: string): boolean {
        const uuidRegex = '[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}';
        return !!uuid.match(uuidRegex);
    }

    /**
     * Get list of changes from two objects.
     * @param list empty array
     * @returns
     */
    static getDiff = (list: Array<any>) => {
        return {
            VALUE_CREATED: 'created',
            VALUE_UPDATED: 'updated',
            VALUE_DELETED: 'deleted',
            VALUE_UNCHANGED: 'unchanged',
            map: function (obj1, obj2) {
                if (this.isFunction(obj1) || this.isFunction(obj2)) {
                    throw 'Invalid argument. Function given, object expected.';
                }
                if (this.isValue(obj1) || this.isValue(obj2)) {
                    const valueType = this.compareValues(obj1, obj2);
                    return [
                        {
                            type: valueType,
                            data: obj1 === undefined ? obj2 : obj1,
                        },
                        list,
                    ];
                }

                var diff = {};
                for (var key in obj1) {
                    if (this.isFunction(obj1[key])) {
                        continue;
                    }

                    var value2 = undefined;
                    if (obj2[key] !== undefined) {
                        value2 = obj2[key];
                    }

                    const changed = this.map(obj1[key], value2);
                    diff[key] = changed[0];
                    switch (diff[key]['type']) {
                        case this.VALUE_CREATED:
                        case this.VALUE_DELETED:
                        case this.VALUE_UPDATED:
                            list.push({
                                [key]: { old: obj1[key], new: value2 },
                            });
                        case this.VALUE_UNCHANGED:
                    }
                }
                for (var key in obj2) {
                    if (this.isFunction(obj2[key]) || diff[key] !== undefined) {
                        continue;
                    }

                    const changed = this.map(undefined, obj2[key]);
                    diff[key] = changed[0];
                    switch (diff[key]['type']) {
                        case this.VALUE_CREATED:
                            Object.keys(diff[key]['data']).forEach((k) => {
                                list.push({
                                    [k]: {
                                        old: null,
                                        new: diff[key]['data'][k],
                                    },
                                });
                            });

                        case this.VALUE_DELETED:
                        case this.VALUE_UPDATED:

                        case this.VALUE_UNCHANGED:
                    }
                }

                return [diff, list];
            },
            compareValues: function (value1, value2) {
                if (value1 === value2) {
                    return this.VALUE_UNCHANGED;
                }
                if (this.isDate(value1) && this.isDate(value2) && value1.getTime() === value2.getTime()) {
                    return this.VALUE_UNCHANGED;
                }
                if (value1 === undefined) {
                    return this.VALUE_CREATED;
                }
                if (value2 === undefined) {
                    return this.VALUE_DELETED;
                }
                return this.VALUE_UPDATED;
            },
            isFunction: function (x) {
                return Object.prototype.toString.call(x) === '[object Function]';
            },
            isArray: function (x) {
                return Object.prototype.toString.call(x) === '[object Array]';
            },
            isDate: function (x) {
                return Object.prototype.toString.call(x) === '[object Date]';
            },
            isObject: function (x) {
                return Object.prototype.toString.call(x) === '[object Object]';
            },
            isValue: function (x) {
                return !this.isObject(x) && !this.isArray(x);
            },
        };
    };

    /**
     * Try to find a specific property in an object
     * e.g.
     *
     *   var Car = {
     *     Engine: {
     *       Cylinder: 12,
     *       Displacement: 8
     *     },
     *     Wheels: [
     *       { size: 20 },
     *       { size: 20 },
     *       { size: 22 },
     *       { size: 22 },
     *     ],
     *     Interior: {
     *       Seats: [
     *         { color: 'black' },
     *         { color: 'red' },
     *         { color: 'green' },
     *         { color: 'yellow' },
     *       ]
     *     }
     *   }
     *
     * Example 1: get the Cylinder Prop
     *
     * getValueByPath('Engine.Cylinder', Car)
     *
     * Example 2: get the color of the 3th seat
     *
     * const color = getValueByPath<Color>('Interior.Seats.2.color', Car)
     *
     *
     *
     *
     * @param path Path to the (nested)property
     * @param obj The (nested) object we want to search in.
     * @returns Returns the value from given path.
     *
     */
    static getValueByPath<T>(path: string[] | string, obj): T {
        const properties = Array.isArray(path) ? path : path.split('.');
        const result = properties.reduce<T>((prev: T, curr: string) => {
            return prev && prev[curr];
        }, obj);
        return result;
    }

    /**
     * Update the value of given object and property.
     * @param obj Object to update the property
     * @param path The property path
     * @param value The value
     * @returns 
     */
    static setProperty = (obj, path, value) => {
        const [head, ...rest] = path.split('.')
    
        return {
            ...obj,
            [head]: rest.length
                ? this.setProperty(obj[head], rest.join('.'), value)
                : value
        }
    }
}
