import React, { Component,PropTypes } from 'react';

import ReactCSSTransitionGroup from 'react-addons-css-transition-group';

import Select from 'react-select';

import Locale from '../extensions/dist/Locale';

import _ from 'underscore';

import Ajax from '../extensions/dist/Ajax';

import AlertDismissable from './AlertDismissable.jsx';

import DataViewSelect from './DataViewSelect.jsx';


/**
 * {InputItemsArray} inputItems - Variable to store selected input values
 */


/**
 * Class Representing Variable Values Display Component
 * @extends React.Component
 */
export default class VariableValuesDisplay extends React.Component{
  /**
   * Initialize this.state and props
   * @param  {Object} props - Props
   */
  constructor(props){
    super(props);
    this.state = {
      value: null,
      loadSelect: false,
      options: props.initialData,
      language: props.language,
      alertText: null,
      defaultDataView:null
    };
  }
  /**
   * componentDidMount Update value state and execute ajaxPost
   * @return {}
   */
  componentDidMount(){
    let initialValue = this.findDefaultKey(this.props.initialData);
    if(initialValue != null){
      this.setState({
        value: initialValue
      });
      let formData = {
        'categoryOid':initialValue
      };

      this.ajaxPost(this.props.urlGetDataView,formData,true,this);
    }
  }
  /**
   * findDefaultKey Helper function to check if isDefault index exists inside the initialArray or not
   * @param  {Array} initialArray - Our initialArray
   * @return {String}              null if no index is found or
   */
  findDefaultKey(initialArray){
    let defaultIndex = _.filter(initialArray, function (value, key) {
      if (value.isDefault === true ) {
        return true;
      }
    });
    if(typeof defaultIndex[0] != 'undefined'){
      return defaultIndex[0].value;
    }
    else{
      return null;
    }
  }
  /**
   * postAjax AjaxPost - Ajax post
   * @param  {String} url       [description]
   * @param  {Array} data      [description]
   * @param  {Boolean} async     [description]
   * @param  {Object} component [description]
   */
  ajaxPost(url,data,async,component) {
    Ajax.httpPost(url,data,async).then(function(response) {
      const result = JSON.parse(response).result,
            dataViewArray = Array.isArray(result) ? result : [],
            error = JSON.parse(response).error;

      if(dataViewArray.length == 0 ){
        this.setState({
          loadSelect: false,
          alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'warning'
        })
      }
      else{
        let defaultIndex = this.findDefaultKey(dataViewArray);
        this.setState({
          dataView: dataViewArray,
          loadSelect: true,
          defaultDataView: defaultIndex
        });
      }
    }.bind(component)).catch(function(error) {
      this.setState({
        alertText: Locale.getMessage(this.props.language,'serverError'),
        alertType: 'danger'
      });
    }.bind(component));
  }
  /**
   * Onchange Variable Values Display Select
   * Executes Ajax Post to GetDataView Method and updates Component state
   * @param  {string} value - Selected value
   */
  onChange(value) {
    this.setState({
      value:value,
      dataView: null,
      loadSelect: false
    });
    let formData = {
      'categoryOid':value.value
    };
    /** this is the specific scope
     */
    this.ajaxPost(this.props.urlGetDataView,formData,true,this);
  }
  handleAlertDismiss() {
    this.setState({alertText: null});
  }
  /**
   * Render Component
   * @return {Object} React View
   */
  render(){
    return (
      <section>
        <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss= {this.handleAlertDismiss.bind(this)} alertType={this.state.alertType}/>
        <div className="selectGroup">
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <h3 className="section-heading">{Locale.getMessage(this.props.language,'dataViewCategory')}</h3>
            <Select  className="section-select" ref="dataViewCategorySelect"
              placeholder={ Locale.getMessage(this.props.language,'select') }
              value= {this.state.value}
              options={this.state.options}
              onChange={this.onChange.bind(this)}
              noResultsText= { Locale.getMessage(this.props.language,'noResultsFound') }
              clearable= {false}
            />
          </ReactCSSTransitionGroup>
        </div>
        { this.state.loadSelect ?
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <DataViewSelect ref="dataViewSelect"
              title={ Locale.getMessage(this.props.language,'viewParameters')}
              placeholder={ Locale.getMessage(this.props.language,'select') }
              defaultDataView= {this.state.defaultDataView}
              noResultsText= { Locale.getMessage(this.props.language,'noResultsFound') }
              options={this.state.dataView}
              url={this.props.urlGetViewParameters}
              urlGetVariableValues= {this.props.urlGetVariableValues}
              urlSearchByType= {this.props.urlSearchByType}
              language={this.props.language}
            />
          </ReactCSSTransitionGroup>
        : null
        }
      </section>
    );
  }
}



VariableValuesDisplay.propTypes = {
  value: React.PropTypes.string,
  label: React.PropTypes.string,
  placeholder: React.PropTypes.string,
  noResultsText: React.PropTypes.string,
  options: React.PropTypes.array,
  onChange: React.PropTypes.func,
  titleDataView: React.PropTypes.string
};

VariableValuesDisplay.defaultProps = {
  initialValue: null
};
