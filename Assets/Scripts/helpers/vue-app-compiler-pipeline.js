const gulp = require('gulp');
const all = require('gulp-all');
const plumber = require('gulp-plumber');
const rollup = require('gulp-rollup');
const uglify = require('gulp-uglify');
const rename = require('gulp-rename');
const buble = require('rollup-plugin-buble');
const babel = require('rollup-plugin-babel');
const alias = require('rollup-plugin-alias');
const commonjs = require('rollup-plugin-commonjs');
const replace = require('rollup-plugin-replace');
const json = require('rollup-plugin-json');
const nodeResolve = require('rollup-plugin-node-resolve');
const path = require('path');
const onError = require('rollup-plugin-onerror');
const vue = require('rollup-plugin-vue');

const getVueApps = require('./get-vue-apps');

const defaultOptions = {
    rootPath: './Assets/Apps/',
    destinationPath: './wwwroot/apps/',
    rollupAlias: {},
    isProduction: false,
    overrideEntryPath: null,
    getAppNames: getVueApps,
    withCommonJs: true,
    withBabel: false,
    commonJsOptions: undefined,
};

function compile(options) {
    const opts = options ? { ...defaultOptions, ...options } : defaultOptions;

    function nodeModulePath(relativePath) {
        return path.resolve(path.join(__dirname, '..', '..', '..', 'node_modules', relativePath));
    }

    return all(opts.getAppNames(opts.rootPath)
        .map((appName) => {
            const entryPath = opts.overrideEntryPath
                ? opts.overrideEntryPath(opts, appName)
                : path.join(opts.rootPath, appName, '/main.js');

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
                        onError((err) => {
                            console.log('There was an Error with your rollup build');
                            console.error(err);
                        }),
                        vue(),
                        json(),
                        nodeResolve({ preferBuiltins: true, browser: true, mainFields: ['module', 'jsnext:main'] }),
                        alias({
                                vue: nodeModulePath(
                                    opts.isProduction ? 'vue/dist/vue.common.prod.js' : 'vue/dist/vue.esm.browser.js'),
                                vuelidate: nodeModulePath('vuelidate/'),
                                'vue-router': nodeModulePath('vue-router/dist/vue-router.common.js'),
                                'vue-axios': nodeModulePath('vue-axios/'),
                                axios: nodeModulePath('axios/'),
                                resolve: ['.js', '/index.js', '/lib/index.js', '/src/index.js'],
                                ...opts.rollupAlias,
                            }),
                        replace({
                            'process.env.NODE_ENV': JSON.stringify(opts.isProduction ? 'production' : 'development'),
                            'process.env.BUILD': JSON.stringify('web'),
                        }),
                    ]
                        .concat(opts.withCommonJs ? [commonjs(opts.commonJsOptions)] : [])
                        .concat(opts.withBabel ? [babel()] : [buble()]),
                }))
                .pipe(rename(appName + '.js'))
                .pipe(gulp.dest(opts.destinationPath))
                .pipe(uglify())
                .pipe(rename(appName + '.min.js'))
                .pipe(gulp.dest(opts.destinationPath));
        }));
}

module.exports = { compile };
