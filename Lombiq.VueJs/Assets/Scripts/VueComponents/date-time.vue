<template>
    <time :datetime="serialized">{{ formatted }}</time>
</template>

<script>
export default {
    props: {
        date: { required: true },
        culture: { type: String, default: () => window.Vue.$orchardCore.dateTime.culture },
        timeZone: { type: String, default: () => window.Vue.$orchardCore.dateTime.timeZone },
        options: {
            // Defaults to "short date" format if this property is not passed.
            default: () => ({
                year: 'numeric',
                month: 'short',
                day: 'numeric',
            }),
        },
    },
    computed: {
        serialized(self) {
            return typeof self.date === 'string' ? self.date : JSON.parse(JSON.stringify(self.date));
        },
        formatted(self) {
            const formatter = new Intl.DateTimeFormat(
                self.culture || window.navigator.language,
                { ...self.options, timeZone: self.timeZone });

            const date = typeof self.date === 'string'
                ? new Date(self.date)
                : self.date;
            return formatter.format(date);
        },
    },
};
</script>
