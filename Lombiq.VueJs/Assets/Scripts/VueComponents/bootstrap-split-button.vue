<template>
    <div class="btn-group bootstrapSplitButton">
        <button type="button"
                :class="`bootstrapSplitButton__main btn btn-${type} ${buttonClasses}`"
                :disabled="options.length === 0"
                @click="mainAction">
            {{ text ? text : options[0].text }}
        </button>
        <button type="button"
                class="dropdown-toggle dropdown-toggle-split bootstrapSplitButton__dropdownButton btn"
                :class="`btn-${type} ${toggleClasses}`"
                :disabled="options.length === 0"
                :aria-expanded="open.toString()"
                @click="open = !open">
            <span class="visually-hidden">[[ Toggle Dropdown ]]</span>
        </button>
        <div v-if="open" class="w-100 h-100 fixed-bottom" style="z-index: auto" @click="open = false"></div>
        <ul class="bootstrapSplitButton__dropdownMenu dropdown-menu"
            :class="`${dropdownClasses} ${open ? 'show' : ''}`"
            :style="`margin-top: ${marginTop}px`">
            <li v-for="option in options" :key="option.text" class="bootstrapSplitButton__dropdownMenuItem">
                <a v-if="option.text && option.text.trim && option.text.trim().startsWith('---')">
                    <hr class="dropdown-divider">
                </a>
                <a v-else :href="option.url" class="dropdown-item">
                    {{ option.text }}
                </a>
            </li>
        </ul>
    </div>
</template>

<script>
export default {
    props: {
        type: { type: String, default: 'primary' },
        text: { default: () => null },
        options: { type: Array, required: true }, // { text: String, href: String }[]
        buttonClasses: { type: String, default: '' },
        toggleClasses: { type: String, default: '' },
        dropdownClasses: { type: String, default: '' },
    },
    data: () => ({
        open: false,
        marginTop: 0,
    }),
    methods: {
        mainAction() {
            window.location.href = this.options[0].url;
        },
    },
    watch: {
        open(value) {
            if (!value) return;
            const shadowOffset = this.type === 'transparent' ? 0 : 8;
            this.marginTop = shadowOffset +
                this.$el.querySelector('.dropdown-toggle-split').offsetHeight;
        },
    },
};
</script>
