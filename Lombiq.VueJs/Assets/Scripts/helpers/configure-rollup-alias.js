const alias = require('@rollup/plugin-alias');
const path = require('path');

module.exports = function configureRollupAlias(vueJsNodeModulesPath, isProduction, furtherRollupAliases) {
    const entries = {
        vue: path.resolve(vueJsNodeModulesPath, isProduction
            ? 'vue/dist/vue.common.prod.js'
            : 'vue/dist/vue.esm.browser.js'), // #spell-check-ignore-line
        vuelidate: path.resolve(path.join(vueJsNodeModulesPath, 'vuelidate/')), // #spell-check-ignore-line
        'vue-router': path.resolve(path.join(
            vueJsNodeModulesPath, 'vue-router/dist/vue-router.common.js')),
        'vue-axios': path.resolve(path.join(vueJsNodeModulesPath, 'vue-axios/')),
        axios: path.resolve(path.join(vueJsNodeModulesPath, 'axios/')),
        ...furtherRollupAliases,
    };

    return alias({ entries });
};
