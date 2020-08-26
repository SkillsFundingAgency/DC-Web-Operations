import { getHandleBarsTemplate, registerPartialTemplate } from '/assets/js/handlebars-helpers.js';
import { Templates } from '/assets/js/periodEnd/handlebars-helpers.js';
import { updateSync } from '/assets/js/baseController.js';
import { setControlEnabledState } from '/assets/js/util.js';

class pathController {

    constructor() {
        this._slowTimer = null;
        this.lastMessage = null;
        this._year = 0;
        this._period = 0;
        this._currentState = null;

        registerPartialTemplate('proceedButton', Templates.ProceedButton);
        registerPartialTemplate('pathItemJobSummary', Templates.PathItemJobSummary);
        
        this._ilrPeriodEndTemplate = getHandleBarsTemplate(Templates.ILRPeriodEnd);
        this._ilrPeriodEndNavigationTemplate = getHandleBarsTemplate(Templates.ILRPeriodEndNavigation);
    }

    initialiseState(stateModel) {
        const mcaEnabled = stateModel.collectionClosed && stateModel.mcaReportsReady && !stateModel.mcaReportsPublished;
        const providerEnabled = stateModel.collectionClosed && stateModel.providerReportsReady && !stateModel.providerReportsPublished;
        const reportsFinished = !mcaEnabled && !providerEnabled && stateModel.mcaReportsReady && stateModel.providerReportsReady;

        setControlEnabledState(stateModel.collectionClosed && !state.periodEndStarted, "startPeriodEnd");
        setControlEnabledState(mcaEnabled, "publishMcaReports");
        setControlEnabledState(providerEnabled, "publishProviderReports");
        setControlEnabledState(stateModel.collectionClosed && reportsFinished && !stateModel.periodEndFinished, "closePeriodEnd");
        setControlEnabledState(stateModel.collectionClosed && stateModel.periodEndFinished && stateModel.referenceDataJobsPaused, "resumeReferenceData");

        this._year = stateModel.year;
        this._period = stateModel.period;
    }

    renderPaths(stateModel) {
        updateSync.call(this);

        if (JSON.stringify(this._currentState) !== JSON.stringify(stateModel)) {
            document.getElementById("pathContainer").innerHTML = this._ilrPeriodEndTemplate({ viewModel: stateModel, year: this._year, period: this._period });
            document.getElementById("summaryContainer").innerHTML = this._ilrPeriodEndNavigationTemplate({ viewModel: stateModel });
            this._currentState = stateModel;
        }
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