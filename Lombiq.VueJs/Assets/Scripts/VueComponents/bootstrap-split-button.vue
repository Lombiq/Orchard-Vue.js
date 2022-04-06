<template>
    <div class="btn-group">
        <button type="button" :class="buttonClass" @click="mainAction">
            {{ text ? text : options[0].text }}
        </button>
        <button type="button"
                class="dropdown-toggle dropdown-toggle-split"
                :class="buttonClass"
                :aria-expanded="open.toString()"
                @click="open = !open">
            <span class="visually-hidden">[[ Toggle Dropdown ]]</span>
        </button>
        <ul class="dropdown-menu" :class="{ show: open }" :style="`margin-top: ${marginTop}px`">
            <li v-for="option in options" :key="option.text">
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
    },
    data: () => ({
        open: false,
        marginTop: 0,
    }),
    computed: {
        buttonClass(self) { return 'btn btn-' + self.type; },
    },
    methods: {
        mainAction() {
            window.location.href = this.options[0].url;
        },
    },
    watch: {
        open(value) {
            if (value) this.marginTop = this.$el.querySelector('.dropdown-toggle-split').offsetHeight;
        },
    },
};
</script>
