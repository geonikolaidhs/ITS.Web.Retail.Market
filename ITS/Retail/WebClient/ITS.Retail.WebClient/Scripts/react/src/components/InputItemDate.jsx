import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import 'moment/locale/el';
import moment from 'moment';
import InputItemsArray from '../extensions/dist/InputItemsArray';
import InputWithDatePicker from './InputWithDatePicker.jsx';

/**
 * Class Representing Input Item Date
 * @extends React.Component
 */
export default class InputItemDate extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      value: moment().format('L'),
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
   * onDayChange event handler is fired whenever Day Change event is fired
   * @param  {Object} value - Day Change Value
   */
  onDayChange(value){
    InputItemsArray.addItem({
      name: this.props.name,
      value: value
    }, this.props.inputItems);
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
        <InputWithDatePicker
          language={this.props.language}
          onDayChange={this.onDayChange.bind(this)}
          ref={ this.props.name }
          name = {this.props.name}
          description = {this.props.description}
        />
      </ReactCSSTransitionGroup>
    )
  }
}
