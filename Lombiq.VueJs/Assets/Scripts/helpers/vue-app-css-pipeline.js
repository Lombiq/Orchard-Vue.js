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

function globPromise(path) {
    return new Promise((resolve, reject) =>
        glob(path, (err, matches) =>
            err ? reject(err) : resolve(matches)));
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

            for (let i = 0; i < paths.length; i++) {
                await fs.promises.copyFile(
                    paths[i],
                    path.join(opts.destinationPath, path.basename(paths[i]))
                )
            }
        }));
};

module.exports = { compileCss };
