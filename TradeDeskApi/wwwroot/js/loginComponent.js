function createLoginForm() {
    var form = document.createElement('div');
    form.className = 'login-form';

    var title = document.createElement('h1');
    title.innerText = 'Login';
    form.appendChild(title);

    var label = document.createElement('label');
    label.setAttribute('for', 'apiKey');
    label.innerText = 'API Key:';
    form.appendChild(label);

    var input = document.createElement('input');
    input.type = 'text';
    input.id = 'apiKey';
    form.appendChild(input);

    var button = document.createElement('button');
    button.id = 'submitLogin';
    button.innerText = 'Submit';
    form.appendChild(button);

    return form;
}
function fetchUserProfile(apiKey) {
    return $.ajax({
        url: '/api/Home', // replace with your API endpoint to get user profile
        headers: { 'x-api-key': apiKey }
    });
}

function attachLoginEvents() {
    $("#submitLogin").click(function () {
        const apiKey = $("#apiKey").val();
        if (apiKey) {
            $.ajaxSetup({
                headers: { 'x-api-key': apiKey }
            });

            // Fetch user profile and store it in the router's state
            fetchUserProfile(apiKey).then(userProfile => {
                router.setState({
                    userProfile: userProfile
                });
                router.navigate('dashboard');
            }).catch(err => {
                console.error('Failed to fetch user profile:', err);
            });
        }
    });
}


// Register the route
router.register('login', createLoginForm, attachLoginEvents);