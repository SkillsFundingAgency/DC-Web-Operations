import { Templates, Partials, registerHelper, registerHelpers, getHandleBarsTemplate, registerPartialTemplate } from '/assets/js/handlebars-helpers.js';
import { removeSpaces } from '/assets/js/util.js';

export const getPathTemplate = function (templateHelpers, partialOverrides = {}) {
    registerHelpers(templateHelpers);
    registerHelper('removeSpaces', removeSpaces);

    registerPartialTemplate('proceedButton', partialOverrides.proceedButton || Partials.ProceedButton);
    registerPartialTemplate('pathItemJobSummary', partialOverrides.pathItemJobSummary || Partials.PathItemJobSummary);
    registerPartialTemplate('proceedableItemWrapper', partialOverrides.proceedableItemWrapper || Partials.ProceedableItemWrapper);
    registerPartialTemplate('pathHeader', partialOverrides.pathHeader || Partials.PathHeader);

    return getHandleBarsTemplate(Templates.PeriodEnd);
}

export const isStateModelDifferent = function(currentState, newState) {
    return JSON.stringify(currentState) !== JSON.stringify(newState);
}


