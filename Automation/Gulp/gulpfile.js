var gulp = require('gulp'),
	compass = require('gulp-compass'),
	autoprefixer = require('gulp-autoprefixer'),
	jshint = require('gulp-jshint'),
	jshintXMLReporter = require('gulp-jshint-xml-file-reporter'),
	imagemin = require('gulp-imagemin'),
	pngquant = require('imagemin-pngquant'),
	notify = require('gulp-notify'),
	scsslint = require('gulp-scss-lint'),
	coffee = require("gulp-coffee"),
	jsdoc = require("gulp-jsdoc"),
	coffeelint = require('gulp-coffeelint'),
	stylish = require('coffeelint-stylish'),
	gutil = require('gulp-util'),
	path = require('path');


// task
gulp.task('compile-coffee', function () {
    return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/*.coffee') // path to your file
    .pipe(coffee({bare: true}).on('error', gutil.log))
    .pipe(gulp.dest('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/scripts'))
    .pipe(notify({ message: 'Coffee Compile complete' }));
});


gulp.task('lint-coffee', function () {
    return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/*.coffee')
    .pipe(coffeelint('coffeelint.json'))
    .pipe(coffeelint.reporter(stylish))
    .pipe(notify({ message: 'Coffee lint complete' }));
});


gulp.task('sass', function() {
	return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/css/*.scss')
    .pipe(compass({
      project: path.join(__dirname, '../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content'),
			//sourcemap: true,
      css: 'css',
      sass: 'css'
    }))
	.pipe(autoprefixer({
		browsers: ['last 2 versions']
	}))
	.pipe(notify({ message: 'Sass task complete' }));
});


gulp.task('sass-b2c', function() {
	return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/B2C/css/*.scss')

    .pipe(compass({
      project: path.join(__dirname, '../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/B2C'),
			//sourcemap: true,
      css: 'css',
      sass: 'css'
    }))
	.pipe(autoprefixer({
		browsers: ['last 2 versions']
	}))
	.pipe(notify({ message: 'Sass B2C task complete' }));
});

gulp.task('lint-scss', function() {
  return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/css/*.scss')
    .pipe(scsslint({
	  'maxBuffer': 99999999,
	  'reporterOutputFormat': 'Checkstyle',
	  'filePipeOutput': 'scssReport.xml'
    }))
    .on('error', function(error) {
      // Would like to catch the error here
      console.log(error);
      this.emit('end');
    })
    .pipe(gulp.dest('../../ITS/Retail/WebClient/ITS.Retail.WebClient/reports'))
  	.pipe(notify({ message: 'Scss-lint task complete' }));
});

gulp.task('lint-scss-b2c', function() {
  return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/B2C/css/*.scss')
    .pipe(scsslint({
	  'maxBuffer': 99999999,
	  'reporterOutputFormat': 'Checkstyle',
	  'filePipeOutput': 'scssReportB2C.xml'
    }))
    .on('error', function(error) {
      // Would like to catch the error here
      console.log(error);
      this.emit('end');
    })
    .pipe(gulp.dest('../../ITS/Retail/WebClient/ITS.Retail.WebClient/reports'))
  	.pipe(notify({ message: 'Scss-lint B2C task complete' }));
});

gulp.task('lint', function () {
    return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/scripts/*.js')
        .pipe(jshint())
        .pipe(jshint.reporter(jshintXMLReporter))
        .on('end', jshintXMLReporter.writeFile({
            format: 'checkstyle',
            filePath: '../../ITS/Retail/WebClient/ITS.Retail.WebClient/reports/jshint.xml'
        }))
		.pipe(notify({ message: 'JSlint task complete' }));
});

gulp.task('documentation-js', function () {
	return gulp.src("../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/scripts/*.js")
	  .pipe(jsdoc('../../documentation-js',{
			path: 'ink-docstrap',
			systemName      : 'WRM',
			footer          : "",
			copyright       : "ITS SA",
			navType         : "vertical",
			theme           : "simplex",
			linenums        : true,
			highlightTutorialCode : true,
			// collapseSymbols : false,
			inverseNav      : true,
			dateFormat : 'dddd, MMMM Do YYYY, h:mm:ss a',
			syntaxTheme :'dark'
		}))
	.pipe(notify({ message: 'Jsdoc task complete' }));
});




gulp.task('images-root', function() {
  return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/*')
    .pipe(imagemin({
	  	optimizationLevel: 3,
	  	progressive: true,
	  	interlaced: true,
	  	progressive: true,
		svgoPlugins: [{removeViewBox: false}],
		use: [pngquant()]
 	}))
  	.pipe(gulp.dest('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content'))
  	.pipe(notify({ message: 'images-root task complete' }));
});

gulp.task('images-subdir', function() {
  return gulp.src('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/images/*')
    .pipe(imagemin({
	  	optimizationLevel: 3,
	  	progressive: true,
	  	interlaced: true,
	  	progressive: true,
		svgoPlugins: [{removeViewBox: false}],
		use: [pngquant()]
 	}))
  	.pipe(gulp.dest('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/images'))
	.pipe(notify({ message: 'images-subdir task complete' }));
});

// Watch
gulp.task('default', function() {

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/css/*.scss', ['sass']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/B2C/css/*.scss', ['sass-b2c']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/css/*.scss', ['lint-scss']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/scripts/*.js', ['lint']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/scripts/*.js', ['documentation-js']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/*', ['images-root']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/images/*', ['images-subdir']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/*.coffee', ['compile-coffee']);

  gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Scripts/*.coffee', ['lint-coffee']);


});


gulp.task('compile-sass',function(){
	gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/css/*.scss', ['sass']);
});

gulp.task('compile-sass-b2c',function(){
	gulp.watch('../../ITS/Retail/WebClient/ITS.Retail.WebClient/Content/B2C/css/*.scss', ['sass-b2c']);
});
