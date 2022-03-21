'use strict';

var DemoRepeater = { name: 'demo-repeater', template: document.querySelector('.VueComponent-DemoRepeater').innerHTML, /* eslint-disable-line */ /* eslint-disable linebreak-style */
    props: {
        data: {
            type: Array,
            required: true,
        },
    },
};

// You can include other components and JS files. The export must be ESM style like below

window.Vue.component('demo-sfc', { name: 'demo-sfc', template: document.querySelector('.VueComponent-DemoSfc').innerHTML, /* eslint-disable-line */ /* eslint-disable linebreak-style */
    // added during compilation.
    // name: "demo-sfc",

    // Here we declare a required and an optional property, both are validated to be numeric. The
    // "value" here is a special property name, along with the "input" event name. They together
    // make up the input and output part of the "v-model" directive you saw in the cshtml.
    props: {
        value: {
            type: Number,
            required: true,
        },
        max: {
            type: null,
            default: 10,
        },
    },

    // Computed properties are like getters, but they are cached and reactive. For example this
    // one is only re-evaluated if self.value changes which triggers an update.
    computed: {
        repeaterData(self) {
            return Array.from({ length: self.value })
                .map((_, i) => `DemoRepeater__listItem_${i + 1}`);
        },
    },

    // You can include multiple child components. During build the exported SFC is registered as
    // a global component, but its children remain private, composed using the components property.
    components: {
        DemoRepeater,
    },
});
