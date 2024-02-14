import { createApp } from 'vue'

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach(async function initializeVueComponentApp(element) {
    const componentName = element.getAttribute('data-name');
    const viewModel = JSON.parse(element.getAttribute('data-model'));
    const component = (await import('vue-component-' + componentName)).default;

    if (!Array.isArray(window.VueApplications[componentName])) window.VueApplications[componentName] = [];

    const app = createApp(component, viewModel);
    app.$appId = element.id;
    window.VueApplications[componentName].push(app);
    app.mount(element);
});
