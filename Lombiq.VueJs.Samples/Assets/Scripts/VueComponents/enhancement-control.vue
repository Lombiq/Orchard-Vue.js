<template>
    <div class="d-flex justify-content-between">
        <!-- Use the event modifiers like ".prevent" when working with native events to cut down on
             event handling boilerplate -->
        <button type="button"
                class="btn btn-primary"
                :class="{ disabled: modelValue <= 1 }"
                @click.prevent="$emit('update:modelValue', modelValue - 1)">
            [[ << ]]
        </button>
        <button v-for="page in max"
                :key="'pager-' + page"
                class="btn btn-primary"
                :class="{ disabled: page === modelValue }"
                type="button"
                @click.prevent="$emit('update:modelValue', page)">
            {{ page }}
        </button>
        <button type="button"
                class="btn btn-primary"
                :class="{ disabled: modelValue >= max }"
                @click.prevent="$emit('update:modelValue', modelValue + 1)">
            [[ >> ]]
        </button>
    </div>
</template>

<script>
export default {
    props: {
        modelValue: {
            type: Number,
            required: true,
        },
        max: {
            type: Number,
            required: true,
        },
    },

    emits: [ 'update:model-modelValue' ],
};

// NEXT STATION: Assets/Scripts/VueComponents/enhancement-table-body.vue
</script>
