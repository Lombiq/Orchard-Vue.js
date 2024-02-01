const alias = require('@rollup/plugin-alias');
const path = require('path');

module.exports = function configureRollupAlias(vueJsNodeModulesPath, isProduction, furtherRollupAliases) {
    const entries = {
        vue: path.resolve(vueJsNodeModulesPath, isProduction
            ? 'vue/dist/vue.common.prod.js'
            : 'vue/dist/vue.esm.browser.js'), // #spell-check-ignore-line
        ...furtherRollupAliases,
    };

    return alias({ entries });
};
