const gulp = require('gulp');

const assetsSourceBasePath = './node_modules/';
const assetsDestinationPath = './wwwroot/';

const es6PromisePath = assetsSourceBasePath + 'es6-promise/dist/*.js';

const jsAssets = [es6PromisePath];

gulp.task('copy-assets', () => gulp.src(jsAssets).pipe(gulp.dest(assetsDestinationPath)));

gulp.task('default', gulp.parallel('copy-assets'));
