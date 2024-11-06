var path = require('path'),
    webpack = require('webpack');

module.exports = {
    //watch: true,
    //context: path.join(__dirname, 'react'),
    entry: {
        server: './server',
        client: './client'
    },
    output: {
        path: path.join(__dirname, '../build'),
        filename: '[name].bundle.js'
    },
    plugins: [
        new webpack.optimize.OccurenceOrderPlugin()
    ],
    cache: true,
    debug: false,
    devtool: 'eval-cheap-source-map"',
    module: {
      preLoaders: [
        {
          test: /\.jsx?$/,
          loaders: ['eslint']//, 'jscs']
        }
      ],
      loaders: [
        // Transform JSX in .jsx files
        {
          test: /\.jsx$/,
          exclude: /node_modules/,
          loader: 'babel',
          query: {
            retainLines: true,
            cacheDirectory: true
          },
          include: [
             path.resolve(__dirname, "src/components"),
          //   path.resolve(__dirname, "extensions")
          ],
          exclude: [
             path.resolve(__dirname,"node_modules"),
             path.resolve(__dirname,"src/extensions")
          ]
        }
      ]
    },
    resolve: {
        // Allow require('./blah') to require blah.jsx
        extensions: ['', '.jsx', '.js']
    },
    externals: {
        // Use external version of React (from CDN for client-side, or
        // bundled with ReactJS.NET for server-side)
        jquery: 'jQuery'
    }
};
