﻿import { updateSync } from '/assets/js/baseController.js';
import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class referenceDataController {

    constructor(downloadController) {
        this._slowTimer = null;
        this._downloadController = downloadController;
    }

    registerHandlers(hub, type) {
        hub.registerMessageHandler("ReceiveMessage", (state) => this.renderFiles(type, state));

        hub.registerMessageHandler("TurnOffMessage", () => {
            hub.unregisterMessageHandler("UploadState");
            hub.unregisterMessageHandler("ReceiveMessage");
            hub.clearInterval();
        });
    }
    
    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    sortByDate(stateModel) {

        stateModel.files.sort(function (a, b) {
            return Number(a.submissionDate) - Number(b.submissionDate);
        });
    }

    renderFiles(controllerName, state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);
        if (!stateModel || !stateModel.files) {
            return;
        }

        this.sortByDate(stateModel);

        var compiledTemplate = getHandleBarsTemplate(Templates.ReferenceDataFilesList);
        document.getElementById("filesList").innerHTML = compiledTemplate({ viewModel: stateModel, controllerName: controllerName, downloadController: this._downloadController });

        // Can be written in a single line.
        //document.getElementById("filesList").innerHTML = getHandleBarsTemplate(Templates.ReferenceDataFilesList)({ viewModel: stateModel,controllerName: controllerName });
    }
}

export default referenceDataController;