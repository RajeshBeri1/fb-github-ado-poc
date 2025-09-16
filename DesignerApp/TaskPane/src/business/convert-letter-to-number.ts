

export const convertLetterToNumber = (str: string) => {
    const start = 96; // "a".charCodeAt(0) - 1

    if (typeof str === 'string' && str.length === 1) {
        const val = str.toLowerCase().charCodeAt(0) - start;
        return val >= 0 ? val - 1 : -1;
    } else {
        const len = str.length;
        const strArr = str.toLowerCase().split('');
        const out = strArr.reduce((out, char, pos) => {
            const val = char.charCodeAt(0) - start;
            const pow = Math.pow(26, len - pos - 1);
            return out + val * pow;
        }, 0);
        return out - 1;
    }
};
