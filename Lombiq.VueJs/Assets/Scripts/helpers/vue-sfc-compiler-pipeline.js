const del = require('del');
const fs = require('fs');
const json = require('@rollup/plugin-json');
const alias = require('@rollup/plugin-alias');
const path = require('path');
const replace = require('@rollup/plugin-replace');
const { nodeResolve } = require('@rollup/plugin-node-resolve');

const rollupPipeline = require('./rollup-pipeline');
const vuePlugin = require('./rollup-plugin-vue-sfc-orchard-core');
const { getVueComponents } = require('./get-vue-files');
const { executeFunctionByCommandLineArgument, leaveNodeModule } = require('./process-helpers');
const tryOpenJson = require('./try-open-json');

// If this script is invoked from "npm explore lombiq-vuejs" then we have to navigate back to the current project root.
leaveNodeModule();

const defaultOptions = {
    sfcRootPath: path.join('Assets', 'Scripts', 'VueComponents'),
    sfcDestinationPath: path.join('wwwroot', 'vue'),
    vueJsNodeModulesPath: path.resolve(__dirname, '..', '..', '..', 'node_modules'),
    rollupAlias: {},
    rollupNodeResolve: { preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] },
    isProduction: false,
};

function processRollupNodeResolve(opts) {
    if (!opts.rollupNodeResolve) opts.rollupNodeResolve = {};

    if (Array.isArray(opts.rollupNodeResolve.resolveOnlyRules)) {
        const rules = opts.rollupNodeResolve.resolveOnlyRules;

        opts.rollupNodeResolve.resolveOnly = function resolveOnly(item) {
            for (let i = 0; i < rules.length; i++) {
                const rule = rules[i];
                if (rule.regex && item.match(new RegExp(rule.value))) return !!rule.include;
                if (!rule.regex && item === rule.value) return !!rule.include;
            }

            return true;
        };
    }
}

function compile(options) {
    const fileOptions = tryOpenJson('vue-sfc-compiler-pipeline.json');
    const opts = { ...defaultOptions, ...fileOptions, ...(options ?? { }) };
    processRollupNodeResolve(opts);

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
            alias(opts.rollupAlias),
            nodeResolve(opts.rollupNodeResolve),
            replace({
                values: {
                    'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                    'process.env.BUILD': JSON.stringify('web'),
                },
                preventAssignment: true,
            }),
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
