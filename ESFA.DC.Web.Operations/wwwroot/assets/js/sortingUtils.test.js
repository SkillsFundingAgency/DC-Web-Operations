import { sortByProviderName } from './sortingUtils.js';

describe("sortByProviderName", () => {

    test('sorts from A to Z', () => {
        // Arrange
        const first = { providerName: 'Za' };
        const second = { providerName: 'Az' };

        // Act
        const result = sortByProviderName(first, second);

        // Assert
        expect(result).toBe(1);
    });

    test('treats falsey values as empty string for comparison', () => {
        // Arrange
        const first = { providerName: null };
        const second = { };

        // Act
        const result = sortByProviderName(first, second);

        // Assert
        expect(result).toBe(0);
    });
})

