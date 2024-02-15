import { createApp } from 'vue'

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach(async function initializeVueComponentApp(element) {
    const { componentName, viewModel, modelProperty } = JSON.parse(element.getAttribute('data-vue'));
    const component = (await import('vue-component-' + componentName)).default;

    const vModel = typeof viewModel === 'object' && modelProperty in viewModel
        ? `v-model="viewModel.${modelProperty}"`
        : '';

    createApp({
        data: function data() {
            return { viewModel };
        },
        components: { [componentName]: component },
        template: `<${componentName} v-bind="viewModel"${vModel} />`,
        mounted: function mounted() {
            window.VueApplications[element.id] = this;
            this.$appId = element.id;
        },
    }).mount(element);
});
