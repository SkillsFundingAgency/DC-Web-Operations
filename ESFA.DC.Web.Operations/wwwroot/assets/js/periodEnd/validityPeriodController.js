class ValidityPeriodController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aSaveList');
        this._data = {};
    }

    updatePage(data) {
        this._data = JSON.parse(data);
        this.drawGrid();
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
        hub.registerMessageHandler("GetValidityStructure", (data) => this.updatePage(data));
        hub.registerMessageHandler("UpdateValidityPeriod", (data) => this.updatePage(data));
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }



    drawPaths(pathId) {
        let path = null;
        
        for (let i = 0; i < this._data.length; i++) {
            if (this._data[i].pathId === pathId) {
                path = this._data[i];
                break;
            }
        }

        const name = path.name;
        const enabled = path.isValidForPeriod ? true : false;
        const checked = enabled ? 'checked="checked"' : '';
        const type = path.entityType;
        const id = pathId;
        const pathItems = path.pathItems;

        const pathHTML = this.generateHtml(name, enabled, checked, type, id);
    }

    generateHtml(itemName, enabledFlag, checkedAttribute, type, id) {
        return `<tr class="govuk-table__row">
                         <td class="govuk-table__cell">${itemName}</td>
                         <td class="govuk-table__cell">
                            <div class="govuk-checkboxes">
                                <div class="flex">
                                    <div class="govuk-checkboxes__item">
                                        <input class="govuk-checkboxes__input"
                                               id="chkEnabled"
                                               name="chkEnabled"
                                               type="checkbox"
                                               ${checkedAttribute}
                                               value="${enabledFlag}" 
                                               onchange="window.checkedChanged(${type}, ${id}, this);"/>
                                        <label class="govuk-label govuk-checkboxes__label">
                                        </label>
                                    </div>
                                </div>
                            </div>
                       </td>
                  </tr>`;
    }

    getData(connection, action, collectionYear, period) {
        connection
            .invoke(action, collectionYear, period)
            .catch(
                err =>
                    console.error(err.toString())
            );
    }

    saveList(connection, action, collectionYear, period) {
        connection
            .invoke(action, collectionYear, period, this._data)
            .catch(
                err =>
                    console.error(err.toString())
            );
    }
}

export default ValidityPeriodController