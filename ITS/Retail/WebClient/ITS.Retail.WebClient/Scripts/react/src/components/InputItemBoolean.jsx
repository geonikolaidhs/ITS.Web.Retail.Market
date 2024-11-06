import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';

import InputItemsArray from '../extensions/dist/InputItemsArray';

/**
 * Class Representing Input Item Component
 * @extends React.Component
 */
export default class InputItemBoolean extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      value: false,
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
   * handleChangeCheckbox event handler is fired on change
   * @param  {Object} event - handleChangeCheckbox event
   */
  handleChangeCheckbox(event){
    this.setState({
      checked:event.target.checked
    })
    InputItemsArray.addItem({name: this.props.name,value: event.target.checked},this.props.inputItems);
  }
  /**
   * Render Input Item Component
   * @return {Object} React View
   */
  render() {
    return(
      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
        <div className="InputItem is-Checkbox">
          <input ref={ this.props.name }
            checked={this.state.checked}
            className="InputItem-Input InputItem-Input--checkbox"
            type="checkbox"
            onChange={this.handleChangeCheckbox.bind(this)}
            />
          <label>{this.props.description} - {this.props.name}</label>
        </div>
      </ReactCSSTransitionGroup>
    )
  }
}
