import { Templates, getHandleBarsTemplate } from '/assets/js/handlebars-helpers.js';
import Client from '/assets/js/periodEnd/client.js';
import Hub from '/assets/js/hubs/hub.js';

class ValidityPeriodController {

    constructor() {
        this._hub = new Hub("validityPeriodHub", this.displayConnectionState);
        this._client = new Client(this._hub.getConnection());

        this._template = getHandleBarsTemplate(Templates.ValidityPeriod);

        this.registerHandlers();
        this._hub.startHub(this.getData.bind(this));

        document.getElementById("collectionYear").addEventListener("click", this.getData.bind(this));
        document.getElementById("periodNumber").addEventListener("click", this.getData.bind(this));
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
    }

    getData() {
        const period = document.getElementById('periodNumber').value;
        const collectionYear = document.getElementById('collectionYear').value;
        this._client.invokeAction("GetValidityStructure", collectionYear, period);
    }
}

export default ValidityPeriodController