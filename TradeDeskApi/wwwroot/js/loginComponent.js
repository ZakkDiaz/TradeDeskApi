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

function attachLoginEvents() {
    $("#submitLogin").click(function () {
        const apiKey = $("#apiKey").val();
        if (apiKey) {
            $.ajaxSetup({
                headers: { 'x-api-key': apiKey }
            });
            router.navigate('dashboard');
        }
    });
}

// Register the route
router.register('login', createLoginForm, attachLoginEvents);