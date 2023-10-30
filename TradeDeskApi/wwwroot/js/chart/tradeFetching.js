let tradeFetchInterval; // Declare a variable to store the interval ID
function startFetchingTrades() {
    stopFetchingTrades(); // Clear any existing intervals
    tradeFetchInterval = setInterval(fetchAndMergeLatestTrades, 1000); // Start a new interval
}

function stopFetchingTrades() {
    if (tradeFetchInterval) {
        clearInterval(tradeFetchInterval);
        tradeFetchInterval = null;
    }
}
function fetchAndMergeLatestTrades() {
    $.get("/api/data/latestTrades/" + router.state.watchedSymbol)
        .done(function (newTrades) {
            let existingTrades = router.state.latestTrades || [];

            // Create a new map to merge both existing and new trades
            const tradeMap = new Map();
            [...existingTrades, ...newTrades].forEach(trade => {
                tradeMap.set(trade.id, trade);  // Use 'id' as the unique identifier
            });

            // Convert the map back to an array and sort it by dealTime
            const mergedAndSortedTrades = Array.from(tradeMap.values()).sort((a, b) => b.dealTime - a.dealTime);

            // Update the router state
            router.setState({ latestTrades: mergedAndSortedTrades });

            // Re-initialize the chart
            initTradeChart();
        })
        .fail(function () {
            console.error('Failed to fetch latest trades');
        });
}