import '@testing-library/jest-dom'
const fs = require('fs');
const path = require('path');
const Handlebars = require('Handlebars');

beforeAll(() => {
    Handlebars.registerHelper('removeSpaces', (item) => item);
    Handlebars.registerHelper('isPathItemCurrent', () => false);
    Handlebars.registerHelper('includeInNav', () => true);
});

describe('ILR Navigation', () => {

    const templateHtml = fs.readFileSync(path.resolve(__dirname, `PeriodEndNavigation.html`), 'utf8');
    const template = Handlebars.compile(templateHtml.toString());

    test('should render each path as link', () => {
        // Arrange
        const viewModel = { paths: [{ name: 'First', pathId: 1 }, { name: 'Second', pathId: 2 }, { name: 'Third', pathId: 3 }] };

        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        const pathHeadings = document.querySelectorAll(`.govuk-heading-s`);
        expect(pathHeadings.length).toBe(3);
        expect(pathHeadings[0].text).toBe(viewModel.paths[0].name);
        expect(pathHeadings[1].text).toBe(viewModel.paths[1].name);
        expect(pathHeadings[2].text).toBe(viewModel.paths[2].name);
    });

    test('should render each path item as link', () => {
        // Arrange
        const viewModel = { "paths": [{ "name": "Path", "pathId": 1, pathItems: [{ pathId: 1, name: 'First' }, { pathId: 2, name: 'Second'}]}]};

        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        const pathItemHeadings = document.querySelectorAll('.small-link');
        expect(pathItemHeadings.length).toBe(2);
        expect(pathItemHeadings[0].text).toBe(viewModel.paths[0].pathItems[0].name);
        expect(pathItemHeadings[1].text).toBe(viewModel.paths[0].pathItems[1].name);
    });

    test('should add active attribute to current path item', () => {
        // Arrange
        const viewModel = { "paths": [{ "name": "Path", "pathId": 1, pathItems: [{ pathId: 1, name: 'Past item', ordinal: 1}, { pathId: 2, name: 'Current item', ordinal:2}] }] };
        Handlebars.registerHelper('isPathItemCurrent', (ordinal) => { return ordinal === viewModel.paths[0].pathItems[1].ordinal ? true : false });

        // Act
        document.body.innerHTML = template(viewModel);

        // Assert
        const pathHeadings = document.querySelectorAll('.active');
        expect(pathHeadings.length).toBe(1);
        expect(pathHeadings[0].text).toBe(viewModel.paths[0].pathItems[1].name);
    });

});