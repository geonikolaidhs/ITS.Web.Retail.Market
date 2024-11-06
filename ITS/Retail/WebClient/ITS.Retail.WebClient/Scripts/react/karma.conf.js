var webpack = require('webpack');

module.exports = function (config) {
  config.set({
    browsers: ['Chrome'/*, 'Firefox', 'IE'*/],
    singleRun: false, //run only once by default
    frameworks: [ 'mocha','chai' ], //use the mocha test framework
    files: [
      'tests.webpack.js' //just load this file,
    ],
    plugins: [
      'karma-chrome-launcher',
      'karma-firefox-launcher',
      'karma-ie-launcher',
      'karma-phantomjs-launcher',
      'karma-chai',
      'karma-mocha',
      'karma-sourcemap-loader',
      'karma-webpack',
      'karma-coverage',
      'karma-mocha-reporter'
    ],
    preprocessors: {
        'tests.webpack.js': ['webpack', 'sourcemap'], //preprocess with webpack and our sourcemap loader
    },
    reporters: [ 'mocha', 'coverage' ], //report results in this format
    webpack: { //kind of a copy of your webpack config
      devtool: 'inline-source-map', //just do inline source maps instead of the default
      module: {
        loaders: [
          {
            test: /\.jsx$/,
            loader: 'babel',
          }
        ],
        postLoaders: [ { //delays coverage til after tests are run, fixing transpiled source coverage error
            test: /\.jsx$/,
            exclude: /(test|node_modules|bower_components)\//,
            loader: 'istanbul-instrumenter' } ]
      }
    },
    webpackServer: {
      noInfo: true //please don't spam the console when running in karma!
    },
    coverageReporter: {
      type: 'html', //produces a html document after code is run
      dir: 'coverage/' //path to created html doc
    }
  });
};
