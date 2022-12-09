const buble = require('rollup-plugin-buble');
const alias = require('rollup-plugin-alias');
const commonjs = require('rollup-plugin-commonjs');
const replace = require('rollup-plugin-replace');
const json = require('rollup-plugin-json');
const nodeResolve = require('rollup-plugin-node-resolve');
const fs = require('fs');
const path = require('path');
const log = require('fancy-log');

const rollupPipeline = require('./rollup-pipeline');
const { getVueApps } = require('./get-vue-files');

const defaultOptions = {
    rootPath: './Assets/Apps/',
    destinationPath: './wwwroot/apps/',
    vueJsNodeModulesPath: path.join(__dirname, '..', '..', '..', 'node_modules'),
    rollupAlias: {},
    isProduction: false,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    if (!fs.existsSync(opts.vueJsNodeModulesPath)) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" does not exist!`);
    }
    if (!fs.lstatSync(opts.vueJsNodeModulesPath).isDirectory()) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" is not a directory!`);
    }

    return rollupPipeline(
        opts.destinationPath,
        getVueApps(opts.rootPath)
            .map((appName) => ({ fileName: appName, entryPath: path.join(opts.rootPath, appName, '/main.js') })),
        [
            json(),
            alias({
                vue: path.resolve(path.join(opts.vueJsNodeModulesPath, opts.isProduction
                    ? 'vue/dist/vue.common.prod.js'
                    : 'vue/dist/vue.esm.browser.js')),
                vuelidate: path.resolve(path.join(opts.vueJsNodeModulesPath, 'vuelidate/')),
                'vue-router': path.resolve(path.join(
                    opts.vueJsNodeModulesPath, 'vue-router/dist/vue-router.common.js')),
                'vue-axios': path.resolve(path.join(opts.vueJsNodeModulesPath, 'vue-axios/')),
                axios: path.resolve(path.join(opts.vueJsNodeModulesPath, 'axios/')),
                resolve: ['.js', '/index.js', '/lib/index.js', '/src/index.js'],
                ...opts.rollupAlias,
            }),
            nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }), // #spell-check-ignore-line
            replace({
                'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                'process.env.BUILD': JSON.stringify('web'),
            }),
            commonjs(),
            buble(),
        ]
    );
}

module.exports = { compile };
