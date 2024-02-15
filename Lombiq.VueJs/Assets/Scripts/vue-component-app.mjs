import { createApp } from 'vue'

window.VueApplications = window.VueApplications ?? { };

document.querySelectorAll('.lombiq-vue').forEach(async function initializeVueComponentApp(element) {
    const { componentName, viewModel, modelProperties } = JSON.parse(element.getAttribute('data-vue'));
    const component = (await import('vue-component-' + componentName)).default;

    const vModel = typeof viewModel === 'object'
        ? modelProperties
            .filter(property => property in viewModel)
            .map(property => ` @update:${property}="newValue => viewModel.${property} = newValue"`)
        : [];

    createApp({
        data: function data() {
            return { viewModel };
        },
        components: { [componentName]: component },
        template: `<${componentName} v-bind="viewModel"${vModel.join('')} />`,
        mounted: function mounted() {
            window.VueApplications[element.id] = this;
            this.$appId = element.id;
        },
    }).mount(element);
});
