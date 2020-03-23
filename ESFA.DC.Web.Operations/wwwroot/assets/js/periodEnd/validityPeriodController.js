class ValidityPeriodController {

    constructor() {

        this._aBtnDownloadCSV = document.getElementById('aSaveList');

        this._data = {};

    }

    updatePage(data) {

        this._data = JSON.parse(data);

        this.drawGrid();

    }

    displayConnectionState(state) {

        const stateLabel = document.getElementById("state");

        stateLabel.textContent = `Status: ${state}`;

    }

    drawGrid() {

        var sb = [];
        for (var i = 0; i < this._data.length; i++) {
            var item = this._data[i];
            var enabled = item.enabled ? true : false;
            var checked = enabled ? 'checked="checked"' : '';

            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell">${item.collection}</td>`);
            sb.push(`<td class="govuk-table__cell">
                        <div class="govuk-checkboxes">
                            <div class="flex">
                                <div class="govuk-checkboxes__item">
                                    <input class="govuk-checkboxes__input"
                                           id="chkEnabled"
                                           name="chkEnabled"
                                           type="checkbox"
                                           ${checked}
                                           value="${enabled}" 
                                           onchange="window.checkedChanged(${i}, this);"/>
                                    <label class="govuk-label govuk-checkboxes__label">
                                    </label>
                                </div>
                            </div>
                        </div>
                     </td>`);
            sb.push(`</tr>`);
        }
        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;
    }

    getData(connection, action, period) {
        connection
            .invoke(action, period)
            .catch(
                err =>
                    console.error(err.toString())
            );
    }

    saveList(connection, action, period) {
        connection
            .invoke(action, period, this._data)
            .catch(
                err =>
                    console.error(err.toString())
            );
    }
}

export default ValidityPeriodController