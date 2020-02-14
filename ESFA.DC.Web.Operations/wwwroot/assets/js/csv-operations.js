export function convertToCsv(args) {
    var fName = args.filename;
    var rows = new Array();    
    for (var i = 0; i < args.data.length; i++) {
        var row = [];
        var item = args.data[i];

        if (i == 0) {
            for (var x in item) {
                row.push(x);
            }
            rows.push(row);
            row = [];
        }

        for (var x in item) {
            row.push(item[x])
        }

        rows.push(row);
    }

    var csv = '';
    for (var i = 0; i < rows.length; i++) {
        var row = rows[i];
        for (var j = 0; j < row.length; j++) {
            var val = row[j] === null ? '' : row[j].toString();
            val = val.replace(/\t/gi, " ");
            if (j > 0)
                csv += '\t';
            csv += val;
        }
        csv += '\n';
    }

    var cCode, bArr = [];
    bArr.push(255, 254);
    for (var i = 0; i < csv.length; ++i) {
        cCode = csv.charCodeAt(i);
        bArr.push(cCode & 0xff);
        bArr.push(cCode / 256 >>> 0);
    }

    var blob = new Blob([new Uint8Array(bArr)], { type: 'text/csv;charset=UTF-16LE;' });
    if (navigator.msSaveBlob) {
        navigator.msSaveBlob(blob, fName);
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) {
            var url = window.URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", fName);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        }
    }
}