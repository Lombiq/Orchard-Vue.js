const all = require('gulp-all');
const path = require('path');
const gulp = require('gulp');
const plumber = require('gulp-plumber');
const rollup = require('gulp-rollup');
const rename = require('gulp-rename');
const uglify = require('gulp-uglify');

module.exports = function rollupPipeline(
    destinationPath,
    filesAndEntryPaths,
    rollupPlugins,
    rollupOptions = null,
    outputFileNameTransform = null) {
    return all(filesAndEntryPaths
        .map((pair) => {
            const fileName = pair.fileName;
            const entryPath = pair.entryPath;
            const outputFileName = outputFileNameTransform ? outputFileNameTransform(fileName) : fileName;

            const defaultRollupOptions = {
                allowRealFiles: true,
                input: path.join(entryPath),
                output: {
                    format: 'cjs',
                },
                onwarn: (warning, next) => {
                    if (warning.code === 'THIS_IS_UNDEFINED') return;
                    next(warning);
                },
            };
            const options = rollupOptions ? { ...defaultRollupOptions, ...rollupOptions } : defaultRollupOptions;

            if (Array.isArray(rollupPlugins)) {
                if (Array.isArray(options.plugins)) {
                    rollupPlugins.forEach(plugin => options.plugins.push(plugin));
                }
                else {
                    options.plugins = rollupPlugins;
                }
            }

            return gulp.src(entryPath)
                .pipe(plumber())
                .pipe(rollup(options))
                .pipe(rename(outputFileName + '.js'))
                .pipe(gulp.dest(destinationPath))
                .pipe(uglify())
                .pipe(rename(outputFileName + '.min.js'))
                .pipe(gulp.dest(destinationPath));
        }));
};
