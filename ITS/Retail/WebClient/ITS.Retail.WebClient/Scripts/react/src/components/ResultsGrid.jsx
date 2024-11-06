import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';
import Ajax from '../extensions/dist/Ajax';
import { Button,Glyphicon } from 'react-bootstrap';
import AlertDismissable from './AlertDismissable.jsx';
import ResultsTable from './ResultsTable.jsx';

/**
 * Class Result Grid
 * @extends React.Component
 */
export default class ResultsGrid extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      submitting:false,
      selectedValues:[],
      alertText:'',
      alertType:'',
      alertStyle:'',
      dismissAfter:5000
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleClick = this.handleClick.bind(this);
    this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
    this.handleSelectAll = this.handleSelectAll.bind(this);
  }
  /**
   * handleClick Button onClick event handler
   * @param  {Object} element
   */
  handleClick(element){

    element.preventDefault();

    this.setState({submitting: true,alertStyle:'',dismissAfter:5000});

    let data = {};
    data.selectedFiles = JSON.stringify(this.state.selectedValues);

    Ajax.httpPost(this.props.urlGetPOSTransactionFiles,data,true).then(function(response) {
      const result = JSON.parse(response),
            error = JSON.parse(response).error;

      this.setState({submitting: false});

      if(error){
        this.setState({
          alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'warning',
          dismissAfter: 30000000,
          alertStyle:  {
            wordWrap: 'break-word',
            right: 0,
            top: '10px',
            position: 'relative'
          }
        })
      }
      else{
        this.setState({
          alertText: typeof result.result != 'undefined' ? result.result : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'success'
        });
      }
    }.bind(this)).catch(function(error) {
      this.setState({
        submitting: false,
        alertText: Locale.getMessage(this.props.language,'serverError'),
        alertType: 'danger'
      });
    }.bind(this));
  }
  /**
   * handleChange ResultsTable onChange event handler
   * @param  {Object} event
   * @param  {Object} selectedValue
   * @param  {Object} filePath
   */
  handleChange(event,selectedValue,filePath){

    let newSelectedValues = this.state.selectedValues.slice();

    if(event.target.checked){
      newSelectedValues.push({file:filePath,oid:selectedValue});
      this.setState({selectedValues:newSelectedValues});
    }
    else{
        let finalSelectedValues = this.state.selectedValues;
        finalSelectedValues = finalSelectedValues.filter(item => item.oid !== selectedValue);
        this.setState({selectedValues:finalSelectedValues});
    }
  }
  /**
   * handleSelectAll ResultsTable onSelectAll event handler
   * @param  {Object} values
   * @param  {Object} filePath
   */
  handleSelectAll(values,filePath){
    let finalSelectedValues = this.state.selectedValues;
    finalSelectedValues = finalSelectedValues.filter(item => item.file !== filePath);

    if(values.length > 0){
      finalSelectedValues = finalSelectedValues.slice();
      values.forEach(value =>{
        finalSelectedValues.push({file:filePath,oid: value})
      });
    }
    this.setState({selectedValues:finalSelectedValues});
  }
  /**
   * handleAlertDismiss onDismiss handler for Alert
   * Set null value on alertText to empty Alert Text
   */
  handleAlertDismiss() {
    this.setState({alertText: null});
  }
  /**
   * Render ResultsGrid Component
   * @return {Object} React View
   */
  render(){
    return (
      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>

        <section>
          <br/>
          <h3 className="TitleHeader is-Sub">{Locale.getMessage(this.props.language,'selectFromDocuments')}</h3>
          {(JSON.parse(this.props.results)).map(results => {
              return (
                <div key={results.Filepath}>
                  <h3>{Locale.getMessage(this.props.language,'fileName')}: <small>'{results.Filepath}'</small></h3>
                  <ResultsTable
                    filePath={results.Filepath}
                    results={results.POSFileDocumentHeaders}
                    onChange={this.handleChange}
                    onSelectAll={this.handleSelectAll}
                    >
                  </ResultsTable>
                </div>
              );
          })}

          <Button  onClick={this.handleClick} type="submit" bsStyle="primary" bsSize="lg"  disabled={this.state.submitting} >
            { Locale.getMessage(this.props.language,'execute') }
            { this.state.submitting ? <Glyphicon glyph="refresh" /> : <Glyphicon glyph="menu-right" /> }
          </Button>
        </section>
        <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss} alertType={this.state.alertType} dismissAfter={this.state.dismissAfter} isFull={true} hasStyle={this.state.alertStyle}/>
      </ReactCSSTransitionGroup>
    );
  }
}
