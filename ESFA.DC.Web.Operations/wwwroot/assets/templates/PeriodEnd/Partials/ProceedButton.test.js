import '@testing-library/jest-dom'
const fs = require('fs');
const path = require('path');
const Handlebars = require('Handlebars');

beforeAll(() => {
    Handlebars.registerHelper('getProceedLabelText', () => 'getProceedLabelText');
    Handlebars.registerHelper('getProceedButtonText', () => 'getProceedButtonText');
    Handlebars.registerHelper('canContinue', () => false);
});

describe('proceed button', () => {

    const templateHtml = fs.readFileSync(path.resolve(__dirname, `proceedButton.html`), 'utf8');
    const viewModel = { pathItem: { "pathId": 0, "pathItemId": 4 }, yearPeriod: { "year": 1920, "period": 1 } };

    test('should be disabled when can continue is false', () => {
        // Arrange
        const template = Handlebars.compile(templateHtml.toString());
    
        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        expect(document.querySelector('button[type="submit"]')).toBeDisabled();
    });

    test('should be enabled when can continue is true', () => {
        // Arrange
        const template = Handlebars.compile(templateHtml.toString());
        Handlebars.registerHelper('canContinue', () => true);

        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        expect(document.querySelector('button[type="submit"]')).toBeEnabled();
    });

    test('onclick should pass correct parameters to periodEnd Client', () => {
        // Arrange
        const template = Handlebars.compile(templateHtml.toString());

        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        expect(document.querySelector('button[type="submit"]').getAttribute('onclick')).toBe(`window.periodEndClient.proceed(${viewModel.yearPeriod.year},${viewModel.yearPeriod.period},${viewModel.pathItem.pathId},${viewModel.pathItem.pathItemId});this.disabled=true;`);
    });
});
