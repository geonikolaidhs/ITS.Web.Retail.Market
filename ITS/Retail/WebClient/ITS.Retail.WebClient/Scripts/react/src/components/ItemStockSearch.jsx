import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';

/**
 * Class for ItemStock search for later reprocess
 * @extends React.Component
 */
export default class ItemStockSearch extends React.Component {
   /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      language:props.language,
      store: props.storeValue,
      itembarcode: props.itemBarcode,
      from_date: props.from_date
    };
  }

  render(){
    return(
        <div className="inputGroup">
            <div className="inputItems">
            </div>
        </div>
    );
  }
}
