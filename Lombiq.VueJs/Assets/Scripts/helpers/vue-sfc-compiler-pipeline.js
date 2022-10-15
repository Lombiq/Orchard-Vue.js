const commonjs = require('@rollup/plugin-commonjs');
const del = require('del');
const fs = require('fs');
const json = require('@rollup/plugin-json');
const path = require('path');
const replace = require('@rollup/plugin-replace');
const { nodeResolve } = require('@rollup/plugin-node-resolve');

const { handleErrorMessage } = require('nodejs-extensions/scripts/handle-error');

const argsExecute = require('./args-execute');
const configureRollupAlias = require('./configure-rollup-alias');
const rollupPipeline = require('./rollup-pipeline');
const vuePlugin = require('./rollup-plugin-vue-sfc-orchard-core');
const { getVueComponents } = require('./get-vue-files');

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

    process.stdout.write(`vue component files: ${components.join(', ')}\n`);

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
            configureRollupAlias(opts.vueJsNodeModulesPath, opts.isProduction, opts.rollupAlias),
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
    return del(opts.destinationPath, { force: true });
}

module.exports = { compile, clean };
argsExecute(module.exports);
