/*****************************************************
 * Paginator Function                                *
 *****************************************************
 * config : {
 *     get_rows : function used to select rows to do pagination on
 *         If no function is provided, checks for a config.table element and looks for rows in there to page
 *
 *     box : Empty element that will have page buttons added to it
 *         If no config.box is provided, but a config.table is, then the page buttons will be added using the table
 *
 *     table : table element to be paginated
 *         not required if a get_rows function is provided
 *
 *     rows_per_page : number of rows to display per page
 *         default number is 10
 *
 *     page: page to display
 *         default page is 1
 *
 *     box_mode: "list", "buttons", or function. determines how the page number buttons are built.
 *         "list" builds the page index in list format and adds class "pagination" to the ul element. Meant for use with bootstrap
 *         "buttons" builds the page index out of buttons
 *         if this field is a function, it will be passed the config object as its only param and assumed to build the page index buttons
 *
 *     page_options: false or [{text: , value: }, ... ] used to set what the dropdown menu options are available, resets rows_per_page value
 *         false prevents the options from being displayed
 *         [{text: , value: }, ... ] allows you to customize what values can be chosen, a value of 0 will display all the table's rows.
 *         the default setup is
 *           [
 *               { value: 5,  text: '5'   },
 *               { value: 10, text: '10'  },
 *               { value: 20, text: '20'  },
 *               { value: 50, text: '50'  },
 *               { value: 100,text: '100' },
 *               { value: 0,  text: 'All' }
 *           ]
 *
 *     active_class: set the class for page buttons to have when active.
 *          defaults to "active"
 *
 *     disable: true or false, shows all rows of the table and hides pagination controlls if set to true.
 *
 *     tail_call: function to be called after paginator is done.
 *
 * }
 */
function paginator(config) {
    // throw errors if insufficient parameters were given
    if (typeof config != "object")
        throw "Paginator was expecting a config object!";
    if (typeof config.get_rows != "function" && !(config.table instanceof Element))
        throw "Paginator was expecting a table or get_row function!";

    // get/set if things are disabled
    if (typeof config.disable == "undefined") {
        config.disable = false;
    }

    // get/make an element for storing the page numbers in
    var box;
    if (!(config.box instanceof Element)) {
        config.box = document.createElement("div");
    }
    box = config.box;

    // get/make function for getting table's rows
    if (typeof config.get_rows != "function") {
        config.get_rows = function () {
            var table = config.table
            var tbody = table.getElementsByTagName("tbody")[0]||table;

            // get all the possible rows for paging
            // exclude any rows that are just headers or empty
            children = tbody.children;
            var trs = [];
            for (var i=0;i<children.length;i++) {
                if (children[i].nodeType = "tr") {
                    if (children[i].getElementsByTagName("td").length > 0) {
                        trs.push(children[i]);
                    }
                }
            }

            return trs;
        }
    }
    var get_rows = config.get_rows;
    var trs = get_rows();

    // get/set rows per page
    if (typeof config.rows_per_page == "undefined") {
        var selects = box.getElementsByTagName("select");
        if (typeof selects != "undefined" && (selects.length > 0 && typeof selects[0].selectedIndex != "undefined")) {
            config.rows_per_page = selects[0].options[selects[0].selectedIndex].value;
        } else {
            config.rows_per_page = 10;
        }
    }
    var rows_per_page = config.rows_per_page;

    // get/set current page
    if (typeof config.page == "undefined") {
        config.page = 1;
    }
    var page = config.page;

    // get page count
    var pages = (rows_per_page > 0)? Math.ceil(trs.length / rows_per_page):1;

    // get/set current page
    if (typeof config.pages === "undefined") {
        config.pages = pages;
    }

    // check that page and page count are sensible values
    if (pages < 1) {
        pages = 1;
    }
    if (page > pages) {
        page = pages;
    }
    if (page < 1) {
        page = 1;
    }
    config.page = page;
 
    // hide rows not on current page and show the rows that are
    for (var i=0;i<trs.length;i++) {
        if (typeof trs[i]["data-display"] == "undefined") {
            trs[i]["data-display"] = trs[i].style.display||"";
        }
        if (rows_per_page > 0) {
            if (i < page*rows_per_page && i >= (page-1)*rows_per_page) {
                trs[i].style.display = trs[i]["data-display"];
            } else {
                // Only hide if pagination is not disabled
                if (!config.disable) {
                    trs[i].style.display = "none";
                } else {
                    trs[i].style.display = trs[i]["data-display"];
                }
            }
        } else {
            trs[i].style.display = trs[i]["data-display"];
        }
    }

    // page button maker function
    var li;
    var make_button = function(symbol, index, config, disabled, active) {
            var button = document.createElement("button");
            button.innerHTML = symbol;
            button.addEventListener("click",
                function(event) {
                    event.preventDefault();
                    if (this.disabled !== true) {
                        config.page = index;
                        paginator(config);
                    }
                    return false;
                },
                false);
            if (disabled) {
                button.disabled = true;
                button.className = "govuk-button govuk-button--disabled govuk-!-margin-right-1";
            } else {
                button.className = "govuk-button govuk-button--secondary govuk-!-margin-right-1";
                if (active) {
                    button.className = "govuk-button govuk-!-margin-right-1";
                }
            }
            return button;
        };

        var pageArray = generatePagination(config);
        var pageBox = document.createElement(config.box_mode == "list" ? "ul" : "div");
        
        var leftButton = make_button("< Previous", (page > 1 ? page - 1 : 1), config, (page == 1), false);
        pageBox.appendChild(leftButton);
        for (var i = 0 ; i < pageArray.length; i++) {
            if (pageArray[i] === '...') {
                li = make_button(pageArray[i], pageArray[i], config, true, false);
                pageBox.appendChild(li);
            } else {
                li = make_button(pageArray[i], pageArray[i], config, false, (page == pageArray[i]));
                pageBox.appendChild(li);
            }
        }

        var rightButton = make_button("Next >", (pages > page ? page + 1 : page), config, (page == pages), false);
        pageBox.appendChild(rightButton);

        if (box.childNodes.length) {
            while (box.childNodes.length > 1) {
                box.removeChild(box.childNodes[0]);
            }
            box.replaceChild(pageBox, box.childNodes[0]);
        } else {
            box.appendChild(pageBox);
        }

    // make rows per page selector
    if (!(typeof config.page_options === "boolean" && !config.page_options)) {
        if (typeof config.page_options === "undefined") {
            config.page_options = [
                { value: 5,  text: '5'   },
                { value: 10, text: '10'  },
                { value: 20, text: '20'  },
                { value: 50, text: '50'  },
                { value: 100,text: '100' },
                { value: 0,  text: 'All' }
            ];
        }
        var options = config.page_options;
        var select = document.createElement("select");
        for (var i=0;i<options.length;i++) {
            var o = document.createElement("option");
            o.value = options[i].value;
            o.text = options[i].text;
            select.appendChild(o);
        }
        select.value = rows_per_page;
        select.addEventListener("change", function () {
            config.rows_per_page = this.value;
            paginator(config);
        }, false);
        box.appendChild(select);
    }

    // status message
    var stat = document.createElement("span");
    stat.innerHTML = "On page " + page + " of " + pages
        + ", showing rows " + (((page-1)*rows_per_page)+1)
        + " to " + (trs.length<page*rows_per_page||rows_per_page==0?trs.length:page*rows_per_page)
        + " of " + trs.length;
    box.appendChild(stat);

    // hide pagination if disabled
    if (config.disable) {
        if (typeof box["data-display"] === "undefined") {
            box["data-display"] = box.style.display||"";
        }
        box.style.display = "none";
    } else {
        if (box.style.display == "none") {
            box.style.display = box["data-display"]||"";
        }
    }

    // run tail function
    if (typeof config.tail_call == "function") {
        config.tail_call(config);
    }

    return box;
}

