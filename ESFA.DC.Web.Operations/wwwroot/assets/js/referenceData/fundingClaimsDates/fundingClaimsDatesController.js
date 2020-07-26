import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class FundingClaimsDatesController {

    constructor() {
        this._fundingClaimsDatesList = {}
    }

    addEvents() {
        //this._modifyBtn = document.querySelectorAll('#modify');
        const modifyButtons = document.querySelectorAll('#modify');
        for (const modifyButton of modifyButtons) {
            modifyButton.addEventListener("click", this.modify.bind(this));
        }

        const cancelLinks = document.querySelectorAll('#cancel');
        for (const cancel of cancelLinks) {
            cancel.addEventListener("click", this.cancel.bind(this));
        }

        const saveButtons = document.querySelectorAll('#save');
        for (const saveBtn of saveButtons) {
            saveBtn.addEventListener("click", this.save.bind(this));
        }

        //const forms = document.querySelectorAll('#fundingClaimsDatesForm');
        //for (const form of forms) {
        //    form.addEventListener("submit", this.submit.bind(this));
        //}
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    init() {
        console.log("init fundingclaimsdatescontroller");

    }

    getFundingClaimsDates() {
        console.log("init getFundingClaimsDates");
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaData(this.populateFundingClaimsDates.bind(this));
    }

    populateFundingClaimsDates(fundingClaimsDatesList) {

        this._fundingClaimsDatesList = fundingClaimsDatesList;
        this.render(fundingClaimsDatesList);
        console.log("populateFundingClaimsDates getFundingClaimsDates");
    }

    render(fundingClaimsDatesList) {
        var compiledTemplate = getHandleBarsTemplate(Templates.FundingClaimsDatesList);
        document.getElementById("fundingClaimsDatesList").innerHTML = compiledTemplate({ viewModel: fundingClaimsDatesList });
        this.addEvents();
    }

    modify(event) {
        let id = event.target.dataset.id;
        let found = this._fundingClaimsDatesList.find(x => x.id == id);
        found.inEditMode = true;
        this.render(this._fundingClaimsDatesList);
    }

    cancel(event) {
        let id = event.target.dataset.id;
        let found = this._fundingClaimsDatesList.find(x => x.id == id);
        found.inEditMode = false;
        this.render(this._fundingClaimsDatesList);
    }

    save(event) {
        let form = event.target.closest('form');
        var formtojson = this.formToJson(form);
        //var formData = new FormData(form);
        //var json = this.toJson(formData);
        //var data = this.serialize(form);
        //var jsonData = this.formToJSON(form.elements);
        //console.log(jsonData);
        window.fundingClaimsDatesClient.updateFundingClaimsCollectionMetadata(formtojson);
        this.render(this._fundingClaimsDatesList);

    }

    submit(event) {
        let tr = event.target.closest('form');

        // push changes.
        this.render(this._fundingClaimsDatesList);
    }

    findParentForm(elem) {
        var parent = elem.parentNode;
        if (parent && parent.tagName != 'FORM') {
            parent = this.findParentForm(parent);
        }
        return parent;
    }

    formToJson(formElement) {
        //var inputElements = formElement.getElementsByTagName("input , select"),
        //    jsonObject = {};
        //for (var i = 0; i < inputElements.length; i++) {
        //    var inputElement = inputElements[i];
        //    jsonObject[inputElement.name] = inputElement.value;

        //}
        //return JSON.stringify(jsonObject);

        let json = {};
        Array.from(formElement.querySelectorAll('input, select, textarea'))
            .filter(element => element.name)
            .forEach(element => {
                json[element.name] = element.type === 'checkbox' ? element.checked : element.value;
            });
        return json;
    }

    toJson(formData) {
        var object = {};
        formData.forEach(function (value, key) {
            if (!Reflect.has(object, key)) {
                object[key] = value;
                return;
            }
            if (!Array.isArray(object[key])) {
                object[key] = [object[key]];
            }
            object[key].push(value);
        });
        var json = JSON.stringify(object);
        console.log(json);
    };

    formToJSON(elements) {
        return [].reduce.call(elements, function (data, element) {

            // Make sure the element has the required properties and should be added.
            if (this.isValidElement(element) && this.isValidValue(element)) {

                /*
                 * Some fields allow for more than one value, so we need to check if this
                 * is one of those fields and, if so, store the values as an array.
                 */
                if (this.isCheckbox(element)) {
                    data[element.name] = (data[element.name] || []).concat(element.value);
                } else if (this.isMultiSelect(element)) {
                    data[element.name] = this.getSelectValues(element);
                } else {
                    data[element.name] = element.value;
                }
            }

            return data;
        }, {});
    }

    isValidElement(element) {
        return element.name && element.value;
    }

    isValidValue(element) {
        return !['checkbox', 'radio'].includes(element.type) || element.checked;
    }

    /**
     * Checks if an input is a checkbox, because checkboxes allow multiple values.
     * @param  {Element} element  the element to check
     * @return {Boolean}          true if the element is a checkbox, false if not
     */
    isCheckbox(element) {
        return element.type === 'checkbox';
    }

    /**
     * Checks if an input is a `select` with the `multiple` attribute.
     * @param  {Element} element  the element to check
     * @return {Boolean}          true if the element is a multiselect, false if not
     */
    isMultiSelect(element) {
        return element.options && element.multiple;
    }

    getSelectValues(options) {
        return [].reduce.call(options, function (values, option) {
            return option.selected ? values.concat(option.value) : values;
        }, []);
    };

    serialize(form) {

        // Setup our serialized data
        var serialized = [];

        // Loop through each field in the form
        for (var i = 0; i < form.elements.length; i++) {

            var field = form.elements[i];

            // Don't serialize fields without a name, submits, buttons, file and reset inputs, and disabled fields
            if (!field.name || field.disabled || field.type === 'file' || field.type === 'reset' || field.type === 'submit' || field.type === 'button') continue;

            // If a multi-select, get all selections
            if (field.type === 'select-multiple') {
                for (var n = 0; n < field.options.length; n++) {
                    if (!field.options[n].selected) continue;
                    serialized.push(encodeURIComponent(field.name) + "=" + encodeURIComponent(field.options[n].value));
                }
            }

            // Convert field data to a query string
            else if ((field.type !== 'checkbox' && field.type !== 'radio') || field.checked) {
                serialized.push(encodeURIComponent(field.name) + "=" + encodeURIComponent(field.value));
            }
        }

        return serialized.join('&');

    };
}

export default FundingClaimsDatesController