<template>
    <div class="d-flex justify-content-between">
        <!-- Use the event modifiers like ".prevent" when working with native events to cut down on
             event handling boilerplate -->
        <button type="button"
                class="btn btn-primary"
                :class="{ disabled: value <= 1 }"
                @click.prevent="$emit('input', value - 1)">
            [[ << ]]
        </button>
        <button v-for="page in max"
                :key="'pager-' + page"
                class="btn btn-primary"
                :class="{ disabled: page === value }"
                type="button"
                @click.prevent="$emit('input', page)">
            {{ page }}
        </button>
        <button type="button"
                class="btn btn-primary"
                :class="{ disabled: value >= max }"
                @click.prevent="$emit('input', value + 1)">
            [[ >> ]]
        </button>
    </div>
</template>

<script>
export default {
    props: {
        value: {
            type: Number,
            required: true,
        },
        max: {
            type: Number,
            required: true,
        },
    },
};

// NEXT STATION: Assets/Scripts/VueComponents/enhancement-table-body.vue
</script>
