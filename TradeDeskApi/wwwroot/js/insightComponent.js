function createInsightComponent(index) {
    const insight = document.createElement('div');
    insight.className = 'insight';

    const title = document.createElement('h1');
    title.innerText = `${index} Insights`;
    insight.appendChild(title);

    // Add a Home button to navigate back to the dashboard
    const homeButton = document.createElement('button');
    homeButton.id = 'homeButton';
    homeButton.innerText = 'Home';
    insight.appendChild(homeButton);

    // ... (you can add elements to display volume, spread, volatility, charts, etc.)

    return insight;
}

// Register the route
router.register('insight', createInsightComponent, null);