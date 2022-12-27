const commonjs = require('@rollup/plugin-commonjs');
const del = require('del');
const fs = require('fs');
const json = require('@rollup/plugin-json');
const path = require('path');
const replace = require('@rollup/plugin-replace');
const { nodeResolve } = require('@rollup/plugin-node-resolve');

const configureRollupAlias = require('./configure-rollup-alias');
const rollupPipeline = require('./rollup-pipeline');
const vuePlugin = require('./rollup-plugin-vue-sfc-orchard-core');
const { getVueComponents } = require('./get-vue-files');
const { executeFunctionByCommandLineArgument, leaveNodeModule } = require('./process-helpers');

// If this script is invoked from "npm explore lombiq-vuejs" then we have to navigate back to the current project root.
leaveNodeModule();

const defaultOptions = {
    sfcRootPath: path.resolve('Assets', 'Scripts', 'VueComponents'),
    sfcDestinationPath: path.resolve('wwwroot', 'vue'),
    vueJsNodeModulesPath: path.resolve(__dirname, '..', '..', '..', 'node_modules'),
    rollupAlias: {},
    isProduction: false,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    if (!fs.existsSync(opts.sfcRootPath)) return Promise.resolve([]);
    const components = getVueComponents(opts.sfcRootPath);
    if (components.length === 0) return Promise.resolve([]);

    process.stdout.write(`vue component files: ${components.join(', ')}\n`);

    if (!fs.existsSync(opts.vueJsNodeModulesPath)) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" does not exist!`);
    }
    if (!fs.lstatSync(opts.vueJsNodeModulesPath).isDirectory()) { // #spell-check-ignore-line
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" is not a directory!`);
    }

    return rollupPipeline(
        opts.sfcDestinationPath,
        components.map((appName) => ({ fileName: appName, entryPath: path.join(opts.sfcRootPath, appName) })),
        [
            vuePlugin(),
            json(),
            configureRollupAlias(opts.vueJsNodeModulesPath, opts.isProduction, opts.rollupAlias),
            nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }), // #spell-check-ignore-line
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
}

async function clean(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;
    return del(opts.sfcDestinationPath, { force: true });
}

module.exports = { compile, clean };
executeFunctionByCommandLineArgument(module.exports);
