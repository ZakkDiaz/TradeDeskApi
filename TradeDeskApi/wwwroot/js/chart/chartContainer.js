function createTradeChart() {
    const chartContainer = document.createElement('div');
    chartContainer.id = 'chartContainer';

    // Create a top bar
    const topBar = document.createElement('div');
    topBar.className = 'top-bar';  // Use the class from your .css file

    // Create a button to navigate back to the dashboard
    const backButton = document.createElement('button');
    backButton.textContent = 'Back to Dashboard';
    backButton.addEventListener('click', () => {
        // Navigate back to the dashboard
        router.navigate('dashboard'); // Replace with your actual navigation logic
    });

    topBar.appendChild(backButton);
    chartContainer.appendChild(topBar);

    const svg = document.createElementNS("http://www.w3.org/2000/svg", "svg");
    chartContainer.appendChild(svg);

    // Call updateSVGDimensions here, after appending the SVG to the DOM
    updateSVGDimensions();

    // Listen for window resize events to update the SVG dimensions
    window.addEventListener('resize', updateSVGDimensions);

    return chartContainer;
}
function updateSVGDimensions() {
    const chartContainer = document.getElementById('chartContainer');
    if (chartContainer) {
        const svg = chartContainer.querySelector('svg');
        if (svg) {
            const rect = chartContainer.getBoundingClientRect();
            svg.setAttribute('width', rect.width);
            svg.setAttribute('height', rect.height);
        }
    }
}