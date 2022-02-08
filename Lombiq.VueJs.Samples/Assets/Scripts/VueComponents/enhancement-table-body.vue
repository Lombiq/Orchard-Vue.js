<template>
    <tbody>
        <tr v-for="row in rows" :key="row.id">
            <td>{{ row.number }}</td>
            <td>{{ row.date }}</td>
            <td>{{ row.day }}</td>
            <td>{{ row.stars }}</td>
        </tr>
    </tbody>
</template>

<script>
const ten = Array.from({ length: 10 });

export default {
    props: {
        data: {
            type: Array,
            required: true,
        }
    },
    computed: {
        rows(self) {
            return self.data.map((row, rowIndex) => Object.assign(
                { },
                row,
                {
                    id: 'enhancement-table-body-row-' + rowIndex,
                    stars: ten
                        .map((_, index) => index < row.random ? '★' : '✰')
                        .join(''),
                }));
        },
    },
};
</script>
