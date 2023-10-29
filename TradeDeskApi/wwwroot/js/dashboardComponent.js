function createDashboard(wallet) {
    var dashboard = document.createElement('div');
    dashboard.className = 'dashboard';

    var title = document.createElement('h1');
    title.innerText = 'Wallet Dashboard';
    dashboard.appendChild(title);

    var info = document.createElement('div');
    info.className = 'wallet-info';
    dashboard.appendChild(info);

    var name = document.createElement('div');
    name.innerHTML = `<strong>Name:</strong> <span>${wallet.name}</span>`;
    info.appendChild(name);

    var funds = document.createElement('div');
    funds.innerHTML = `<strong>Total Funds:</strong> <span>${wallet.totalFunds}</span>`;
    info.appendChild(funds);

    var isTest = document.createElement('div');
    isTest.innerHTML = `<strong>Is Test:</strong> <span>${wallet.isTest ? 'Yes' : 'No'}</span>`;
    info.appendChild(isTest);

    // Add button if IsTest is true
    if (wallet.isTest) {
        var addFundsButton = document.createElement('button');
        addFundsButton.id = 'addFundsButton';  // Added this line
        addFundsButton.className = 'add-funds-button';
        addFundsButton.innerText = '+';
        isTest.appendChild(addFundsButton);
    }

    return dashboard;
}

function fetchWallet() {
    $.get("/api/TradeBroker/wallet", function (wallet) {
        // First, create the dashboard with the fetched wallet data
        const dashboard = createDashboard(wallet);

        // Then, update the HTML content
        $('#app').html(dashboard);

        // Attach event listeners for dynamic elements like the Add Funds button
        if (wallet.isTest) {
            $('#addFundsButton').click(function () {
                $.get(`/api/tradebroker/addfunds/100`, function (response) {
                    fetchWallet();  // Refresh the dashboard with updated wallet data
                }).fail(function () {
                    alert('Failed to add funds');
                });
            });
        }
    }).fail(function () {
        alert("Failed to fetch wallet. Please try again.");
    });
}

// Register the route
router.register('dashboard', () => { }, fetchWallet); // Note: We set componentFunc to null