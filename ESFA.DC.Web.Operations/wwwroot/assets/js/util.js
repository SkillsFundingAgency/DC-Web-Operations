﻿export function removeSpaces(str) {
    return str.replace(/\s+/g, "");;
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