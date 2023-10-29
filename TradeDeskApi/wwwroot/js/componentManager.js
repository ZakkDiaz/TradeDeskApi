class ComponentManager {
    constructor() {
        this.components = {};
    }

    register(name, componentFunc) {
        this.components[name] = componentFunc;
    }

    display(name, props) {
        if (this.components[name]) {
            var element = this.components[name](props);
            $('#app').html(element);
        } else {
            console.error(`Component ${name} not found`);
        }
    }
}

var componentManager = new ComponentManager();