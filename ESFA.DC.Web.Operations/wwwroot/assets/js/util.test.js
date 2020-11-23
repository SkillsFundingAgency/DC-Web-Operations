import { removeSpaces, sToTime, arrayHasItems } from './util.js';

describe("removeSpaces", () => {
    test('removeSpaces removes all spaces', () => {
        expect(removeSpaces(" remove all spaces ")).toBe("removeallspaces");
    });
});

describe("sToTime", () => {
    test('converts to correct hours', () => {
        expect(sToTime(3600)).toBe('01: 00: 00 secs');
    });

    test('converts to correct mins', () => {
        expect(sToTime(60)).toBe('00: 01: 00 secs');
    });

    test('converts to correct secs', () => {
        expect(sToTime(30)).toBe('00: 00: 30 secs');
    });
});

describe("arrayHasItems", () => {
    test('returns false when null', () => {
        expect(arrayHasItems(null)).toBe(false);
    });

    test('returns false when new array', () => {
        expect(arrayHasItems(new Array())).toBe(false);
    });

    test('returns false when empty array', () => {
        expect(arrayHasItems([])).toBe(false);
    });

    test('returns true when array has items', () => {
        expect(arrayHasItems(['a', 'b'])).toBe(true);
    });
});

