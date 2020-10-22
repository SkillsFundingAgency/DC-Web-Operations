import { $onAll } from '/assets/js/util.js';

class SiteController {

    constructor() {

        if (typeof window.GOVUK === 'undefined') {
            document.body.className = document.body.className.replace('js-enabled', '');
        }

        this.registerEvents();
    }

    registerEvents() {
        $onAll(document.querySelectorAll('.navigation-back'), 'click', () => window.history.back());
        $onAll(document.querySelectorAll('.navigation-back-to-start'), 'click', () => window.location.replace("index"));
        $onAll(document.querySelectorAll('.navigation-back-to-email-home'), 'click', () => window.location.replace("/emaildistribution"));
    }
}

export const siteController = new SiteController();
