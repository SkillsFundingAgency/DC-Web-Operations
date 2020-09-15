import { updateSync } from '/assets/js/baseController.js';
import { setControlEnabledState } from '/assets/js/util.js';
import { Templates, Partials, getHandleBarsTemplate } from '/assets/js/handlebars-helpers.js';
import { getPathTemplate, isStateModelDifferent } from '/assets/js/periodEnd/basePathController.js';
import * as helpers from '/assets/js/periodEnd/periodEndUtil.js';

class pathController {

    constructor() {
        this._slowTimer = null;
        this._year = 0;
        this._period = 0;
        this._currentState = null;
        this._periodEndTemplate = getPathTemplate(helpers, { pathHeader: Partials.ALLFPathHeader });
        this._allfFileListTemplate = getHandleBarsTemplate(Templates.ALLFPeriodEndFileList);
    }

    initialiseState(stateModel) {
        setControlEnabledState(!stateModel.periodEndFinished, "uploadFile");
        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndStarted, "startPeriodEnd");
        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndFinished && stateModel.closePeriodEndEnabled, "closePeriodEnd");

        this._year = stateModel.year;
        this._period = stateModel.period;
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
        if (isStateModelDifferent(this._currentState, stateModel )) {
            document.getElementById('pathContainer').innerHTML = this._periodEndTemplate({ viewModel: stateModel, yearPeriod: { year: this._year, period: this._period } });
            this._currentState = stateModel;
        }
    }
}

export default pathController;