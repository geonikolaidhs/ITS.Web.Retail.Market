const postcss = require('gulp-postcss'),
      gulp = require('gulp'),
      fs = require('fs'),
      path = require('path'),
      concatCss = require('gulp-concat-css');
      babel = require('gulp-babel'),
      reporter = require("postcss-reporter");

gulp.task('post-css', function () {
  return gulp.src('css/postcss/style.css')
    .pipe( postcss([
      require('postcss-mixins'),
      require('postcss-nested'),
      require('precss'),
      require('postcss-easings'),
      require('lost'),
      require('autoprefixer'),
      require('postcss-media-variables')(), // first run
      require('postcss-custom-media')(/* options */),
      require('postcss-css-variables')(/* options */),
      require('postcss-calc')(/* options */),
      require('postcss-media-variables')(), // second run
      require('postcss-bem')({
       style: 'suit', // suit or bem, suit by default,
      }),
      // require('doiuse')({
      //   browsers: [
      //     'ie >= 9',
      //     '> 1%'
      //   ],
      //   //ignore: ['rem'], // an optional array of features to ignore
      //   onFeatureUsage: function (usageInfo) {
      //     console.log(usageInfo.message)
      //   }
      // }),
      // require('stylelint')(require('stylelint-config-suitcss')),
      // reporter({ clearMessages: true }),
      ]
    ))
    .pipe( gulp.dest('css/') );
});


// identifies a dependent task must be complete before this one begins
gulp.task('css', ['post-css'], function() {
  return gulp.src(['css/lib/glyphicons.css','css/style.css'])
    .pipe(concatCss("./css/style.css"))
    .pipe(gulp.dest('../build/'));
});

//Copy JS to Dev
gulp.task('copy-js-2-dev', function () {
    return gulp.src([
      './node_modules/react/dist/react-with-addons.js',
      './node_modules/react-dom/dist/react-dom.js'
    ])
    .pipe(gulp.dest('../build/'))
})

gulp.task('copy-css-2-dev', function () {
    return gulp.src([
      './node_modules/react-select/dist/react-select.css',
      './node_modules/react-day-picker/lib/style.css'
    ])
    .pipe(gulp.dest('../build/lib/'))
})

gulp.task('copy-fonts-2-dev',function(){
  return gulp.src('./fonts/*.{eot,svg,ttf,woff,woff2}')
    .pipe(gulp.dest('../build/fonts/'))
})

gulp.task('babel', function(){
    return gulp.src('src/extensions/*.js')
        .pipe(babel())
        .pipe(gulp.dest('src/extensions/dist'));
});


gulp.task('default',['copy-js-2-dev','copy-fonts-2-dev','copy-css-2-dev','css','babel'], function(){
  gulp.watch('css/**/*.css', ['css']);
  gulp.watch('src/extensions/*.js', ['babel']);
});

gulp.task('production',['copy-js-2-dev','copy-fonts-2-dev','copy-css-2-dev','css','babel']);
