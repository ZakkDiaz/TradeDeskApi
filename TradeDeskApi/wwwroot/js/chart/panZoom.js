function initializeD3Zoom() {
    const svg = d3.select("#chartContainer svg");
    const zoom = d3.zoom().on("zoom", zoomed);
    console.log('init', svg);
    function zoomed(event) {
        console.log('zoomed', event);
        svg.attr('transform', event.transform);
    }

    svg.call(zoom);
}