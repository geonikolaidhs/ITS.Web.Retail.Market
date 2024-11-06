// All JavaScript in here will be loaded server-side
// Expose components globally so ReactJS.NET can use them
var React = require('expose?React!react');
var ReactCSSTransitionGroup = require('expose?ReactCSSTransitionGroup!react-addons-css-transition-group');
var ReactDOM = require('expose?ReactDOM!react-dom');
var Components = require('expose?Components!./src/components');
