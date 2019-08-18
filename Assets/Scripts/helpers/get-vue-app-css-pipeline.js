const gulp = require('gulp');
const all = require('gulp-all');
const rename = require('gulp-rename');
const path = require('path');

const getVueApps = require('./get-vue-apps');

const defaultOptions = {
    rootPath: './Assets/Apps/',
    destinationPath: './wwwroot/Apps/',
    vueJsNodeModulesPath: '../Lombiq.VueJs/node_modules',
    stylesPath: 'styles',
    rollupAlias: {}
};

const getVueAppCssPipeline = options => {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return all(getVueApps(opts.rootPath)
        .map(function (appName) {
            const entryPath = path.join(opts.rootPath, appName, opts.stylesPath, '/*.css');

            console.log(entryPath);

            return gulp.src(entryPath)
                .pipe(rename({ dirname: '' }))
                .pipe(gulp.dest(opts.destinationPath));
        }));
};

module.exports = getVueAppCssPipeline;