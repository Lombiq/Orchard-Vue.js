<template>
    <div v-if="pageCount > 0" class="vuePagination">
        <div class="vuePagination__item vuePagination__item_first"
             :class="{ vuePagination__item_disabled: modelValue === 1 }"
             @click="modelValue > 1 && $emit('update:model-value', 1)">
            <i class="fas fa-chevron-left"></i>
            <i class="fas fa-chevron-left"></i>
        </div>
        <div class="vuePagination__item vuePagination__item_prev"
             :class="{ vuePagination__item_disabled: modelValue <= 1 }"
             @click="modelValue > 1 && $emit('update:model-value', modelValue - 1)">
            <i class="fas fa-chevron-left"></i>
        </div>
        <div v-for="index in indexes"
             :key="'vuePagination__item_' + index"
             class="vuePagination__item"
             :class="{ vuePagination__item_current: index === modelValue }"
             @click="index !== modelValue && $emit('update:model-value', index )">
            {{ index }}
        </div>
        <div class="vuePagination__item vuePagination__item_next"
             :class="{ vuePagination__item_disabled: modelValue >= pageCount }"
             @click="modelValue < pageCount && $emit('update:model-value', modelValue + 1)">
            <i class="fas fa-chevron-right"></i>
        </div>
        <div v-if="pageCount !== Number.POSITIVE_INFINITY"
             class="vuePagination__item vuePagination__item_last"
             :class="{ vuePagination__item_disabled: modelValue === pageCount }"
             @click="modelValue < pageCount && $emit('update:model-value', pageCount )">
            <i class="fas fa-chevron-right"></i>
            <i class="fas fa-chevron-right"></i>
        </div>
    </div>
</template>

<script>
export default {
    emits: ['update:model-value'],
    props: {
        modelValue: {
            type: Number,
            required: true,
        },
        pageCount: {
            type: Number,
            default: Number.POSITIVE_INFINITY,
        },
    },
    computed: {
        indexes(self) {
            return Array.from({ length: 5 })
                .map((_, index) => index - 2 + self.modelValue)
                .filter((index) => index > 0 && index <= self.pageCount);
        },
    },
};
</script>
