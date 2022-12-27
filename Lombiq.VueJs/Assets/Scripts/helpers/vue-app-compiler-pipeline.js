const commonjs = require('@rollup/plugin-commonjs');
const del = require('del');
const fs = require('fs');
const glob = require('glob');
const json = require('@rollup/plugin-json');
const path = require('path');
const replace = require('@rollup/plugin-replace');
const { nodeResolve } = require('@rollup/plugin-node-resolve');

const configureRollupAlias = require('./configure-rollup-alias');
const rollupPipeline = require('./rollup-pipeline');
const { getVueApps } = require('./get-vue-files');
const { executeFunctionByCommandLineArgument, leaveNodeModule } = require('./process-helpers');

// If this script is invoked from "npm explore lombiq-vuejs" then we have to navigate back to the current project root.
leaveNodeModule();

const defaultOptions = {
    appRootPath: path.resolve('Assets', 'Apps'),
    appDestinationPath: path.resolve('wwwroot', 'apps'),
    appStylesPath: 'styles',
    vueJsNodeModulesPath: path.resolve(__dirname, '..', '..', '..', 'node_modules'),
    rollupAlias: {},
    isProduction: false,
};

function compileApp(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    if (!fs.existsSync(opts.vueJsNodeModulesPath)) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" does not exist!`);
    }
    if (!fs.lstatSync(opts.vueJsNodeModulesPath).isDirectory()) { // #spell-check-ignore-line
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" is not a directory!`);
    }

    return rollupPipeline(
        opts.appDestinationPath,
        getVueApps(opts.appRootPath)
            .map((appName) => ({ fileName: appName, entryPath: path.join(opts.appRootPath, appName, '/main.js') })),
        [
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
        (fileName) => path.basename(path.dirname(fileName)));
}

function globPromise(basePath) {
    return new Promise((resolve, reject) => {
        glob(basePath, (err, matches) => (err ? reject(err) : resolve(matches)));
    });
}

function compileCss(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return Promise.all(getVueApps(opts.appRootPath)
        .map(async (appName) => {
            const paths = await globPromise(path.join(
                opts.appRootPath,
                appName,
                opts.appStylesPath,
                '*.css'));

            await Promise.all(paths.map((filePath) => fs.promises.copyFile(
                filePath,
                path.join(opts.appDestinationPath, path.basename(filePath)))));
        }));
}

function compile(options) {
    const appRootPath = options?.appRootPath ? options.appRootPath : defaultOptions.appRootPath;
    if (!fs.existsSync(appRootPath)) return;

    compileApp(options);
    compileCss(options);
}

function clean(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;
    return del(opts.appDestinationPath, { force: true });
}

module.exports = { compile, clean };
executeFunctionByCommandLineArgument(module.exports);
