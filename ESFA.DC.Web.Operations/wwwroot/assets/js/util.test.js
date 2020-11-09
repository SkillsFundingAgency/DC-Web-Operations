import { removeSpaces } from './util.js';

test('removeSpaces removes all spaces', () => {
    expect(removeSpaces(" remove all    spaces ")).toBe("removeallspaces");
});

import { sToTime } from './util.js'; describe("sToTime", () => {
    test('converts to correct hours', () => {
        expect(sToTime(3600)).toBe('01: 00: 00 secs');
    });
});

describe("sToTime", () => {
    test('converts to correct mins', () => {
        expect(sToTime(60)).toBe('00: 01: 00 secs');
    });
});

describe("sToTime", () => {
    test('converts to correct secs', () => {
        expect(sToTime(30)).toBe('00: 00: 30 secs');
    });
});