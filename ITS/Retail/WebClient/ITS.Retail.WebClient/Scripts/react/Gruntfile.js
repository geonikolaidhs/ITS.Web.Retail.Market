module.exports = function(grunt) {
  grunt.initConfig({
    jsdoc : {
        dist : {
            src: ['src/components/*.jsx','src/extensions/dist/*.js'],
            options: {
                plugins: [ 'jsdoc-jsx'],
                jsx: {
                  "extensions": ["js", "jsx"]
                },
                destination: 'doc',
                template : "node_modules/ink-docstrap/template",
                configure : "jsdoc.conf.json"
            }
        }
    },
    removelogging: {
      dist: {
        src: '../build/*.bundle.js'
      }
    },
    remove_usestrict: {
      dist: {
        files: [
          {
            expand: true,
            cwd: '../build',
            dest: '../build',
            src: '*.bundle.js'
          }
        ]
      }
    },
    watch: {
      files: ['src/components/*.js','src/components/*.jsx'],
      tasks: ['jsdoc'],
    },
  });

  grunt.loadNpmTasks('grunt-jsdoc');

  grunt.loadNpmTasks('grunt-contrib-watch');

  grunt.loadNpmTasks("grunt-remove-logging");

  grunt.loadNpmTasks('grunt-remove-usestrict');

  grunt.registerTask('production',['remove_usestrict','jsdoc']);

	grunt.registerTask('default', ['watch']);
};
