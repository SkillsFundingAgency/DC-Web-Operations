export function removeSpaces(str) {
    return str.replace(/\s+/g, "");
}

export function padLeft(str, padString, max) {
    str = str.toString();
    return str.length < max ? padLeft(padString + str, padString, max) : str;
}

export function getColorForPercentage(pct) {
    var _percentColors = [
        { pct: 1, color: { r: 0x00, g: 0xff, b: 0 } },
        { pct: 50, color: { r: 0xff, g: 0xff, b: 0 } },
        { pct: 100, color: { r: 0xff, g: 0x00, b: 0 } }
    ];

    var i = 1;
    for (; i < _percentColors.length - 1; i++) {
        if (pct < _percentColors[i].pct) {
            break;
        }
    }

    var lower = _percentColors[i - 1];
    var upper = _percentColors[i];
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

export function getMessageForPercentage(percentage, options) {
    let i = 0, len = options.length;
    for (; i < len; i++) {
        if (options[i].value <= percentage) {
            return options[i].label;
        }
    }

    return options[0].label;
}

export function getDatetimeFromString(dateString) {
    return new Date(dateString);
}

export function getUkDateString(dateString) {
    const dateSplit = dateString.substring(0, dateString.indexOf('T')).split('-');
    const formattedString = dateSplit[2] + '/' + dateSplit[1] + '/' + dateSplit[0];
    return formattedString;
}

export function getFormattedDatetimeString(dateString) {
    var dateObject = getDatetimeFromString(dateString);

    var month = padLeft(dateObject.getMonth() + 1, '0', 2);
    var day = padLeft(dateObject.getDate(), '0', 2);
    var hour = padLeft(dateObject.getHours(), '0', 2);
    var minute = padLeft(dateObject.getMinutes(), '0', 2);
    var second = padLeft(dateObject.getSeconds(), '0', 2);

    return `${dateObject.getFullYear()}-${month}-${day} ${hour}:${minute}:${second}`;
}

export function getFormattedTimeString(dateString) {
    var dateObject = getDatetimeFromString(dateString);

    var hour = padLeft(dateObject.getHours(), '0', 2);
    var minute = padLeft(dateObject.getMinutes(), '0', 2);
    var second = padLeft(dateObject.getSeconds(), '0', 2);

    return `${hour}:${minute}:${second}`;
}


export function setControlEnabledState(enabledState, controlId) {
    const control = document.getElementById(controlId);
    if (control) {
        control.disabled = !enabledState;
    }
}

export function setControlVisiblity(isVisible, controlId) {
    const control = document.getElementById(controlId);
    if (control) {
        control.style.visibility = isVisible ? 'visible' : 'hidden';
    }
}

export function msToTime(duration) {
    var seconds = Math.floor((duration / 1000) % 60),
        minutes = Math.floor((duration / (1000 * 60)) % 60),
        hours = Math.floor((duration / (1000 * 60 * 60)) % 24);

    hours = (hours < 10) ? "0" + hours : hours;
    minutes = (minutes < 10) ? "0" + minutes : minutes;
    seconds = (seconds < 10) ? "0" + seconds : seconds;

    return hours + ": " + minutes + ": " + seconds + " secs";
}

export function replaceNullOrEmpty(stringValue, replaceValue) {
    if (
        (typeof stringValue == 'undefined')
        ||
        (stringValue == null)
        ||        
        (stringValue.length == 0)
        ||
        (stringValue == "")
        ||
        (stringValue.replace(/\s/g, "") == "")
        ||
        (stringValue == 'null')
    ) {
        return replaceValue;
    }
    else {
        return stringValue;
    }
}

export function sumArrayProperty(array, key) {
    if (array) {
        return array.reduce((a, b) => a + (b[key] || 0), 0);
    }
    return 0;
}

export function getInitialStateModel() {
    const stateContainer = document.getElementById('initialState');
    return stateContainer && stateContainer.innerHTML ? JSON.parse(stateContainer.innerHTML) : {};
} 

export function displayConnectionState(state) {
    const stateLabel = document.getElementById("state");
    if (stateLabel) {
        stateLabel.textContent = `Status: ${state}`;
    }
}

export function $on(target, type, callback, capture) {
    target.addEventListener(type, callback, !!capture);
}

export function $onAll(targets, type, callback, capture) {
    for (const target of targets) {
        $on(target, type, callback, capture);
    }
}

export function parseToObject(jsonToParse) {
    if (typeof jsonToParse !== 'object') {
        return JSON.parse(jsonToParse);
    }
    return jsonToParse;
}


