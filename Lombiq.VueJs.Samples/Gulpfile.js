// Learn more about using Gulp with our Lombiq Gulp Extensions here:
// https://github.com/Lombiq/Orchard-Training-Demo-Module/blob/dev/Gulpfile.js

const gulp = require('gulp');
const watch = require('gulp-watch');

// This is a helper for generating a Gulp pipeline for harvesting Vue applications from the current project's
// /Assets/Apps folder and compiling them to wwwroot.
const vueApp = require('../Lombiq.VueJs/Assets/Scripts/helpers/vue-app-compiler-pipeline');

// This is a helper for generating a Gulp pipeline for harvesting Vue Single File Components from the current project's
// /Assets/Scripts/VueComponents folder and compiling them to wwwroot.
const vueSfc = require('../Lombiq.VueJs/Assets/Scripts/helpers/vue-sfc-compiler-pipeline');

// Gulp tasks for harvesting and compiling Vue apps and single file components.
gulp.task('build:vue-app', () => vueApp.compile());
gulp.task('build:vue-sfc', () => vueSfc.compile());

// Default task that executes all the required tasks to initialize the module assets.
gulp.task('default', gulp.parallel('build:vue-app', 'build:vue-sfc'));

// This task won't be executed automatically, if you want to test this, you need to execute it in the Task Runner
// Explorer. With this you'll be able to automatically compile and minify the sass files right after when you save them.
const assetsBasePath = './Assets/';
gulp.task('watch', () => {
    watch(assetsBasePath + 'Apps/**/*.js', { verbose: true }, gulp.series('build:vue-app'));
    watch(assetsBasePath + 'Scripts/VueComponents/**/*.js', { verbose: true }, gulp.series('build:vue-sfc'));
});

// END OF TRAINING SECTION
// The last two stations were shared. Please return to the Readme to start another section.
