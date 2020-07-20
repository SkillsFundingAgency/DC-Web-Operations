class providerSearchController {
    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        if (stateLabel) {
            stateLabel.textContent = `Status: ${state}`;
        }
    }
}

export default providerSearchController