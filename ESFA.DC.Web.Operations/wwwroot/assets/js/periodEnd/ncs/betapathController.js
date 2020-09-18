import { Templates, getHandleBarsTemplate, Partials } from '/assets/js/handlebars-helpers.js';
import { updateSync } from '/assets/js/baseController.js';
import { setControlEnabledState } from '/assets/js/util.js';
import { getPathTemplate, isStateModelDifferent } from '/assets/js/periodEnd/basePathController.js';
import * as helpers from '/assets/js/periodEnd/periodEndUtil.js';

class pathController {

    constructor() {
        this._year = 0;
        this._period = 0;
        this._currentState = null;
        this._slowTimer = null;
        this.lastMessage = null;
        this._ncsPeriodEndTemplate = getPathTemplate(helpers);
        this._ncsPeriodEndNavigationTemplate = getHandleBarsTemplate(Templates.PeriodEndNavigation);
    }

    initialiseState(stateModel) {

        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndStarted, "startPeriodEnd");
        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndFinished && stateModel.closePeriodEndEnabled, "closePeriodEnd");

        this._year = stateModel.year;
        this._period = stateModel.period;
    }

    renderPaths(stateModel) {
        updateSync.call(this);

        if (isStateModelDifferent(this._currentState, stateModel)) {
            document.getElementById("pathContainer").innerHTML = this._ncsPeriodEndTemplate({ viewModel: stateModel, yearPeriod: { year: this._year, period: this._period } });
            this._currentState = stateModel;
        }

        this.renderNavigation(stateModel);
    }

    renderNavigation(stateModel) {
        document.getElementById("summaryContainer").innerHTML = this._ncsPeriodEndNavigationTemplate(stateModel);
    }

    registerHandlers(hub, state) {
        if (!state.isPreviousPeriod) {
            hub.registerMessageHandler("ReceiveMessage", (state) => this.renderPaths(JSON.parse(state)));
        }

        hub.registerMessageHandler("StartPeriodEndState", (enabled) => setControlEnabledState(enabled, "startPeriodEnd"));
        hub.registerMessageHandler("PeriodClosedState", (enabled) => setControlEnabledState(enabled, "closePeriodEnd"));
        hub.registerMessageHandler("DisablePathItemProceed", (pathItemId) => setControlEnabledState(false, "proceed_" + pathItemId));

        hub.registerMessageHandler("TurnOffMessage", () => {
            hub.unregisterMessageHandler("ReceiveMessage");
            hub.clearInterval();
        });
    }
}

export default pathController;