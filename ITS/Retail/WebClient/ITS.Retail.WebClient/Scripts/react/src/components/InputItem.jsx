import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';

import InputItemsArray from '../extensions/dist/InputItemsArray';

/**
 * Class Representing Input Item Component
 * @extends React.Component
 */
export default class InputItem extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      value: '',
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
   * @param  {string} type - Input type Possible values 'String' || 'Decimal'
   * @param  {Object} event - handleChange event
   */
  handleChange(type,event) {
    this.setState({
      value: event.target.value,
      loadHelpText: false,
      helpText: ''
    });
    this.validateString(event);
  }
  /**
   * onBlurString event handler is fired on input blur when type is String
   * @param  {Object} event - onBlurString event
   */
  onBlurString(event){
    this.validateString(event);
  }
  /**
   * validateString String Validation - Check if String is not empty and then add the item into InputItemsArray
   * @param  {Object} event - validateString event
   */
  validateString(event){
    //InputItemsArray.addItem({name: this.props.name,value: this.state.value},inputItems);
    if(event.target.value.length === 0 ){
      this.setState({
        loadHelpText: true,
        helpText: Locale.getMessage(this.props.language,'addValidValue')
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
  render() {
    let inputClass = 'InputItem-Input InputItem-Input--glow';
    if (this.state.loadHelpText) inputClass += ' is-Empty';
    return(
      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
        <div className="InputItem">
          <h3>{this.props.description} - {this.props.name}</h3>
          <input ref={ this.props.name }
            value={this.state.value}
            className={inputClass}
            type="text"
            defaultValue={this.state.defaultValue}
            onChange={this.handleChange.bind(this,'String')}
            onBlur={this.onBlurString.bind(this)}
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
