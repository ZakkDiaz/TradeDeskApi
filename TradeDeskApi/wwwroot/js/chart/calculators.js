function makeEMACalculator(period = 600) {
    let lastEMA = 0;
    const multiplier = 2 / (period + 1);

    const calculate = (price) => {
        lastEMA = lastEMA ? ((price - lastEMA) * multiplier) + lastEMA : price;
        return lastEMA;
    };

    const reset = () => {
        lastEMA = 0;
    };

    return {
        calculate,
        reset
    };
}

function makeRSICalculator(period = 600) {
    let gains = [];
    let losses = [];
    let lastPrice = 0;

    const calculate = (price) => {
        let currentGain = 0;
        let currentLoss = 0;
        const difference = price - lastPrice;
        if (difference > 0) currentGain = difference;
        else currentLoss = -difference;

        gains.push(currentGain);
        losses.push(currentLoss);

        if (gains.length > period) {
            gains.shift();
            losses.shift();
        }

        const avgGain = gains.reduce((acc, val) => acc + val, 0) / period;
        const avgLoss = losses.reduce((acc, val) => acc + val, 0) / period;

        const rs = avgGain / avgLoss;
        lastPrice = price;

        return 100 - (100 / (1 + rs));
    };

    const reset = () => {
        gains = [];
        losses = [];
        lastPrice = 0;
    };

    return {
        calculate,
        reset
    };
}