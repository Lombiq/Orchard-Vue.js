import { createApp } from 'vue';

window.VueApplications = window.VueApplications ?? { };

function toKebabCase(camelCase) {
    return Array
        .from(camelCase)
        .map((letter) => { const lowerCase = letter.toLowerCase(); return letter === lowerCase ? letter : '-' + lowerCase })
        .join('');
}

document.querySelectorAll('.lombiq-vue').forEach(async function initializeVueComponentApp(element) {
    const { name, model } = JSON.parse(element.dataset.vue);
    const component = (await import(name + '.vue')).default;

    const plugins = await Promise.all(element
        .dataset
        .plugins
        .split(',')
        .map(async word => (await import(word.trim())).default));

    const hasEmit = Array.isArray(component?.emit);
    const vModel = Object
        .keys(model)
        .map(property => ({ property: property, eventName: 'update:' + toKebabCase(property) }))
        .filter(pair => !hasEmit || component.emit.includes(pair.eventName))
        .map(pair => ` @${pair.eventName}="viewModel.${pair.property} = $event"`);

    const app = createApp({
        data: function data() {
            return { viewModel: model, root: element };
        },
        components: { [name]: component },
        template: `<${name} v-bind="viewModel" ${vModel.join('')} ref="main" />`,
        mounted: function mounted() {
            window.VueApplications[element.id] = this;
            this.$appId = element.id;
        },
    })

    plugins.forEach((plugin) => app.use(plugin));

    app.mount(element);
});
