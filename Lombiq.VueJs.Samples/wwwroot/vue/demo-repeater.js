'use strict';

window.Vue.component('demo-repeater', { name: 'demo-repeater', template: document.querySelector('.VueComponent-DemoRepeater').innerHTML, /* eslint-disable-line */ /* eslint-disable linebreak-style */
    props: {
        data: {
            type: Array,
            required: true,
        },
    },
});
