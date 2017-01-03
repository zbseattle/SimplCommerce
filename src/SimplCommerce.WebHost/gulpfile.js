/// <binding AfterBuild='copy-modules' />
"use strict";

var gulp = require('gulp'),
    clean = require('gulp-clean');

var paths = {
    devModules: "../Modules/",
    hostModules: "./Modules/"
};

var modules = [
    'SimplCommerce.Module.Core',
    'SimplCommerce.Module.Catalog',
    'SimplCommerce.Module.Orders',
    'SimplCommerce.Module.Cms',
    'SimplCommerce.Module.Search',
    'SimplCommerce.Module.SampleData',
    'SimplCommerce.Module.Localization',
    'SimplCommerce.Module.ActivityLog',
    'SimplCommerce.Module.Reviews'
];

gulp.task('clean-module', function () {
    return gulp.src([paths.hostModules + '*'], { read: false })
    .pipe(clean());
});

gulp.task('copy-modules', ['clean-module'], function () {
    modules.forEach(function (module) {
        gulp.src([paths.devModules + module + '/Views/**/*.*', paths.devModules + module + '/wwwroot/**/*.*'], { base: module })
            .pipe(gulp.dest(paths.hostModules + module));
        gulp.src(paths.devModules + module + '/bin/Debug/netstandard1.6/**/*.*')
            .pipe(gulp.dest(paths.hostModules + module + '/bin'));
    });

    gulp.src(paths.devModules + 'SimplCommerce.Module.SampleData/SampleContent/**/*.*')
            .pipe(gulp.dest(paths.hostModules + 'SimplCommerce.Module.SampleData/SampleContent'));
});

for (var i = 0; i < modules.length; i++) {
    var shortName = modules[i].split('.')[2];
    gulp.task('copy-' + shortName + '-module', function () {
        gulp.src([paths.devModules + modules[i] + '/Views/**/*.*', paths.devModules + modules[i] + '/wwwroot/**/*.*'], { base: modules[i] })
            .pipe(gulp.dest(paths.hostModules + modules[i]));
        gulp.src(paths.devModules + modules[i] + '/bin/Debug/netstandard1.6/**/*.*')
            .pipe(gulp.dest(paths.hostModules + modules[i] + '/bin'));
    });
}

gulp.task('copy-ms', function () {
    modules.forEach(function (module) {
        gulp.src([paths.devModules + module + '/Views/**/*.*', paths.devModules + module + '/wwwroot/**/*.*'], { base: module })
            .pipe(gulp.dest(paths.hostModules + module));
    });

    gulp.src(paths.devModules + 'SimplCommerce.Module.SampleData/SampleContent/**/*.*')
            .pipe(gulp.dest(paths.hostModules + 'SimplCommerce.Module.SampleData/SampleContent'));
});
