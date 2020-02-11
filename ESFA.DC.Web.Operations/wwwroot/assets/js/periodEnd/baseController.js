import { padLeft } from '/assets/js/util.js';

export const updateSync = function () {
    const classScope = this;

    let date = new Date();
    let day = padLeft(date.getDate(), "0", 2);
    let month = padLeft(date.getMonth() + 1, "0", 2);

    let hours = padLeft(date.getHours(), "0", 2);
    let minutes = padLeft(date.getMinutes(), "0", 2);
    let seconds = padLeft(date.getSeconds(), "0", 2);

    const dateLabel = document.getElementById("lastSync");
    dateLabel.textContent = `Last updated: ${day}/${month}/${date.getFullYear()} ${hours}:${minutes}:${seconds}`;

    const slowLabel = document.getElementById("slowConnection");
    slowLabel.textContent = "";

    clearInterval(classScope._slowTimer);
    classScope._slowTimer = setTimeout(function () { slowLabel.textContent = "💤" }, 10000);
}