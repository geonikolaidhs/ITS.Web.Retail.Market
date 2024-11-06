import React, { Component }  from 'react';
import ReactCSSTransitionGroup from  'react-addons-css-transition-group';
import DayPicker, { DateUtils } from 'react-day-picker';
import moment from 'moment';
import LocaleUtils from 'react-day-picker/moment';
import 'moment/locale/el';
import Locale from '../extensions/dist/Locale';

/**
 * Class Representing Input With DatePicker Component where datetime may be empty
 * @extends React.Component
 */
export default class InputDatePickerWithOptionalValue extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      value: moment().format('L'), // The value of the input field
      month: new Date(), // The month to display in the calendar
      loadHelpText: false,
      helpText: Locale.getMessage(this.props.language,'addValidDate'),
      displayDatePicker: false
    }
  }
  /**
   * Input Change handler when onDayClick event is fired
   * @param  {Object} e OnDayCLick Event
   */
  handleInputChange(e) {
    const { value } = e.target;
    /**
     * Change the current month only if the value entered by the user is a valid
     * date, according to the `L` format
     */
    if (moment(value, 'L', true).isValid()) {
      this.setState({
        loadHelpText: false,
        month: moment(value, 'L').toDate(),
        value
      }, this.showCurrentDate);
      this.props.onDayChange(value);
    }
    else if ( value === '' ){
      var minimumValue = '01/01/0001';
      this.setState( { loadHelpText: false,
                       month: moment(minimumValue,'L').toDate(),
                       value: minimumValue,
                       displayDatePicker: false
      });
      this.props.onDayChange(minimumValue);
    }
    else {
      this.setState({ value }, this.showCurrentDate);
      this.setState({
        loadHelpText: true
      })
    }
  }
  /**
   * Day Click handler when onDayClick event is fired
   * @param  {Object} e - OnDayCLick Event
   * @param  {Object} day - selectedDay
   */
  handleDayClick(e, day) {
    let selectedDay = moment(day).format('L');
    this.setState({
      value: selectedDay,
      month: day,
      loadHelpText: false
    });
    this.props.onDayChange(selectedDay);
  }
  /**
   * Shows Current Month when Input onfocus event is fired
   */
  showCurrentDate() {
    this.refs.daypicker.showMonth(this.state.month);
    this.setState({
      displayDatePicker: true
    });
  }
  /**
   * Toggles DatePicker show or hide when icon onclick event is fired
   * @param  {Object} e - OnClick event
   */
  toggleDatePicker(e){
    if(this.state.displayDatePicker){
      this.setState({
        displayDatePicker:false
      })
    }
    else{
      this.setState({
        displayDatePicker:true
      })
    }
  }
  /**
   * Render Input Item Component
   * @return {Object} React View
   */
  render() {
    const selectedDay = moment(this.state.value, 'L', true).toDate();

    let inputClass = 'InputItem-Input InputItem-Input--glow InputItem-Input--calendar',

        dayPickerClass = 'is-hidden',

        calendarClass = 'fa fa-calendar InputItem-Calendar';

    if (this.state.loadHelpText) inputClass += ' is-Empty';

    calendarClass = this.state.displayDatePicker ? 'fa fa-calendar-times-o fa-lg InputItem-Calendar' : 'fa fa-calendar fa-lg InputItem-Calendar';

    dayPickerClass = this.state.displayDatePicker ? 'is-visible' : 'is-hidden';

    return (
      <div className="InputItem">
        <h3>{this.props.caption}</h3>
        <span onClick={this.toggleDatePicker.bind(this)}><i className={calendarClass} ></i></span>
        <input
          ref="input"
          type="text"
          placeholder="DD-MM-YYYY"
          value={ this.state.value }
          className={inputClass}
          onChange={ this.handleInputChange.bind(this) }
          onFocus={ this.showCurrentDate.bind(this) }
          />

        <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
          <DayPicker
            ref="daypicker"
            className={dayPickerClass}
            localeUtils={ LocaleUtils } locale={this.props.language}
            initialMonth={ this.state.month }
            modifiers={{
              selected: day => DateUtils.isSameDay(selectedDay, day)
            }}
            onDayClick={ this.handleDayClick.bind(this) }
          />
        </ReactCSSTransitionGroup>

        { this.state.loadHelpText ?
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <p className="InputItem-Warning" >
              <span className="InputItem-Warning-Text" >{this.state.helpText}</span>
            </p>
          </ReactCSSTransitionGroup>
        : null }

      </div>

    );
  }
}
