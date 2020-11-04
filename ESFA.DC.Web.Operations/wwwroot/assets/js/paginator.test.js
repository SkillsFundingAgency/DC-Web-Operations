import { paginator } from './paginator.js';

describe('Paginator arguments', () => {

    test('throws error if no config object supplied', () => {
        expect(() => { paginator() }).toThrow('Paginator was expecting a config object!');
    });

    test('throws error if null config object supplied', () => {
        expect(() => { paginator(null) }).toThrow('Paginator was expecting a config object!');
    });

    test('throws error if simple type config object supplied', () => {
        expect(() => { paginator(1) }).toThrow('Paginator was expecting a config object!');
    });

    test('throws error if no getRows is not function', () => {
        expect(() => { paginator({ get_rows: 'not a function'}) }).toThrow('Paginator was expecting a table or get_row function!');
    });

    test('throws error if page_change_event is supplied but is not a function', () => {
        expect(() => { paginator({ get_rows: () => {}, page_change_event: 'not a function' }) }).toThrow('Paginator was expecting page_change_event to be a function!');
    });
});

describe('Paginator display', () => {
    document.body.innerHTML = `<table id='test-table'>
                                    <tr><td>1</td></tr>
                                    <tr><td>2</td></tr>
                                    <tr><td>3</td></tr>
                                    <tr><td>4</td></tr>
                                    <tr><td>5</td></tr>
                                    <tr><td>6</td></tr>
                                    <tr><td>7</td></tr>
                                    <tr><td>8</td></tr>
                                    <tr><td>9</td></tr>
                                    <tr><td>10</td></tr>
                                </table>
                                <div id="index_native" class="box"></div>`;

    test('hides items not on first page', () => {
        paginator({
            table: document.getElementById("test-table"),
            box: document.getElementById("index_native"),
            page: 1,
            rows_per_page: 5,
            page_options: false,
        });

        const hiddenRows = document.querySelectorAll('tr[style="display: none;"]');
        expect(hiddenRows.length).toBe(5);
        expect(hiddenRows[0].innerHTML).toBe("<td>6</td>");
    });

    test('shows all items when if on single page', () => {
        paginator({
            table: document.getElementById("test-table"),
            box: document.getElementById("index_native"),
            page: 1,
            rows_per_page: 10,
            page_options: false,
        });

        expect(document.querySelectorAll('tr[style="display: none;"]').length).toBe(0);
    });

    test('defaults to correct page', () => {
        paginator({
            table: document.getElementById("test-table"),
            box: document.getElementById("index_native"),
            page: 2,
            rows_per_page: 5,
            page_options: false,
        });

        expect(document.body.innerHTML).toContain('On page 2 of 2, showing rows 6 to 10 of 10');
    });
});

