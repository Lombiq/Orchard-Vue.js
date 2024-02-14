import { createApp } from 'vue'

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach((element) => {
    const componentName = element.getAttribute('data-name');
    const viewModel = JSON.parse(element.getAttribute('data-model'));

    if (!Array.isArray(window.VueApplications[componentName])) window.VueApplications[componentName] = [];

    const app = createApp(import(componentName), viewModel);
    app.$appId = element.id;
    window.VueApplications[componentName].push(app);
    app.mount(element);
});
