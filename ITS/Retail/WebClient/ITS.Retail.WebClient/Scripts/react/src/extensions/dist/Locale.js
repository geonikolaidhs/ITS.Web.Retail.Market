'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

var _messages = require('./messages');

var _messages2 = _interopRequireDefault(_messages);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

/**
 * Class Providing Locale messages per language
 */
var Locale = function () {
  /**
   * Initialize this.lang and this.message
   */
  function Locale() {
    _classCallCheck(this, Locale);

    this.lang = lang;
    this.message = message;
  }
  /**
   * Get Message with lang and message name and return the localized message
   * @param  {String} lang    Language Locale
   * @param  {String} message Message
   * @return {String}         Returned Message
   */


  _createClass(Locale, null, [{
    key: 'getMessage',
    value: function getMessage(lang, message) {
      return _messages2.default.values[lang][message];
    }
  }]);

  return Locale;
}();

exports.default = Locale;