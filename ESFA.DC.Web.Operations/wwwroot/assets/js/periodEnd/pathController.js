import { Templates, getHandleBarsTemplate, Partials} from '/assets/js/handlebars-helpers.js';
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
        this._ilrPeriodEndTemplate = getPathTemplate(helpers, { pathHeader: Partials.ILRPathHeader });
        this._ilrPeriodEndNavigationTemplate = getHandleBarsTemplate(Templates.PeriodEndNavigation);
    }

    initialiseState(stateModel) {
        const mcaEnabled = stateModel.collectionClosed && stateModel.mcaReportsReady && !stateModel.mcaReportsPublished;
        const providerEnabled = stateModel.collectionClosed && stateModel.providerReportsReady && !stateModel.providerReportsPublished;
        const reportsFinished = !mcaEnabled && !providerEnabled && stateModel.mcaReportsReady && stateModel.providerReportsReady;

        setControlEnabledState(stateModel.collectionClosed && !stateModel.periodEndStarted, "startPeriodEnd");
        setControlEnabledState(mcaEnabled, "publishMcaReports");
        setControlEnabledState(providerEnabled, "publishProviderReports");
        setControlEnabledState(stateModel.collectionClosed && reportsFinished && !stateModel.periodEndFinished, "closePeriodEnd");
        setControlEnabledState(stateModel.collectionClosed && stateModel.periodEndFinished && stateModel.referenceDataJobsPaused, "resumeReferenceData");

        this._year = stateModel.year;
        this._period = stateModel.period;
    }

    renderPaths(stateModel) {
        updateSync.call(this);

        if (isStateModelDifferent(this._currentState, stateModel)) {
            document.getElementById("pathContainer").innerHTML = this._ilrPeriodEndTemplate({ viewModel: stateModel, yearPeriod:{ year: this._year, period: this._period }});
            this._currentState = stateModel;
        }
    }

    renderNavigation(stateModel) {
        document.getElementById("summaryContainer").innerHTML = this._ilrPeriodEndNavigationTemplate(stateModel);
    }

    registerHandlers(hub, state) {
        if (!state.isPreviousPeriod) {
            hub.registerMessageHandler("ReceiveMessage", (state) => this.renderPaths(JSON.parse(state)));
        }

        hub.registerMessageHandler("StartPeriodEndState", (enabled) => setControlEnabledState(enabled, "startPeriodEnd"));
        hub.registerMessageHandler("MCAReportsState", (enabled) => setControlEnabledState(enabled, "publishMcaReports"));
        hub.registerMessageHandler("ProviderReportsState", (enabled) => setControlEnabledState(enabled, "publishProviderReports"));
        hub.registerMessageHandler("PeriodClosedState", (enabled) => setControlEnabledState(enabled, "closePeriodEnd"));

        hub.registerMessageHandler("TurnOffMessage", () => {
            hub.unregisterMessageHandler("ReceiveMessage");
            hub.clearInterval();
        });

        hub.registerMessageHandler("ReferenceJobsButtonState", () => setControlEnabledState(true, "resumeReferenceData"));
        hub.registerMessageHandler("DisablePathItemProceed", (pathItemId) => setControlEnabledState(false, "proceed_" + pathItemId));
    }
}

export default pathController;