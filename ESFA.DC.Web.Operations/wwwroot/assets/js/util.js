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

export function getFormattedDatetimeString(dateString) {
    var dateObject = getDatetimeFromString(dateString);

    var month = padLeft(dateObject.getMonth() + 1, '0', 2);
    var day = padLeft(dateObject.getDate(), '0', 2);
    var hour = padLeft(dateObject.getHours(), '0', 2);
    var minute = padLeft(dateObject.getMinutes(), '0', 2);
    var second = padLeft(dateObject.getSeconds(), '0', 2);

    return `${dateObject.getFullYear()}-${month}-${day} ${hour}:${minute}:${second}`;
}

export function setControlEnabledState(enabledState, controlId) {
    const control = document.getElementById(controlId);
    if (control) {
        control.disabled = !enabledState;
    }
}

//export function generatePagination(config) {
//    const offset = 2;
//    var current = config.page;
//    var last = config.pages;
//    const leftOffset = current - offset;
//    const rightOffset = current + offset + 1;

//    /**
//     * Reduces a list into the page numbers desired in the pagination
//     * @param {array} accumulator - Growing list of desired page numbers
//     * @param {*} _ - Throwaway variable to ignore the current value in iteration
//     * @param {*} idx - The index of the current iteration
//     * @returns {array} The accumulating list of desired page numbers
//     */
//    function reduceToDesiredPageNumbers(accumulator, _, idx) {
//        const currIdx = idx + 1;

//        if (
//            // Always include first page
//            currIdx === 1
//            // Always include last page
//            || currIdx === last
//            // Include if index is between the above defined offsets
//            || (currIdx >= leftOffset && currIdx < rightOffset)) {
//            return [
//                ...accumulator,
//                currIdx,
//            ];
//        }

//        return accumulator;
//    }

//    /**
//     * Transforms a list of desired pages and puts ellipsis in any gaps
//     * @param {array} accumulator - The growing list of page numbers with ellipsis included
//     * @param {number} currentPage - The current page in iteration
//     * @param {number} currIdx - The current index
//     * @param {array} src - The source array the function was called on
//     */
//    function transformToPagesWithEllipsis(accumulator, currentPage, currIdx, src) {
//        const prev = src[currIdx - 1];

//        // Ignore the first number, as we always want the first page
//        // Include an ellipsis if there is a gap of more than one between numbers
//        if (prev != null && currentPage - prev !== 1) {
//            return [
//                ...accumulator,
//                '...',
//                currentPage,
//            ];
//        }

//        // If page does not meet above requirement, just add it to the list
//        return [
//            ...accumulator,
//            currentPage,
//        ];
//    }

//    const pageNumbers = Array(last)
//        .fill()
//        .reduce(reduceToDesiredPageNumbers, []);

//    const pageNumbersWithEllipsis = pageNumbers.reduce(transformToPagesWithEllipsis, []);

//    return pageNumbersWithEllipsis;
//}
