class JobProcessingController {
    constructor() {
        this._firstDonut = document.getElementById("firstDonut");        
        this._firstCircle = document.getElementById("firstCircle");

        this._percentColors = [
            { pct: 1, color: { r: 0x00, g: 0xff, b: 0 } },
            { pct: 50, color: { r: 0xff, g: 0xff, b: 0 } },
            { pct: 100, color: { r: 0xff, g: 0x00, b: 0 } }
        ];
    }

    updatePage(data) {
        data = JSON.parse(data);
        
        this._firstDonut.setAttribute("data-count", data.jobCount);;
        this._firstDonut.textContent = data.jobCount;

        let percentage = (data.jobCount / 125) * 100;
        this._firstCircle.setAttribute("stroke-dasharray", `${percentage},100`);
        this._firstCircle.setAttribute("style", "stroke:" + this.getColorForPercentage(percentage));
        
        var sb = [];
        sb.push(`<div class="govuk-summary-list__row ilr">`);
        for (var i = 0; i < data.jobCount; i++) {
            var item = data.jobs[i];
            sb.push(`<dt class="govuk-summary-list__value"><a href="#">${item.providerName}</a></dt>`);
            sb.push(`<dd class="govuk-summary-list__value">${item.ukprn}</dd>`);
            sb.push(`<dd class="govuk-summary-list__value">${item.timeTaken}</dd>`);
            sb.push(`<dd class="govuk-summary-list__value">${item.averageProcessingTime}</dd>`);
        }
        sb.push(`</div>`);

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;
    }

    getColorForPercentage(pct) {
        var i = 1;
        for (; i < this._percentColors.length - 1; i++) {
            if (pct < this._percentColors[i].pct) {
                break;
            }
        }

        var lower = this._percentColors[i - 1];
        var upper = this._percentColors[i];
        var range = upper.pct - lower.pct;
        var rangePct = (pct - lower.pct) / range;
        var pctLower = 1 - rangePct;
        var pctUpper = rangePct;
        var color = {
            r: Math.floor(lower.color.r * pctLower + upper.color.r * pctUpper),
            g: Math.floor(lower.color.g * pctLower + upper.color.g * pctUpper),
            b: Math.floor(lower.color.b * pctLower + upper.color.b * pctUpper)
        };
        return 'rgb(' + [color.r, color.g, color.b].join(',') + ')';
    }

    displayConnectionState(state) {
        //const stateLabel = document.getElementById("state");
        //stateLabel.textContent = `Status: ${state}`;
    }
}

export default JobProcessingController