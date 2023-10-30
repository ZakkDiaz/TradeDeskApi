
function drawLineSegment({ svg, lastX, lastY, x, y, tradeType }) {
    const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
    line.setAttribute("x1", lastX);
    line.setAttribute("y1", lastY);
    line.setAttribute("x2", x);
    line.setAttribute("y2", y);
    line.setAttribute("stroke", tradeType === 1 ? "green" : "red");
    svg.appendChild(line);
}

function drawVolumeBar({ svg, x, volumeHeight, tradeType }) {
    const rect = document.createElementNS("http://www.w3.org/2000/svg", "rect");
    rect.setAttribute("x", x);
    rect.setAttribute("y", 0);
    rect.setAttribute("width", 2);
    rect.setAttribute("height", volumeHeight);
    rect.setAttribute("fill", tradeType === 1 ? "green" : "red");
    svg.appendChild(rect);
}
let calculateEMA = makeEMACalculator();
let calculateRSI = makeRSICalculator();
let lastY_RSI = null;
function drawRSI({ svg, lastX, x, rsi }) {
    const y = window.innerHeight - (rsi / 100) * window.innerHeight;
    const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
    line.setAttribute("x1", lastX);
    line.setAttribute("y1", lastY_RSI);
    line.setAttribute("x2", x);
    line.setAttribute("y2", y);
    line.setAttribute("stroke", "orange");
    line.setAttribute("stroke-width", "1");
    line.setAttribute("stroke-opacity", "0.7");
    svg.appendChild(line);
    lastY_RSI = y;
    return y;  // Return the current y-coordinate for the next segment
}

let lastY_EMA = null;
function drawEMA({ svg, lastX, x, ema, minPrice, maxPrice }) {
    const y = window.innerHeight - ((parseFloat(ema) - minPrice) / (maxPrice - minPrice) * window.innerHeight);
    const line = document.createElementNS("http://www.w3.org/2000/svg", "line");
    line.setAttribute("x1", lastX);
    line.setAttribute("y1", lastY_EMA);
    line.setAttribute("x2", x);
    line.setAttribute("y2", y);
    line.setAttribute("stroke", "blue");
    line.setAttribute("stroke-width", "1");
    line.setAttribute("stroke-opacity", "0.7");
    svg.appendChild(line);
    lastY_EMA = y;
    return y;  // Return the current y-coordinate for the next segment
}