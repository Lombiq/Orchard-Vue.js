const gulp = require('gulp');
const all = require('gulp-all');
const plumber = require('gulp-plumber');
const rollup = require('gulp-rollup');
const uglify = require('gulp-uglify');
const rename = require('gulp-rename');
const buble = require('rollup-plugin-buble');
const alias = require('rollup-plugin-alias');
const commonjs = require('rollup-plugin-commonjs');
const replace = require('rollup-plugin-replace');
const json = require('rollup-plugin-json');
const nodeResolve = require('rollup-plugin-node-resolve');
const path = require('path');
const vuePlugin = require('rollup-plugin-vue');

const { getVueComponents } = require('./get-vue-files');

const defaultOptions = {
    rootPath: './Assets/Scripts/VueComponents/',
    destinationPath: './wwwroot/vue/',
    vueJsNodeModulesPath: '../Lombiq.VueJs/node_modules',
    rollupAlias: {},
    isProduction: false,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    return all(getVueComponents(opts.rootPath)
        .map((appName) => {
            const entryPath = path.join(opts.rootPath, appName);

            return gulp.src(entryPath)
                .pipe(plumber())
                .pipe(rollup({
                    allowRealFiles: true,
                    input: path.join(entryPath),
                    output: {
                        format: 'cjs',
                    },
                    onwarn: (warning, next) => {
                        if (warning.code === 'THIS_IS_UNDEFINED') return;
                        next(warning);
                    },
                    plugins: [
                        vuePlugin({
                            target: 'node',
                        }),
                        json(),
                        nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }),
                        alias({
                            vue: path.resolve(path.join(opts.vueJsNodeModulesPath, opts.isProduction
                                ? 'vue/dist/vue.common.prod.js'
                                : 'vue/dist/vue.esm.browser.js')),
                            vuelidate: path.resolve(path.join(opts.vueJsNodeModulesPath, 'vuelidate/')),
                            'vue-router': path.resolve(path.join(
                                opts.vueJsNodeModulesPath, 'vue-router/dist/vue-router.common.js')),
                            'vue-axios': path.resolve(path.join(opts.vueJsNodeModulesPath, 'vue-axios/')),
                            axios: path.resolve(path.join(opts.vueJsNodeModulesPath, 'axios/')),
                            resolve: ['.js', '/index.js', '/lib/index.js', '/src/index.js'],
                            ...opts.rollupAlias,
                        }),
                        replace({
                            'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                            'process.env.BUILD': JSON.stringify('web'),
                        }),
                        commonjs(),
                        buble(),
                    ],
                }))
                .pipe(rename(appName + '.js'))
                .pipe(gulp.dest(opts.destinationPath))
                .pipe(uglify())
                .pipe(rename(appName + '.min.js'))
                .pipe(gulp.dest(opts.destinationPath));
        }));
}

module.exports = { compile };
