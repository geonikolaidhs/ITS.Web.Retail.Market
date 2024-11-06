import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';

import InputItemsArray from '../extensions/dist/InputItemsArray';

/**
 * Class Representing Input Item Decimal
 * @extends React.Component
 */
export default class InputItemDecimal extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props){
    super(props);
    this.state = {
      value: 0,
      loadHelpText: false,
      helpText: '',
      isVisible: false
    }
  }
  /**
   * componentWillMount - componentWillMount Event
   * Adds name and value on InputItemsArray on component mount
   */
  componentWillMount(){
    InputItemsArray.addItem({name: this.props.name,value: this.state.value},this.props.inputItems);
  }
  /**
   * handleChange event handler is fired on change of input item
   * @param  {string} type - Input type Possible values Decimal'
   * @param  {Object} event - handleChange event
   */
  handleChange(event) {
    this.setState({
      value: event.target.value,
      loadHelpText: false,
      helpText: ''
    });

    this.validateDecimal(event);
  }
  /**
   * onBlurDecimal event handler is fired on input blur when type is Decimal
   * @param  {Object} event - onBlurDecimal event
   */
  onBlurDecimal(event){
    this.validateDecimal(event);
  }
  /**
   * validateDecimal Decimal Validation - Check if Decimal is not empty and valid Decimal and then add the item into InputItemsArray
   * @param  {Object} event validateDecimal event
   */
  validateDecimal(event){
    if(event.target.value.length === 0 || isNaN(parseFloat(event.target.value))){
      this.setState({
        loadHelpText: true,
        helpText: Locale.getMessage(this.props.language,'addValidNumericValue')
      })
    }
    else{
      InputItemsArray.addItem({name: this.props.name,value: this.state.value},this.props.inputItems);
    }
  }
  /**
   * Render Input Item Component
   * @return {Object} React View
   */
  render () {
    let inputClass = 'InputItem-Input InputItem-Input--glow';
    if (this.state.loadHelpText) inputClass += ' is-Empty';
    return(
      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
        <div className="InputItem" >
          <h3>{this.props.description} - {this.props.name}</h3>
          <input ref={ this.props.name }
            value={this.state.value}
            className={inputClass}
            type="text"
            defaultValue={this.state.defaultValue}
            onChange={this.handleChange.bind(this)}
            onBlur={this.onBlurDecimal.bind(this)}
            />
          { this.state.loadHelpText ?
            <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
              <p className="InputItem-Warning" >
                <span className="InputItem-Warning-Text" >{this.state.helpText}</span>
              </p>
            </ReactCSSTransitionGroup>
          : null }
        </div>
      </ReactCSSTransitionGroup>
    )
  }
}
