import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import 'moment/locale/el';
import Locale from '../extensions/dist/Locale';
import InputItemsArray from '../extensions/dist/InputItemsArray';
import AlertDismissable from './AlertDismissable.jsx';
import SelectItem from './SelectItem.jsx';
import Ajax from '../extensions/dist/Ajax';
import Griddle from 'griddle-react';
import Select from 'react-select';
import { Button,Glyphicon } from 'react-bootstrap';
import InputItemDate from './InputItemDate.jsx';
import InputItemDecimal from './InputItemDecimal.jsx';
import InputItemBoolean from './InputItemBoolean.jsx';
import InputItem from './InputItem.jsx';
import _ from 'underscore';

let inputItems = new InputItemsArray();

 /**
  * Class representing Data View Select Component
  * @extends React.Component
  */
 export default class DataViewSelect extends React.Component{
   /**
    * Initialize props and this.state
    * @param  { Object } props - props
    */
   constructor(props){
     super(props);
     this.state = {
       value: props.defaultDataView,
       displayGrid: false,
     }
   }
   componentDidMount(){
     if(this.props.defaultDataView != null){

       let formData = {
         'dataViewOid': this.props.defaultDataView
       };

       /**
        * Empty inputItems
        */
       inputItems = new InputItemsArray();

       /**
        * Make the Ajax Post Call
        */
       this.ajaxPost(this.props.url,formData,true,this);
     }
   }
   /**
    * onChange event handler is executed when onChange event is fired
    * @param  {Object} value - Selected Value
    */
   onChange(value){
     let formData = {
       'dataViewOid': value.value
     };

     this.setState({ value });
     /**
      * Empty inputItems
      */
     inputItems = new InputItemsArray();

     /**
      * Make the Ajax Post Call
      */
     this.ajaxPost(this.props.url,formData,true,this);

   }
   /**
    * ajaxPost Ajax Post Method
    * @param  {String} url       Url
    * @param  {Object} data      Data to be posted
    * @param  {Boolean} async    asyncronous call
    * @param  {Object} component Component Object
    */
   ajaxPost(url,data,async,component) {
     Ajax.httpPost(url,data,async).then(function(response) {
       const result = JSON.parse(response).result,
             viewParametersArray = Array.isArray(result) ? result : [],
             error = JSON.parse(response).error;

       if(typeof error != 'undefined' ){
         this.setState({
           displayInputLists: false,
           displayGrid: false,
           alertText: error,
           alertType: 'danger'
         })
       }
       else{
         this.setState({
           inputData: viewParametersArray,
           displayInputLists: true,
           displayGrid: false
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
    * handleButtonClick event is fired when Submit Button is clicked
    * @param  {Object} element Button Element
    */
   handleButtonClick(element){
     element.preventDefault();
     let formData = {},
         emptyValueError = false;
     formData.customDataViewOid = typeof this.state.value.value != 'undefined' ? this.state.value.value : this.state.value;

     /**
      * if statement to check if selectedItemsArray has at least one item and add this value to gridOids
      * @param  {number} selectedItemsArray.length>0 - selectedItemsArray
      */
     if(selectedItemsArray.length>0){
       let selectedValueString = '\''+selectedItemsArray[0]+'\'';

       _.each(_.without(selectedItemsArray, selectedItemsArray[0]),function(element){
         selectedValueString =  selectedValueString +'\,\''+element+'\'';
       });

       formData.gridOids = selectedValueString;
     }


     if(this.state.hasParameters){
         _.each(inputItems, function(element, key){
           if(element.value == null){
             element.value = '';
           }
           if(element.value.length === 0 ){
             emptyValueError = true;
           }
       });
     }

     if(emptyValueError){
       this.setState({
         displayGrid: false,
         alertText: Locale.getMessage(this.props.language,'addNonEmptyValue'),
         alertType:'warning'
       });
       return null;
     }

     formData.paramValues = JSON.stringify(inputItems);
     this.ajaxPostButtonClick(this.props.urlGetVariableValues,formData,true,this);
   }
   /**
    * ajaxPostButtonClick Ajax Post Button Click Method
    * @param  {String} url       Url
    * @param  {Object} data      Data to be posted
    * @param  {Boolean} async    asyncronous call
    * @param  {Object} component Component Object
    */
   ajaxPostButtonClick(url,data,async,component){
     Ajax.httpPost(url,data,async).then(function(response) {
       const result = JSON.parse(response).result,
             gridArray = Array.isArray(result) ? result : [],
             error = JSON.parse(response).error;
         if(gridArray.length == 0 ){
           this.setState({
             displayGrid: false,
             alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
             alertType:'warning'
           })
         }
         else{
           component.setState({
             displayGrid: true,
             gridData: gridArray,
             alertText: Locale.getMessage(this.props.language,'found')+' '+gridArray.length + ' '+ Locale.getMessage(this.props.language,'items'),
             alertType: 'info'
           })
         }
     }.bind(component)).catch(function(error) {
       this.setState({
         alertText: Locale.getMessage(this.props.language,'serverError'),
         alertType: 'danger'
       });
     }.bind(component));
   }
   /**
    * handleAlertDismiss onDismiss handler for Alert
    * Set null value on alertText to empty Alert Text
    */
   handleAlertDismiss() {
     this.setState({alertText: null});
   }
   /**
    * Render Component
    * @return {Object} React View
    */
   render(){
     let results = null,
         buttonClass = null;
     if(typeof this.state.displayInputLists != 'undefined' && this.state.displayInputLists){
       if(this.state.inputData.length>0){
         this.state.hasParameters = true;
         results = this.state.inputData.map(function (input, index) {
           input.name = input.name.replace(/\{|\}/gi,'');

           switch (input.type) {
             case 'Boolean':
               return(
                 <InputItemBoolean key={index} id={index} name={input.name} type={input.type} description={input.description} language={this.props.language} inputItems={inputItems}/>
               )
               break;
             case 'Decimal':
               return(
                 <InputItemDecimal key={index} id={index} name={input.name} type={input.type} description={input.description} language={this.props.language} inputItems={inputItems}/>
               )
               break;
             case 'String':
               return(
                 <InputItem key={index} id={index} name={input.name} type={input.type} description={input.description} language={this.props.language} inputItems={inputItems}/>
               )
               break;
             case 'DateTime':
               return(
                 <InputItemDate key={index} id={index} name={input.name} type={input.type} description={input.description} language={this.props.language} inputItems={inputItems}/>
               )
               break;
             default:
               return(
                 <SelectItem
                   key={index}
                   id={index}
                   name={input.name}
                   type={input.type}
                   description={input.description}
                   urlSearchByType={this.props.urlSearchByType}
                   language={this.props.language}
                   inputItems={inputItems}
                 />
               )
           }
         }, this);
         buttonClass = 'u-sm-margin-top';
       }
       else{
         this.state.hasParameters = false;
       }
     }
     return(
       <form onSubmit={this.handleButtonClick.bind(this)}>
         <div className="selectGroup">
           <h3 className="section-heading">{this.props.title}</h3>
           <Select className="section-select"
             placeholder={ Locale.getMessage(this.props.language,'select') }
             value= {this.state.value}
             onChange={this.onChange.bind(this)}
             noResultsText= { Locale.getMessage(this.props.language,'noResultsFound') }
             options={this.props.options}
             url={this.state.url}
             display={this.props.display}
             clearable= { false }
           />
         </div>

         { this.state.displayInputLists ?
             <div className="inputGroup">
                <div className="inputItems">
                  { results }
                </div>
                <div className="buttonItem">
                   <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                     <Button type="button" className={buttonClass} bsStyle="primary" bsSize="lg" onClick={this.handleButtonClick.bind(this)} url={this.props.urlGetVariableValues} >
                       { Locale.getMessage(this.props.language,'show') }
                       <Glyphicon glyph="search" />
                     </Button>
                   </ReactCSSTransitionGroup>
               </div>
             </div>
          : null}

          {this.state.displayGrid ?
            <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
             <Griddle results={this.state.gridData}
               showFilter={true}
               showSettings={true}
               resultsPerPage={10}
               settingsText= { Locale.getMessage(this.props.language,'settings') }
               filterPlaceholderText= { Locale.getMessage(this.props.language,'filterPlaceholder')}
               nextText= { Locale.getMessage(this.props.language,'next')}
               previousText= { Locale.getMessage(this.props.language,'previous')}
               maxRowsText= { Locale.getMessage(this.props.language,'rowsPerPage')}
               noDataMessage = {Locale.getMessage(this.props.language, 'noDataMessage')}
               sortAscendingComponent={<span className="fa fa-sort-alpha-asc"></span>}
               sortDescendingComponent={<span className="fa fa-sort-alpha-desc"></span>}
               settingsIconComponent = {<span className="fa fa-cogs u-sm-padding-left"></span>}
               previousIconComponent = {<span className="fa fa-chevron-left u-sm-padding-right"></span>}
               nextIconComponent = {<span className="fa fa-chevron-right u-sm-padding-left"></span>}
               />
           </ReactCSSTransitionGroup>
         : null}
         <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss.bind(this)} alertType={this.state.alertType}/>
       </form>
     )
   }
 }

 DataViewSelect.propTypes = {
   label: React.PropTypes.string,
   placeholder: React.PropTypes.string,
   noResultsText: React.PropTypes.string,
   options: React.PropTypes.array,
   onChange: React.PropTypes.func,
   url: React.PropTypes.string,
   urlGetVariableValues:  React.PropTypes.string,
   defaultDataView:React.PropTypes.string
 };
