const renderingFunctions = [
    drawEMA,
    drawRSI,
    drawLineSegment,
    drawVolumeBar,
];

function initTradeChart() {
    startFetchingTrades();  // Start fetching trades when the chart is initialized
    // Create a MutationObserver instance to watch for changes in the parent element
    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            Array.from(mutation.removedNodes).forEach((node) => {
                if (node.id === 'chartContainer') {
                    // Stop fetching trades if the chartContainer is removed
                    stopFetchingTrades();
                    observer.disconnect();
                }
            });
        });
    });

    // Observe changes to the parent element of chartContainer
    const parentElement = document.getElementById('chartContainer').parentElement;
    observer.observe(parentElement, { childList: true });
    const svg = document.querySelector('#chartContainer svg');
    svg.innerHTML = '';  // Clear existing SVG elements
    let data = router.state.latestTrades || [];
    const maxPrice = Math.max(...data.map(d => parseFloat(d.price)));
    const minPrice = Math.min(...data.map(d => parseFloat(d.price)));
    const maxTime = Math.max(...data.map(d => d.dealTime));
    const minTime = Math.min(...data.map(d => d.dealTime));
    const timeRange = maxTime - minTime;

    let lastX, lastY;
    let firstPoint = true;
    calculateEMA.reset();
    calculateRSI.reset();
    const maxVolume = Math.max(...data.map(d => parseFloat(d.quantity)));
    data = data.reverse();
    data.forEach((trade, _i) => {
        const x = ((trade.dealTime - minTime) / timeRange) * window.innerWidth;
        const y = window.innerHeight - ((parseFloat(trade.price) - minPrice) / (maxPrice - minPrice) * window.innerHeight);
        const volumeHeight = (parseFloat(trade.quantity) / maxVolume) * 100;
        
        const params = {
            svg,
            lastX,
            lastY,
            x,
            y,
            tradeType: trade.tradeType,
            volumeHeight,
            ema: calculateEMA.calculate(parseFloat(trade.price)),
            rsi: _i > 50 ? calculateRSI.calculate(parseFloat(trade.price)) : null,
            maxPrice,
            minPrice
        };

        if (!firstPoint && !isNaN(lastY) && !isNaN(y)) {
            renderingFunctions.forEach((func) => {
                func(params);
            });
        }

        lastX = x;
        lastY = y;
        firstPoint = false;
    });
}

router.setState({ latestTrades: [] });
router.register('tradeChart', createTradeChart, initTradeChart);