function generatePagination(config) {
    const offset = 2;
    var current = config.page;
    var last = config.pages;
    const leftOffset = current - offset;
    const rightOffset = current + offset + 1;

    /**
     * Reduces a list into the page numbers desired in the pagination
     * @param {array} accumulator - Growing list of desired page numbers
     * @param {*} _ - Throwaway variable to ignore the current value in iteration
     * @param {*} idx - The index of the current iteration
     * @returns {array} The accumulating list of desired page numbers
     */
    function reduceToDesiredPageNumbers(accumulator, _, idx) {
        const currIdx = idx + 1;

        if (
            // Always include first page
            currIdx === 1
            // Always include last page
            || currIdx === last
            // Include if index is between the above defined offsets
            || (currIdx >= leftOffset && currIdx < rightOffset)) {
            return [
                ...accumulator,
                currIdx,
            ];
        }

        return accumulator;
    }

    /**
     * Transforms a list of desired pages and puts ellipsis in any gaps
     * @param {array} accumulator - The growing list of page numbers with ellipsis included
     * @param {number} currentPage - The current page in iteration
     * @param {number} currIdx - The current index
     * @param {array} src - The source array the function was called on
     */
    function transformToPagesWithEllipsis(accumulator, currentPage, currIdx, src) {
        const prev = src[currIdx - 1];

        // Ignore the first number, as we always want the first page
        // Include an ellipsis if there is a gap of more than one between numbers
        if (prev != null && currentPage - prev !== 1) {
            return [
                ...accumulator,
                '...',
                currentPage,
            ];
        }

        // If page does not meet above requirement, just add it to the list
        return [
            ...accumulator,
            currentPage,
        ];
    }

    const pageNumbers = Array(last)
        .fill()
        .reduce(reduceToDesiredPageNumbers, []);

    const pageNumbersWithEllipsis = pageNumbers.reduce(transformToPagesWithEllipsis, []);

    return pageNumbersWithEllipsis;
}
