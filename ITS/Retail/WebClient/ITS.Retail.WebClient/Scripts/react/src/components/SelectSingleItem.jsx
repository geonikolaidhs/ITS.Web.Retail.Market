import React, { Component }  from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import 'moment/locale/el';
import Locale from '../extensions/dist/Locale';
import InputItemsArray from '../extensions/dist/InputItemsArray';
import AlertDismissable from './AlertDismissable.jsx';
import Select from 'react-select';
import Ajax from '../extensions/dist/Ajax';

/**
 * Class Representing Input With Single Object Select Component
 * @extends React.Component
 */
 export default class SelectSingleItem extends React.Component {
   constructor(props) {
     super(props);
     this.state = {
       value: props.initialValue
     }
     this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
     this.getSearchResults = this.getSearchResults.bind(this);
     this.handleChange = this.handleChange.bind(this);
   }
   /**
    * componentWillMount - componentWillMount Event
    * Adds name and value on InputItemsArray on component mount
    */
   componentWillMount(){
     InputItemsArray.addItem({name: this.props.name,value: this.state.value},this.props.inputItems);
   }
   /**
    * getSearchResults - LoadOptions Async Select     *
    * @param  {Object}   input
    * @param  {Function} callback
    */
   getSearchResults (input, callback) {
     if(input !=null ){
       input = input.toLowerCase();

       let formData = {
         'typeStr': this.props.type,
         'searchFields': this.props.searchFields,
         'label': this.props.label,
         'searchTxt': input
       };

       Ajax.httpPost(this.props.urlSearchByType,formData,true,this).then(function(response) {

         const result = JSON.parse(response).result,
               dataArray = Array.isArray(result) ? result : [];
         var data = {
           options: dataArray.slice(0, 1000),
           complete: false
         };

         callback(null, data);

       }).catch(function(error) {
         callback(error);
       });
     }
   }
   /**
    * handleChange onchange event handler
    * @param  {Object} selectedValue
    */
   handleChange (selectedValue) {
     this.setState({ value:selectedValue });
     if( selectedValue !== null ){
       this.validateValue(selectedValue);
     }
     if(this.props.onChange !== undefined)
     {
       this.props.onChange(selectedValue);
     }
   }
   /**
    * validateValue Validate selectedValue and addItem to InputItemsArray
    * @param  {Object} selectedValue
    */
   validateValue(selectedValue){
     function addQuotes(value){
       return '\''+value+'\'';
     }
     if(selectedValue.length > 1){
       let arrayOfValues = [];
       for(let eachValue of selectedValue){
         arrayOfValues.push(eachValue.value);
       }
       selectedValue = arrayOfValues.join().split(',').join('\',\'');
       InputItemsArray.addItem({name: this.props.name,value: addQuotes(selectedValue)},this.props.inputItems);
     }
     else if(selectedValue.length === 1){
       InputItemsArray.addItem({name: this.props.name,value: addQuotes(selectedValue[0].value)},this.props.inputItems);
     }
   }
   /**
    * handleAlertDismiss onDismiss handler for Alert
    * Set null value on alertText to empty Alert Text
    */
   handleAlertDismiss() {
     this.setState({alertText: null});
   }
   /**
    * Render Select Item Component
    * @return {Object} React View
    */
   render() {
     return (
       <span>
         <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss}/>
         <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
           <div className="inputItem">
             <h3>{this.props.description}</h3>
             <Select.Async
                 loadOptions={this.getSearchResults}
                 value={this.state.value}
                 multi={false}
                 noResultsText= { Locale.getMessage(this.props.language,'noResultsFound') }
                 clearAllText= { Locale.getMessage(this.props.language,'clearAllText') }
                 clearValueText= { Locale.getMessage(this.props.language,'clearValueText') }
                 searchingText= { Locale.getMessage(this.props.language,'searchingText') }
                 searchPromptText= { Locale.getMessage(this.props.language,'searchPromptText') }
                 placeholder={ Locale.getMessage(this.props.language,'selectPlaceholder') }
                 onChange={this.handleChange }
                 cacheAsyncResults={false}
                 minimumInput = {3}
               />
           </div>
         </ReactCSSTransitionGroup>
       </span>
     );
   }
 }


SelectSingleItem.propTypes = {
  value: React.PropTypes.string
};

SelectSingleItem.defaultProps = {
  initialValue: null
};
