import { Templates, getHandleBarsTemplate, registerHelper} from '/assets/js/handlebars-helpers.js';
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

        registerHelper("isInitiatingItem", this.isInitiatingItem);
        registerHelper("disableCheckboxes", this.disableCheckboxes);

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

        $onAll(document.querySelectorAll(".validityCheckbox"), "change",
            (e) => {
                const hiddenItem = e.target.previousElementSibling;
                const state = this.toggleCheckboxValue(e.target, e.target.value);

                e.target.value = state;
                if(hiddenItem)
                    hiddenItem.value = state;
            }
        );
    }

    toggleCheckboxValue(element, currentValue) {
        if (currentValue === "true") {
            this.toggleChildCheckBoxes(element, false);
            return "false";
        }
        else {
            this.toggleChildCheckBoxes(element, true);
            return "true";
        }
    }

    toggleChildCheckBoxes(checkboxElement, checked) {
        const sibling = checkboxElement.closest(".checkbox-container").nextElementSibling;
        if (sibling && sibling.classList.contains("inner-list")) {
            const childCheckboxes = sibling.querySelectorAll("input[type=checkbox]");
            if (checked) {
                sibling.classList.remove("greyed-out");
            } else {
                sibling.classList.add("greyed-out");
            }

            childCheckboxes.forEach((c) => {
                c.disabled = !checked;
            });
        }
    }

    getData() {
        const period = document.getElementById('period').value;
        const collectionYear = document.getElementById('collectionYear').value;
        this._client.invokeAction("GetValidityStructure", collectionYear, period);
    }

    isInitiatingItem(isPausing, hasJobs, isHidden, entityType, options) {
        return isPausing === true && hasJobs === false && isHidden === false && entityType === 2;
    }

    disableCheckboxes(isValid, criticalParent, options) {
        return !isValid && !criticalParent;
    }
}

export const validityController = new ValidityPeriodController();