import { createApp } from 'vue'

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach((element) => {
    const componentName = element.getAttribute('data-name');
    const data = JSON.parse(element.getAttribute('data-model'));

    if (!Array.isArray(window.VueApplications[componentName])) window.VueApplications[componentName] = [];

    const app = createApp({
        el: element,
        data: data,
    });
    app.$appId = element.id;
    window.VueApplications[componentName].push(app);
});
