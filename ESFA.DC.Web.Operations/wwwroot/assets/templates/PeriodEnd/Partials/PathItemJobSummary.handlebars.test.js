const fs = require('fs');
const path = require('path');
const Handlebars = require('Handlebars');

describe('path item summary', () => {
    test('should display correct summary results', () => {
        // Arrange
        const templateHtml = fs.readFileSync(path.resolve(__dirname, `PathItemJobSummary.html`), 'utf8');
        const viewModel = { summary: { numberOfWaitingJobs: 1, numberOfRunningJobs: 2, numberOfFailedJobs: 3, numberOfCompleteJobs: 4 }};
        const template = Handlebars.compile(templateHtml.toString());

        // Act
        const result = template(viewModel);

        // Assert
        expect(result).toContain(`${viewModel.summary.numberOfWaitingJobs} Waiting`);
        expect(result).toContain(`${viewModel.summary.numberOfRunningJobs} Running`);
        expect(result).toContain(`${viewModel.summary.numberOfFailedJobs} Failed`);
        expect(result).toContain(`${viewModel.summary.numberOfCompleteJobs} Complete`);
    });
});
