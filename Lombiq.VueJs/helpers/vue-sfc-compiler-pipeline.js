import fs from 'fs';
import json from '@rollup/plugin-json';
import alias from '@rollup/plugin-alias';
import path from 'path';
import replace from '@rollup/plugin-replace';
import { nodeResolve } from '@rollup/plugin-node-resolve';

import { rollupPipeline } from './rollup-pipeline.js';
import { vuePlugin } from './rollup-plugin-vue-sfc-orchard-core.js';
import { getVueComponents } from './get-vue-files.js';
import { executeFunctionByCommandLineArgument, leaveNodeModule } from './process-helpers.js';
import { tryOpenAndParse } from './try-open-json.js';

// If this script is invoked from "npm explore lombiq-vuejs" then we have to navigate back to the current project root.
leaveNodeModule();

const defaultOptions = {
    sfcRootPath: path.join('Assets', 'Scripts', 'VueComponents'),
    sfcDestinationPath: path.join('wwwroot', 'vue'),
    vueJsNodeModulesPath: path.resolve(import.meta.dirname, '..', 'node_modules'),
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

export function compile(options) {
    const fileOptions = tryOpenAndParse('vue-sfc-compiler-pipeline.json');
    const opts = { ...defaultOptions, ...fileOptions, ...(options ?? { }) };
    processRollupNodeResolve(opts);

    if (!fs.existsSync(opts.sfcRootPath)) return Promise.resolve([]);
    const components = getVueComponents(opts.sfcRootPath);
    if (components.length === 0) return Promise.resolve([]);

    process.stdout.write(`vue component files: ${components.join(', ')}\n`);

    if (!fs.existsSync(opts.vueJsNodeModulesPath)) {
        throw new Error(`The vueJsNodeModulesPath option's path "${opts.vueJsNodeModulesPath}" does not exist!`);
    }
    if (!fs.lstatSync(opts.vueJsNodeModulesPath).isDirectory()) {
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

export async function clean(options) {
    const { deleteAsync } = await import('del');
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return deleteAsync(opts.sfcDestinationPath, { force: true });
}

executeFunctionByCommandLineArgument({ compile, clean });
