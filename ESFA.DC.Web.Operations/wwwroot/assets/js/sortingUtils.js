export function sortByUkprn(a, b) {
    return a.ukprn - b.ukprn;
}

export function sortByProviderName(a, b) {
    const nameA = a.providerName.toUpperCase();
    const nameB = b.providerName.toUpperCase();
    if (nameA < nameB) {
        return -1;
    }
    if (nameA > nameB) {
        return 1;
    }
    return 0;
}