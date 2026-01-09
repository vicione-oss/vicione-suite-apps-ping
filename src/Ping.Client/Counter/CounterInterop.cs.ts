class Counter {
    public showModule(name: string) {
        alert(`Selected: ${name}!`); // eslint-disable-line no-alert
    }
}

export function init() {
    return new Counter();
}
