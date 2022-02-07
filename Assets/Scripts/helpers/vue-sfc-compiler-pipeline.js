const buble = require('rollup-plugin-buble');
const alias = require('rollup-plugin-alias');
const commonjs = require('rollup-plugin-commonjs');
const replace = require('rollup-plugin-replace');
const json = require('rollup-plugin-json');
const nodeResolve = require('rollup-plugin-node-resolve');
const path = require('path');
const log = require('fancy-log');
const del = require('del');

const { getVueComponents } = require('./get-vue-files');
const rollupPipeline = require('./rollup-pipeline');
const vuePlugin = require('./rollup-plugin-vue-sfc-orchard-core')

const defaultOptions = {
    rootPath: './Assets/Scripts/VueComponents/',
    destinationPath: './wwwroot/vue/',
    vueJsNodeModulesPath: '../Lombiq.VueJs/node_modules',
    rollupAlias: {},
    isProduction: false,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    const components = getVueComponents(opts.rootPath);

    log('vue component files: ' + components.join(', '));

    return rollupPipeline(
        opts.destinationPath,
        components.map((appName) => ({ fileName: appName, entryPath: path.join(opts.rootPath, appName), })),
        [
            vuePlugin(),
            json(),
            nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }),
            alias({
                vue: path.resolve(path.join(opts.vueJsNodeModulesPath, opts.isProduction
                    ? 'vue/dist/vue.common.prod.js'
                    : 'vue/dist/vue.esm.browser.js')),
                vuelidate: path.resolve(path.join(opts.vueJsNodeModulesPath, 'vuelidate/')),
                'vue-router': path.resolve(path.join(
                    opts.vueJsNodeModulesPath, 'vue-router/dist/vue-router.common.js')),
                'vue-axios': path.resolve(path.join(opts.vueJsNodeModulesPath, 'vue-axios/')),
                axios: path.resolve(path.join(opts.vueJsNodeModulesPath, 'axios/')),
                resolve: ['.vue', '.js', '/index.js', '/lib/index.js', '/src/index.js'],
                ...opts.rollupAlias,
            }),
            replace({
                'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                'process.env.BUILD': JSON.stringify('web'),
            }),
            commonjs(),
            buble(),
        ],
        null,
        (fileName) => fileName.replace(/\.vue$/i, ''));
}

function clean(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;
    return del(opts.destinationPath + '**/*.js');
}

module.exports = { compile, clean };
