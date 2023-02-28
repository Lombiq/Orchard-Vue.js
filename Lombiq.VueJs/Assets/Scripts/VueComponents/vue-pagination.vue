<template>
    <div v-if="pageCount > 0" class="vuePagination">
        <div class="vuePagination__item vuePagination__item_first"
             :class="{ vuePagination__item_disabled: page === 0 }"
             @click="page > 0 && $emit('change', 0)">
            <i class="fas fa-chevron-left"></i>
            <i class="fas fa-chevron-left"></i>
        </div>
        <div class="vuePagination__item vuePagination__item_prev"
             :class="{ vuePagination__item_disabled: page <= 0 }"
             @click="page > 0 && $emit('change', page - 1)">
            <i class="fas fa-chevron-left"></i>
        </div>
        <div v-for="index in indexes"
             :key="'vuePagination__item_' + index"
             class="vuePagination__item"
             :class="{ vuePagination__item_current: index === page }"
             @click="index !== page && $emit('change', index)">
            {{ index + 1 }}
        </div>
        <div class="vuePagination__item vuePagination__item_next"
             :class="{ vuePagination__item_disabled: page + 1 >= pageCount }"
             @click="page + 1 < pageCount && $emit('change', page + 1)">
            <i class="fas fa-chevron-right"></i>
        </div>
        <div v-if="pageCount !== Number.POSITIVE_INFINITY"
             class="vuePagination__item vuePagination__item_last"
             :class="{ vuePagination__item_disabled: page + 1 === pageCount }"
             @click="page + 1 < pageCount && $emit('change', pageCount - 1)">
            <i class="fas fa-chevron-right"></i>
            <i class="fas fa-chevron-right"></i>
        </div>
    </div>
</template>

<script>
export default {
    model: {
        prop: 'page',
        event: 'change',
    },
    props: {
        page: {
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
                .map((_, index) => index - 2 + self.page)
                .filter((index) => index >= 0 && index < self.pageCount);
        },
    },
};
</script>
