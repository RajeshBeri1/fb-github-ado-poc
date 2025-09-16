/**
 *  Type operator similar to keyOf -> extract all "values" of an Object as possible types.
 *  Definitions {
 *      definition1: string
 *      definition2: number
 *  }
 *  I.e. TValueOf<Definitions> ---> string | number
 **/
export type TValueOf<T> = T[keyof T];
