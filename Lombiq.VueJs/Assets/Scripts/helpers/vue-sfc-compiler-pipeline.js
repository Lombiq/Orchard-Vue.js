const alias = require('@rollup/plugin-alias');
const commonjs = require('@rollup/plugin-commonjs');
const del = require('del');
const fs = require('fs');
const json = require('@rollup/plugin-json');
const log = require('fancy-log');
const path = require('path');
const replace = require('@rollup/plugin-replace');
const { nodeResolve } = require('@rollup/plugin-node-resolve');

const { getVueComponents } = require('./get-vue-files');
const rollupPipeline = require('./rollup-pipeline');
const vuePlugin = require('./rollup-plugin-vue-sfc-orchard-core');

const { handleErrorMessage } = require('nodejs-extensions/scripts/handle-error');

const args = process.argv.splice(2);

const defaultOptions = {
    rootPath: path.resolve('..', '..', 'Assets', 'Scripts', 'VueComponents'),
    destinationPath: path.resolve('..', '..', 'wwwroot', 'vue'),
    vueJsNodeModulesPath: path.resolve(__dirname, '..', '..', '..', 'node_modules'),
    rollupAlias: {},
    isProduction: false,
};

async function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    const components = getVueComponents(opts.rootPath);

    log('vue component files: ' + components.join(', '));

    if (!fs.existsSync(opts.vueJsNodeModulesPath)) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" does not exist!`);
    }
    if (!fs.lstatSync(opts.vueJsNodeModulesPath).isDirectory()) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" is not a directory!`);
    }

    const results = await rollupPipeline(
        opts.destinationPath,
        components.map((appName) => ({ fileName: appName, entryPath: path.join(opts.rootPath, appName) })),
        [
            vuePlugin(),
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
                resolve: ['.vue', '.js', '/index.js', '/lib/index.js', '/src/index.js'],
                ...opts.rollupAlias,
            }),
            nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }),
            replace({
                values: {
                    'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                    'process.env.BUILD': JSON.stringify('web'),
                },
                preventAssignment: true,
            }),
            commonjs(),
        ],
        null,
        (fileName) => fileName.split('.')[0]);

    if (results.length > 0) {
        results.forEach(handleErrorMessage);
        process.exit(1);
    }
}

async function clean(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;
    await del(opts.destinationPath + '**/*.js');
    await del(opts.destinationPath + '**/*.js.map');
}

const consoleOptions = args.length >= 2 ? JSON.parse(args[1]) : undefined;
switch (args[0]) {
    case 'compile': compile(consoleOptions); break;
    case 'clean': clean(consoleOptions); break;
}

module.exports = { compile, clean };
