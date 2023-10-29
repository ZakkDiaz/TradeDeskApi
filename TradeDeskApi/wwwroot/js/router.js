class Router {
    constructor() {
        this.routes = {};
    }

    register(name, componentFunc, initFunc) {
        this.routes[name] = { componentFunc, initFunc };
    }

    navigate(name, props) {
        if (this.routes[name]) {
            const { componentFunc, initFunc } = this.routes[name];
            const element = componentFunc(props);
            $('#app').html(element);
            if (initFunc) {
                initFunc();
            }
        } else {
            console.error(`Route ${name} not found`);
        }
    }
}

var router = new Router();