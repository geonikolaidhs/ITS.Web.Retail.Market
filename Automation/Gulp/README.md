## Installation ##

1. Under ITS.Retail.Platform repository resides a folder /Automation/Gulp where Gulp exists. *The folder is outside of ITS.Retail.WebClient because node_modules produce a problem exceeding Windows PATH*
2. Created new project with **npm --init** with the following packages dependencies. See **package.json** for dependencies.
3. Inside gulpfile.js are all the available commands.

## Usage ##
1. Open a command line and cd into folder and execute **gulp**. This command executes the gulp default task, which watches all CSS, Coffee and js files for changes.
2. Other Commands
	1. **gulp compile-coffee** *Compiles all coffee files under Scripts folder*
	2. **gulp lint-coffee** *Lint all coffee files under Scripts folder*
	3. **gulp sass** *Compiles all scss files under Content/css folder*
	4. **gulp sass-b2c** *Compiles all scss files under Content/B2C/css folder*
	5. **gulp lint-scss** *Lint all scss files under Content/css folder*
	6. **gulp lint-scss-b2c ** *Lint all scss files under Content/B2C/css folder*
	7. **gulp lint** *Lint all js files under Scripts folder*
	8. **gulp documentation-js** *Generates documentation for all js files under Scripts folder and saves it under root folder/documentation-js*
	9. **gulp images-root** *Minimizes all images under Content folder*
	10. **gulp images-subdir** *Minimizes all images under Content/images folder*
	11. **gulp compile-sass** *Watches scss files under Content/css folder and compile them*
	12. **gulp compile-sass-b2c** *Watches scss files under Content/B2C/css folder and compile them*

## Reports ##
All lint reports for .scss, js and .coffee are exported to ITS.Retail.WebClient/reports 


## Usefull Links ##
[Performance Optimization Guide](http://yeoman.io/blog/performance-optimization.html)