<template>
    <!-- This is the template of the component. If you have IDE plugin for Vue.js, this format
         affords you first-class Vue.js coding support. -->
    <div class="DemoSfc__wrapper">
        <select class="DemoSfc__select" @change="$emit('input', parseInt($event.target.value))">
            <!-- Note the bound "key" property - it must be unique and you should always include
                 it for performance reasons. Also the "value" binding is not strictly necessary
                 but can be useful for communicating value with external libraries, e.g. with
                 other JS scripts or during UI testing. -->
            <option v-for="n in max"
                    :key="'demo-sfc-' + n"
                    :selected="value === n"
                    :value="n">
                {{ n }}
            </option>
        </select>
        <p>[[ Does localization escape HTML? <span class="not-html" hidden>YES!</span> ]]</p>
        <!-- Here you can see a non-standard expression. The "[[ ... ]]" is unique to this OC
             module: it performs localization via IStringLocalizer at runtime. -->
        <demo-repeater :data="repeaterData">[[ Hello! ]]</demo-repeater>
    </div>
</template>

<script>
// You can include other components and JS files. The export must be ESM style like below
// (export default { ... }) so the imports should be too. When you import an SFC make sure to
// include the .vue file extension to reliably tell it apart from plain JS modules.
import DemoRepeater from './demo-repeater.vue';

export default {
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
};

// NEXT STATION: Assets/Scripts/VueComponents/demo-repeater.vue
</script>

<style scoped>
  /* The style tag is completely ignored. Please keep using themes as usual. */
</style>
