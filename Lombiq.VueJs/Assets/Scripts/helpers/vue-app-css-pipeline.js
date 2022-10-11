const fs = require('fs');
const path = require('path');
const glob = require('glob');

const { getVueApps } = require('./get-vue-files');

const defaultOptions = {
    rootPath: './Assets/Apps/',
    destinationPath: './wwwroot/apps/',
    vueJsNodeModulesPath: '../Lombiq.VueJs/node_modules',
    stylesPath: 'styles',
    rollupAlias: {},
};

function globPromise(basePath) {
    return new Promise((resolve, reject) => {
        glob(basePath, (err, matches) => (err ? reject(err) : resolve(matches)));
    });
}

const compileCss = (options) => {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return Promise.all(getVueApps(opts.rootPath)
        .map(async (appName) => {
            const paths = await globPromise(path.join(
                opts.rootPath,
                appName,
                opts.stylesPath,
                '*.css'));

            await Promise.all(paths.map((filePath) => fs.promises.copyFile(
                filePath,
                path.join(opts.destinationPath, path.basename(filePath)),
            )));
        }));
};

module.exports = { compileCss };
