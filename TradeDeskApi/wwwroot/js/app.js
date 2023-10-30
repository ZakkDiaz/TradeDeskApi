$(document).ready(function () {
    // Navigate to the initial route
    router.navigate('login');


    // Attach event listener for index change
    $('#app').on('change', '#watchedIndexDropdown', function () {
        fetchWatchedIndexInfo();
    });

    // Attach event listener for Home button in insight component
    $('#app').on('click', '#homeButton', function () {
        router.navigate('dashboard');
    });

    $('#app').on('change', '#watchedIndexDropdown', function () {
        const newIndex = $(this).val();
        // Update router state
        //router.setState({
        //    watchedIndex: newIndex
        //});
        // Refresh header bar or make API call to get updated data
        $('#headerBar').text(`Info about ${newIndex}`);
    });
});