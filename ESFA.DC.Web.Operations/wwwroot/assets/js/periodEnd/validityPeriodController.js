import { Templates, getHandleBarsTemplate } from '/assets/js/handlebars-helpers.js';
import { $on, $onAll } from '/assets/js/util.js';
import Client from '/assets/js/periodEnd/client.js';
import Hub from '/assets/js/hubs/hub.js';

class ValidityPeriodController {

    constructor() {
        this._hub = new Hub("validityPeriodHub", this.displayConnectionState);
        this._client = new Client(this._hub.getConnection());

        this._template = getHandleBarsTemplate(Templates.ValidityPeriod);

        this.registerHandlers();
        this._hub.startHub(this.getData.bind(this));

        $on(document.getElementById("collectionYear"), "change", () => { this.getData(); });
        $on(document.getElementById("period"), "change", () => { this.getData(); });

        
    }

    updatePage(data) {
        data = JSON.parse(data);
        this.renderStructure(data);
    }

    registerHandlers() {
        this._hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
        this._hub.registerMessageHandler("GetValidityStructure", (data) => this.updatePage(data));
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    renderStructure(data) {
        const container = document.getElementById("structureContainer");
        container.innerHTML = this._template({ viewModel: data });

        $onAll(document.querySelectorAll(".validityCheckbox"), "change", (e) => e.target.value = this.toggleCheckboxValue(e.target.value));
    }

    toggleCheckboxValue(currentValue) {
        if (currentValue === "true")
            return "false";
        else
            return "true";
    }

    getData() {
        const period = document.getElementById('period').value;
        const collectionYear = document.getElementById('collectionYear').value;
        this._client.invokeAction("GetValidityStructure", collectionYear, period);
    }
}

export const validityController = new ValidityPeriodController();