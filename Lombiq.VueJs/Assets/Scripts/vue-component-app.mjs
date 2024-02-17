import { createApp } from 'vue';

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach(async function initializeVueComponentApp(element) {
    const { name, model, modelProperties } = JSON.parse(element.dataset.vue);
    const component = (await import('vue-component-' + name)).default;

    const vModel = typeof model === 'object'
        ? modelProperties
            .filter(property => property in model)
            .map(property => ` @update:${property}="viewModel.${property} = $event"`)
        : [];

    createApp({
        data: function data() {
            return { viewModel: model };
        },
        components: { [name]: component },
        template: `<${name} v-bind="viewModel" ${vModel.join('')} ref="main" />`,
        mounted: function mounted() {
            window.VueApplications[element.id] = this;
            this.$appId = element.id;
        },
    }).mount(element);
});
