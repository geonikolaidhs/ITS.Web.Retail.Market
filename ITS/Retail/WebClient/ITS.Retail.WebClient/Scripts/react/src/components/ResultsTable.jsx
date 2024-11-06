import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import { Table } from 'react-bootstrap';
import AlertDismissable from './AlertDismissable.jsx';

/**
 * Class Results Table
 * @extends React.Component
 */
export default class ResultsTable extends React.Component {
  /**
   * constructor
   * @param  {Object} props
   */
  constructor(props){
    super(props);
    this.state = {
      filteredDataList: this.props.results
    };
    this.onFilterChange = this.onFilterChange.bind(this);
  }
  /**
   * onSelectAll input event handler
   * @param  {Object} event
   * @param  {Object} allCheckedValues
   * @param  {Object} filePath
   */
  onSelectAll(event,allCheckedValues,filePath){
      let documentOids = allCheckedValues.map(element => element.DocumentHeaderOid);
      console.log(documentOids);
      let allSelectedInputs = ReactDOM.findDOMNode(this).getElementsByTagName('input');

      let inputSelectAll = $(allSelectedInputs).filter(function(){return $(this).hasClass('isSelectAll') });
      var isChecked = $(inputSelectAll).is(':checked');

      $(allSelectedInputs).each(function(){
        if(!$(this).hasClass('isSelectAll') ){
          $(this).prop('checked', isChecked);
        }
      });
      if(event.target.checked){
        this.props.onSelectAll(documentOids,filePath);
      }
      else{
        this.props.onSelectAll([],filePath);
      }
  }
 

  onFilterChange(event, key){
    if(!event.target.value){
      this.setState({filteredDataList: this.props.results});
    }
    var filterBy = event.target.value.toLowerCase();
    var size = this.props.results.length;
    var filteredList = [];
    for (var i=0; i<size; i++){
     switch(key){
      case 'Status': var v=this.props.results[i].Status; break; 
      case 'GrossTotal': var v=this.props.results[i].GrossTotal.toString(); break;
      case 'ExistingGrossTotal': var v=this.props.results[i].ExistingGrossTotal.toString(); break;
      case 'Difference': var v=this.props.results[i].Difference.toString(); break;
      case 'DocumentType': var v=this.props.results[i].DocumentType; break;
      case 'DocumentSeries': var v=this.props.results[i].DocumentSeries; break;
      case 'Customer': var v=this.props.results[i].Customer; break; 
      case 'DocumentNumber': var v=this.props.results[i].DocumentNumber.toString(); break;
      case 'FiscalDate': var v=this.props.results[i].Fiscaldate.toString(); break;
    }
            
      if (v.toLowerCase().indexOf(filterBy) !== -1){
        filteredList.push(this.props.results[i]);
        console.log(this.props.results[i].DocumentHeaderOid)    
      }
    }
    this.setState({filteredDataList: filteredList});
  }

  /**
   * Render ResultsTable Component
   * @return {Object} React View
   */
    render(){
    return(
      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
        <Table striped bordered condensed hover responsive>
          <thead>
            <tr>
              <th><input className="Checkbox-Checkbox isSelectAll" type="checkbox" onChange={event => this.onSelectAll(event,this.state.filteredDataList,this.props.filePath)}/></th>
              <th>Status</th>
              <th>GrossTotal</th>
              <th>ExistingGrossTotal</th>
              <th>Difference</th>
              <th>DocumentType</th>
              <th>DocumentSeries</th>
              <th>Customer</th>
              <th>DocumentNumber</th>
              <th>FiscalDate</th>
            </tr>
            <tr>
            <th></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'Status')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'GrossTotal')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'ExistingGrossTotal')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'Difference')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'DocumentTypes')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'DocumentSeries')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'Customer')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'DocumentNumber')}/></th>
            <th><input className="search" value={this.state.value} type="text" onChange={event => this.onFilterChange(event, 'FiscalDate')}/></th>
            </tr>
          </thead>
          <tbody>
          {(this.state.filteredDataList).map(results => {
              const key = results.DocumentHeaderOid;
              return(
                <tr key={key}>
                  <td>
                    <input className="Checkbox-Checkbox" type="checkbox" onChange={event => this.props.onChange(event,key,this.props.filePath)}/>
                  </td>
                  <td>{results.Status}</td>
                  <td>{results.GrossTotal}</td>
                  <td>{results.ExistingGrossTotal}</td>
                  <td>{results.Difference}</td>
                  <td>{results.DocumentType}</td>
                  <td>{results.DocumentSeries}</td>
                  <td>{results.Customer}</td>
                  <td>{results.DocumentNumber}</td>
                  <td>{results.FiscalDate}</td>
                </tr>
              );
            })}
          </tbody>
        </Table>
      </ReactCSSTransitionGroup>
    )
  }
}
