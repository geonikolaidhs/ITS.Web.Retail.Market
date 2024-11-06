"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

/**
 * Class Representing InputItemsArray
 * @extends Array
 */
var InputItemsArray = function (_Array) {
  _inherits(InputItemsArray, _Array);

  /**
   * InputItemsArray constructor
   */
  function InputItemsArray() {
    _classCallCheck(this, InputItemsArray);

    return _possibleConstructorReturn(this, (InputItemsArray.__proto__ || Object.getPrototypeOf(InputItemsArray)).call(this));
  }
  /**
   * addItem function adds an new item or updates an existinh item in items array
   * @param {Object} newItem - the new Item
   * @param {Array} items - the items Array
   */


  _createClass(InputItemsArray, null, [{
    key: "addItem",
    value: function addItem(newItem, items) {

      var foundItem = this.foundItem(newItem, items);

      if (foundItem) {
        foundItem.value = newItem.value;
      } else {
        return items.push({
          name: newItem.name,
          value: newItem.value
        });
      }
    }
    /**
     * removeItem method removes the item from an existing item in items array
     * @param  {Object} newItem - the new Item
     * @param  {Array} items - the items Array
     * @return {Array}         the new Array minus the newItem
     */

  }, {
    key: "removeItem",
    value: function removeItem(newItem, items) {
      var returnedArray = items.filter(function (item) {
        return item.name !== newItem.name;
      });
      return returnedArray;
    }
    /**
     * foundItem method finds the if newItem exists in items array
     * @param  {Object} newItem - the new Item
     * @param  {Array} items   - the items Array
     * @return {boolean}         true or false if newItem exists into the array or not
     */

  }, {
    key: "foundItem",
    value: function foundItem(newItem, items) {
      return items.filter(function (item) {
        return item.name === newItem.name;
      })[0];
    }
  }, {
    key: "empty",
    value: function empty(items) {
      return [];
    }
  }]);

  return InputItemsArray;
}(Array);

exports.default = InputItemsArray;