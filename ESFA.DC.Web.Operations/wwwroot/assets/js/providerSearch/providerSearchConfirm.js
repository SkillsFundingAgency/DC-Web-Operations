function searchProviderOnConfirm(result) {
    this._url = '';
    const data = JSON.parse(document.getElementById('initialState').innerHTML);
    if (result.existsInSld) {
        this._url = data.manageProvidersUrl;
    } else {
        this._url = data.addNewProvidersUrl;
    }

    window.location.href = this._url.replace('__ukprn__', result.ukprn);
}