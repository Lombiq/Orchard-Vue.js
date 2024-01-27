import * as Vue from 'https://unpkg.com/vue@3.4.15/dist/vue.esm-browser.js'

window.VueApplications = window.VueApplications ?? { };
if (!('applications' in Vue)) Vue.applications = window.VueApplications;

document.querySelectorAll('.lombiq-vue').forEach((element) => {
    const componentName = element.getAttribute('data-name');
    const data = JSON.parse(element.getAttribute('data-model'));

    if (!Array.isArray(window.VueApplications[componentName])) window.VueApplications[componentName] = [ ];

    const app = new Vue({
        el: element,
        data: data,
    });
    app.$appId = element.id;
    window.VueApplications[componentName].push(app);
});
