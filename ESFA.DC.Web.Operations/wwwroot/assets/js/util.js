export function removeSpaces(str) {
    return str.replace(/\s+/g, "");;
}

export function padLeft(str, padString, max) {
    str = str.toString();
    return str.length < max ? padLeft(padString + str, padString, max) : str;
}