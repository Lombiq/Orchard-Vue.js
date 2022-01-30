const all = require('gulp-all');
const path = require('path');
const gulp = require('gulp');
const plumber = require('gulp-plumber');
const rollup = require('gulp-rollup');
const rename = require('gulp-rename');
const uglify = require('gulp-uglify');
const log = require('fancy-log');

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
                .pipe(plumber({
                    errorHandler: function errorHandler(error) {
                        // Necessary because the regular error.toString() is a bit broken.
                        const details = Object.entries(error)
                            .map((pair) => ({
                                name: pair[0],
                                value: ('' + pair[1]) === '[object Object]' ? JSON.stringify(pair[1]) :  ('' + pair[1]),
                            }))
                            .filter((pair) => pair.name !== 'plugin' && pair.name !== 'message')
                            .map((pair) => `    ${pair.name}: ${pair.value}`.replace(/\n/g, '\n    '))
                            .join('\n')

                        log(`Plumber has found an unhandled error:\nError in plugin "${error.plugin}"\nMessage:\n` +
                            `    ${error.message}\nDetails:\n${details}`);
                    }
                }))
                .pipe(rollup(options))
                .pipe(rename(outputFileName + '.js'))
                .pipe(gulp.dest(destinationPath))
                .pipe(uglify())
                .pipe(rename(outputFileName + '.min.js'))
                .pipe(gulp.dest(destinationPath));
        }));
};
