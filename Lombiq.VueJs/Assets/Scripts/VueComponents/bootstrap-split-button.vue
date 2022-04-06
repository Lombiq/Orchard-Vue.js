<template>
    <div class="btn-group">
        <button type="button"
                :class="buttonClass"
                @click="location.href='options[0].url'">
            {{ text ? text : options[0].text }}
        </button>
        <button type="button"
                class="dropdown-toggle dropdown-toggle-split"
                :class="buttonClass"
                data-bs-toggle="dropdown"
                aria-expanded="false">
            <span class="visually-hidden">[[ Toggle Dropdown ]]</span>
        </button>
        <ul class="dropdown-menu">
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
        text: { default: null, },
        options: { type: Array, required: true }, // { text: String, href: String }[]
    },
    computed: {
        buttonClass(self) { return 'btn btn-' + self.type; },
    }
}
</script>
