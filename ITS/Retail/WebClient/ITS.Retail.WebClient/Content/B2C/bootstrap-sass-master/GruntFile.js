'use strict';

module.exports = function(grunt) {

	grunt.initConfig({

		pkg: grunt.file.readJSON('package.json'),

		project: {
			app: ['app'],
			assets: ['<%= project.app %>/bootstrap'],
			css: ['<%= project.assets %>/stylesheets/style.scss']
		},
		sass: {
			dev: {
				options: {
					style: 'expanded',
					compass: false
				},
				files: {
					'<%= project.assets %>/stylesheets/style.css':'<%= project.css %>'
				}
			}
		},
		watch: {
			sass: {
				files: '<%= project.assets %>/stylesheets/{,*/}*.{scss,sass}',
				tasks: ['sass:dev']
			}
		}
	});

	grunt.loadNpmTasks('grunt-contrib-sass');
	grunt.loadNpmTasks('grunt-contrib-watch');

	grunt.registerTask('default', [
		'watch'
	]);

};