<template>
    <a :href="baseUrl + id">
        {{ title }}
        <slot></slot>
    </a>
</template>

<script>
export default {
    props: {
        baseUrl: { default: window.Vue.$orchardCore.contentItemDisplay.baseUrl },
        contentItem: { },
        contentItemId: { },
        displayText: { },
    },
    computed: {
        id(self) {
            return self.contentItem && self.contentItem.contentItemId
                ? self.contentItem.contentItemId
                : self.contentItemId;
        },
        title(self) {
            return self.contentItem && self.contentItem.displayText
                ? self.contentItem.displayText
                : self.displayText;
        },
    },
};
</script>
