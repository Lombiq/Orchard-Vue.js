'use strict';

window.Vue.component('enhancement-control', { name: 'enhancement-control', template: document.querySelector('.VueComponent-EnhancementControl').innerHTML, /* eslint-disable-line */ /* eslint-disable linebreak-style */
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
});
