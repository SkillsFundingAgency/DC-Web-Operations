class providerSearchController {
    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }
}

export default providerSearchController