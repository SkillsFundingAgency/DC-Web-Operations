import { updateSync } from '/assets/js/baseController.js';
import { setControlEnabledState, removeSpaces } from '/assets/js/util.js';
import { Templates, Partials, registerHelper, registerHelpers, getHandleBarsTemplate, registerPartialTemplate  } from '/assets/js/handlebars-helpers.js';
import * as helpers from '/assets/js/periodEnd/periodEndUtil.js';

class pathController {

    constructor() {
        this._slowTimer = null;
        this._year = 0;
        this._period = 0;
        this._periodEndTemplate = null;
        this._allfFileListTemplate = null;
        this._currentState = null;
    }

    initialiseState(stateModel) {

        setControlEnabledState(!stateModel.periodEndFinished, "uploadFile");
        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndStarted, "startPeriodEnd");
        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndFinished && stateModel.closePeriodEndEnabled, "closePeriodEnd");

        this._year = stateModel.year;
        this._period = stateModel.period;
        this.initialiseTemplating();
    }

    initialiseTemplating() {
        registerHelpers(helpers);
        registerHelper('removeSpaces', removeSpaces);

        registerPartialTemplate('proceedButton', Partials.ProceedButton);
        registerPartialTemplate('pathItemJobSummary', Partials.PathItemJobSummary);
        registerPartialTemplate('proceedableItemWrapper', Partials.ProceedableItemWrapper);
        registerPartialTemplate('pathHeader', Partials.ALLFPathHeader);

        this._periodEndTemplate = getHandleBarsTemplate(Templates.PeriodEnd);
        this._allfFileListTemplate = getHandleBarsTemplate(Templates.ALLFPeriodEndFileList);
     }

    registerHandlers(hub, state) {

        if (!state.isPreviousPeriod) {
            hub.registerMessageHandler("ReceiveMessage", (state) => this.renderPaths(JSON.parse(state)));
        }

        hub.registerMessageHandler("UploadState", (enabled) => setControlEnabledState(enabled, "uploadFile"));
        hub.registerMessageHandler("StartPeriodEndState", (enabled) => setControlEnabledState(enabled, "startPeriodEnd"));
        hub.registerMessageHandler("PeriodClosedState", (enabled) => setControlEnabledState(enabled, "closePeriodEnd"));
        hub.registerMessageHandler("DisablePathItemProceed", (pathItemId) => setControlEnabledState(false, `proceed_${pathItemId}`));

        hub.registerMessageHandler("TurnOffMessage", () => {
            hub.unregisterMessageHandler("UploadState");
            hub.unregisterMessageHandler("StartPeriodEndState");
            hub.unregisterMessageHandler("PeriodClosedState");
            hub.unregisterMessageHandler("ReceiveMessage");
            hub.clearInterval();
        });
    }

    renderFiles(stateModel) {
        updateSync.call(this);
        document.getElementById('fileContainer').innerHTML = this._allfFileListTemplate({ files: stateModel.files, period: this._period});

    }

    renderPaths(stateModel) {
        if (JSON.stringify(this._currentState) !== JSON.stringify(stateModel)) {
            document.getElementById('pathContainer').innerHTML = this._periodEndTemplate({ viewModel: stateModel, yearPeriod: { year: this._year, period: this._period } });
            this._currentState = stateModel;
        }
    }
}

export default pathController;