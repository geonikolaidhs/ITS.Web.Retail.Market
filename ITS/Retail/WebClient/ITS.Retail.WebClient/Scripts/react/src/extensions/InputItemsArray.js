/**
 * Class Representing InputItemsArray
 * @extends Array
 */
export default class InputItemsArray extends Array{
  /**
   * InputItemsArray constructor
   */
  constructor() {
    super();
  }
  /**
   * addItem function adds an new item or updates an existinh item in items array
   * @param {Object} newItem - the new Item
   * @param {Array} items - the items Array
   */
  static addItem(newItem,items) {

    let foundItem = this.foundItem(newItem,items);

    if (foundItem) {
      foundItem.value = newItem.value
    }
    else {
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
  static removeItem(newItem,items){
    let returnedArray =  items.filter(function(item) {
         return item.name !== newItem.name
    });
    return returnedArray;
  }
  /**
   * foundItem method finds the if newItem exists in items array
   * @param  {Object} newItem - the new Item
   * @param  {Array} items   - the items Array
   * @return {boolean}         true or false if newItem exists into the array or not
   */
  static foundItem(newItem,items){
    return items.filter(function(item) {
      return item.name === newItem.name;
    })[0];
  }
  static empty(items){
    return [];
  }
}
