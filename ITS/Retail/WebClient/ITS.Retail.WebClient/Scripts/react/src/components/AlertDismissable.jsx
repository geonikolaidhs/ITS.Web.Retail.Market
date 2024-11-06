import React, { Component }  from 'react';
var ReactCSSTransitionGroup = require('react-addons-css-transition-group');
import 'moment/locale/el';
import Locale from '../extensions/dist/Locale';
import { Alert } from 'react-bootstrap';

/**
 * Class Representing Input With DatePicker Component
 * @extends React.Component
 */
export default class AlertDismissable extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      alertText: props.initialValue,
      alertType: props.initialAlertText,
      dismissAfter: props.dismissAfter,
      hasStyle: props.hasStyle
    }
  }
  /**
   * invoked when component is receiving props, not for initial 'render'
   * @param  {nextProps} nextProps - received Props
   */
  componentWillReceiveProps(nextProps) {
    if(nextProps.alertType){
      this.state.alertType = nextProps.alertType;
    }

    this.state.dismissAfter = nextProps.dismissAfter ? nextProps.dismissAfter :5000;
    this.state.hasStyle = nextProps.hasStyle ? nextProps.hasStyle : null;

  }

  /**
   * handleAlertDismiss onDismiss handler for Alert
   * Set null value on alertText to empty Alert Text
   */
  handleAlertDismiss() {
    this.props.handleAlertDismiss(this);
  }

  /**
   * Render AlertDismissable Component
   * @return {Object} React View
   */
  render() {
    return (
      <div>
        { this.props.alertText ?
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <Alert bsStyle={this.state.alertType} onDismiss={this.handleAlertDismiss.bind(this)} dismissAfter={this.state.dismissAfter} style={this.state.hasStyle}>
              <h4>{ this.props.alertText }</h4>
            </Alert>
          </ReactCSSTransitionGroup>
          : null
        }
      </div>
    );
  }
}


AlertDismissable.propTypes = {
  alertText: React.PropTypes.string,
  alertType: React.PropTypes.string
};

AlertDismissable.defaultProps = {
  initialValue: null,
  initialAlertText: 'danger'
};
