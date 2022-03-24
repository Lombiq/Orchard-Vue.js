<template>
    <a :href="prefix + id">
        {{ title }}
        <slot></slot>
    </a>
</template>

<script>
export default {
    props: {
        prefix: { default: window.Vue.$orchardCore.contentItemDisplay.prefix },
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
    }
}
</script>
