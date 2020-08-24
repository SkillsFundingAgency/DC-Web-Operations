import { removeSpaces } from './util.js';

test('removeSpaces removes all spaces', () => {
    expect(removeSpaces(" remove all    spaces ")).toBe("removeallspaces");
});
