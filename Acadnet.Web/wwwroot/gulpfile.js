"use strict";

const browserSync  = require('browser-sync').create();
const gulp         = require('gulp');
const sass         = require('gulp-sass')(require('node-sass'));
const concat       = require('gulp-concat');
const postcss      = require('gulp-postcss');
const autoprefixer = require('autoprefixer');
const cssnano      = require('cssnano');

function cssMain() {
  return gulp.src("./sass/main.scss")
    .pipe(sass({
      outputStyle: 'expanded',
    }))
    .pipe(postcss([
      autoprefixer({ Browserslist: ['defaults'] }),
    ]))
    .pipe( gulp.dest("./css") )
    .pipe( browserSync.stream() );
}

function cssVendors() {
  return gulp.src("./sass/vendors/*.css")
    .pipe( concat('vendors.css') )
    .pipe( gulp.dest("./css") )
}

function framework() {
  return gulp.src("./sass/framework/main.scss")
    .pipe(sass())
    .pipe(postcss([
      autoprefixer({ Browserslist: ['defaults'] }),
      cssnano(),
    ]))
    .pipe( concat('bootstrap.css') )
    .pipe( gulp.dest("./sass/vendors/") )
}

function server() {
  browserSync.init({
    notify: false,
    server: {
      baseDir: "./"
    }
  });

  gulp.watch('*.html').on('change', gulp.series( browserSync.reload ));
  gulp.watch( ["sass/**/*.scss", "!sass/vendors/**"], cssMain );
  gulp.watch( "sass/vendors/*.*" ).on('change', gulp.series( cssVendors, browserSync.reload ));
}

exports.server = server;
exports.cssMain = cssMain;
exports.cssVendors = cssVendors;
exports.framework = framework;

exports.build = gulp.series(
  framework,
  gulp.parallel(
    cssMain,
    cssVendors
  ),
  server,
);
