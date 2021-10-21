const vueAppCompilerPipeline = require('./vue-app-compiler-pipeline');
const vue = require('rollup-plugin-vue');
const fs = require('fs');
const path = require('path');

function getVueComponentNames(rootPath) {
    return fs
        .readdirSync(rootPath)
        .filter((file) => file.endsWith('.vue') && fs.statSync(path.join(rootPath, file)).isFile())
        .map((file) => file.replace(/\.vue$/, ''));
}

function getEntryPath(opts, componentName) {
    return path.join(opts.rootPath, componentName + '.vue');
}

const defaultOptions = {
    rootPath: './Assets/Components/',
    destinationPath: './wwwroot/components/',
    overrideEntryPath: getEntryPath,
    getAppNames: getVueComponentNames,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return vueAppCompilerPipeline.compile(opts);
}

module.exports = { compile };
