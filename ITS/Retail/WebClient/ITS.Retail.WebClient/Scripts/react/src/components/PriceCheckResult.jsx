import React, { Component,PropTypes } from 'react';
import Locale from '../extensions/dist/Locale';
import { Table,Label } from 'react-bootstrap';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';

/**
 * Class Representing Price Check Result
 * @extends React.Component
 */
export default class PriceCheckResult extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      language:props.language,
      store: props.store,
      storePriceCatalogPolicy: props.storePriceCatalogPolicy,
      customer: props.customer,
      customerPriceCatalogPolicy: props.customerPriceCatalogPolicy,
      item: props.item,
      price: props.price,
      vatIncluded: props.vatIncluded,
      priceCatalog: props.priceCatalog,
      trace: props.trace
    };
  }
  render(){
    return (
      <div className="inputGroup">
         <div className="inputItems">
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <h3>{ Locale.getMessage(this.state.language,'store') }</h3>
            <h4><Label>{ Locale.getMessage(this.state.language,'store') } : { this.state.store }</Label></h4>
            <h4><Label>{ Locale.getMessage(this.state.language,'storePriceCatalogPolicy') } : { this.state.storePriceCatalogPolicy }</Label></h4>
          </ReactCSSTransitionGroup>
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <h3>{ Locale.getMessage(this.state.language,'customer') }</h3>
              <h4><Label>{ Locale.getMessage(this.state.language,'customer') } : { this.state.customer }</Label></h4>
              <h4><Label>{ Locale.getMessage(this.state.language,'customerPriceCatalogPolicy') } : { this.state.customerPriceCatalogPolicy }</Label></h4>
          </ReactCSSTransitionGroup>
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <h3>{ Locale.getMessage(this.state.language,'item') } : { this.state.item }</h3>
            <h3>{ Locale.getMessage(this.state.language,'price') }</h3>
            <Table striped bordered condensed hover responsive>
              <thead>
                <tr>
                  <th>{ Locale.getMessage(this.state.language,'priceCatalog') }</th>
                  <th style={ {backgroundColor: 'navy', fontWeight: 'bold', color:'white'} }>{ Locale.getMessage(this.state.language,'price') }</th>
                  <th>{ Locale.getMessage(this.state.language,'vatIncluded') }</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>{ this.state.priceCatalog }</td>
                  <td style={ {backgroundColor: 'navy', fontWeight: 'bold', color:'white'} }>{ this.state.price }</td>
                  <td>{ this.state.vatIncluded }</td>
                </tr>
              </tbody>
            </Table>
          </ReactCSSTransitionGroup>
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
            <h3>{ Locale.getMessage(this.state.language,'searchDetails') }</h3>
            <Table striped bordered condensed hover responsive>
              <thead>
                <tr>
                  <th>{ Locale.getMessage(this.state.language,'searchOrder') }</th>
                  <th>{ Locale.getMessage(this.state.language,'priceCatalog') }</th>
                  <th>{ Locale.getMessage(this.state.language,'searchMethod') }</th>
                </tr>
              </thead>
              <tbody>
                {(this.props.trace).map(trace => {
                  return(
                    <tr key={trace.Number}>
                      <td>{trace.Number}</td>
                      <td>{trace.PriceCatalogDescription}</td>
                      <td>{trace.SearchMethod}</td>
                    </tr>
                  );
                })}
              </tbody>
            </Table>
          </ReactCSSTransitionGroup>
      </div>
    </div>
    );
  }
}
