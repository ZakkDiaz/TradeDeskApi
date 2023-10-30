function populateCustomDropdown(trackedSymbols, watchedSymbols) {
    const dropdownOptions = document.getElementById('dropdownOptions');
    dropdownOptions.innerHTML = ''; // Clear existing options

    trackedSymbols.forEach((symbolObj) => {
        const optionDiv = document.createElement('div');
        optionDiv.className = 'option';

        const symbolText = document.createElement('span');
        symbolText.innerText = symbolObj.symbol;
        optionDiv.appendChild(symbolText);

        // Add onclick event to fetch latest trades when the symbol is clicked
        symbolText.onclick = function () {
            router.setState({ watchedSymbol: symbolObj.symbol });
            fetchLatestTrades(symbolObj.symbol);
        };

        const watchButton = document.createElement('button');
        watchButton.innerText = watchedSymbols.some(w => w.trackedSymbolId === symbolObj.id) ? '-' : '+';
        watchButton.onclick = function () {
            toggleWatch(symbolObj.id);
        };
        optionDiv.appendChild(watchButton);

        dropdownOptions.appendChild(optionDiv);
    });
}
function toggleWatch(trackedSymbolId) {
    // Determine whether to add or remove the watch based on current state
    const isCurrentlyWatched = router.state.watchedSymbols.some(w => w.trackedSymbolId === trackedSymbolId);

    // Make AJAX call to add or remove the watch
    const apiEndpoint = isCurrentlyWatched ? '/api/tradebroker/removeWatch' : '/api/tradebroker/addWatch';

    $.ajax({
        url: apiEndpoint,
        method: 'POST',
        contentType: 'application/json',  // Setting Content-Type to application/json
        data: JSON.stringify({ UserProfileId: router.state.userProfile.id, TrackedSymbolId: trackedSymbolId }),  // Make sure to stringify the data
        success: function () {
            // Refresh the dropdown to reflect the change
            fetchAndPopulateSymbols();
        },
        error: function () {
            console.error('Failed to toggle watch');
        }
    });
}

function fetchLatestTrades(symbol) {
    $.get(`/api/data/latestTrades/${symbol}`)
        .done(function (response) {
            // Update the router state with the fetched data
            router.setState({
                latestTrades: response
            });
            // Navigate to the tradeChart component to display the data
            router.navigate('tradeChart');
        })
        .fail(function () {
            console.error(`Failed to fetch latest trades for ${symbol}`);
        });
}

function fetchTrackedSymbols() {
    return $.get("/api/TradeBroker/GetSymbols");
}

function fetchWatchedSymbols() {
    return $.get("/api/TradeBroker/GetWatchedSymbols");
}
// Modify fetchAndPopulateSymbols to call populateCustomDropdown
function fetchAndPopulateSymbols() {
    $.when(fetchTrackedSymbols(), fetchWatchedSymbols())
        .done(function (trackedSymbolsResponse, watchedSymbolsResponse) {
            const trackedSymbols = trackedSymbolsResponse[0];
            const watchedSymbols = watchedSymbolsResponse[0];

            // Update router state
            router.setState({
                trackedSymbols,
                watchedSymbols
            });

            // Call populateCustomDropdown to update the UI
            populateCustomDropdown(trackedSymbols, watchedSymbols);
        })
        .fail(function () {
            console.error('Failed to fetch symbols');
        });
}

// Modify your component registration to create a custom dropdown container
function createCustomDropdownContainer() {
    const container = document.createElement('div');
    container.id = 'dropdownOptions';
    return container;
}

// Register the component and its initializer in the router
router.register('symbols', createCustomDropdownContainer, fetchAndPopulateSymbols);