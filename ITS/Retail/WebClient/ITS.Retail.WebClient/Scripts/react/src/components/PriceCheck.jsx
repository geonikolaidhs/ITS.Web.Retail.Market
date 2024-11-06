import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Select from 'react-select';
import Locale from '../extensions/dist/Locale';
import InputItemsArray from '../extensions/dist/InputItemsArray';
import SelectSingleItem from './SelectSingleItem.jsx';
import Ajax from '../extensions/dist/Ajax';
import { Button,Glyphicon } from 'react-bootstrap';
import AlertDismissable from './AlertDismissable.jsx';
import InputItem from './InputItem.jsx';
import PriceCheckResult from './PriceCheckResult'

/**
 * {InputItemsArray} inputCustomerItems - Variable to store selected input values
 */
let inputCustomerItems = new InputItemsArray();
let inputBarcodeItems = new InputItemsArray();

/**
 * Class Representing Price Check
 * @extends React.Component
 */
export default class PriceCheck extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      submitting:false,
      language:props.language,
      alertText:'',
      alertType:'',
      store: props.storeValue,
      customer: props.defaultCustomer,
      itembarcode: props.itemBarcode,
      priceresult: null,
      loadResults: false
    };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
    this.storeChanged = this.storeChanged.bind(this);
    this.customerChanged = this.customerChanged.bind(this);
    this.itemBarcodeChanged = this.itemBarcodeChanged.bind(this);
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
      const result = JSON.parse(response),
            error = JSON.parse(response).error;
            this.setState({customer: { label: result.label , value: result.value},
                          loadResults: false
            });
    }.bind(component)).catch(function(error) {
      this.setState({
        alertText: Locale.getMessage(this.props.language,'serverError'),
        alertType: 'danger'
      });
    }.bind(component));
  }

  // /**
  //  * componentDidMount Update value state and execute ajaxPost
  //  * @return {}
  //  */
  // componentDidMount(){
  //   let formData = {
  //     'store': this.state.store.value
  //   };
  //   this.ajaxPost(this.props.urlGetDefaultCustomer,formData,true,this);
  // }

  storeChanged(selectedStore){
    this.setState({store: selectedStore});

    // let data = {
    //   'store': this.state.store.value
    // };

    // this.ajaxPost(this.props.urlGetDefaultCustomer,data,true,this);
  }

  customerChanged(selectedCustomer){
    if( selectedCustomer === null ){
      selectedCustomer = { label: '' , value: '00000000-0000-0000-0000-000000000000' };
    }
    this.setState({customer: selectedCustomer});
  }

  itemBarcodeChanged(selectedItemBarcode){
    this.setState({itembarcode: selectedItemBarcode});
  }

  /**
   * handleSubmit onSubmit Form event handler
   * @param  {Object} element Form Submit Element
   */
  handleSubmit(element){
    element.preventDefault();

    this.setState({submitting: true});
    let data = {
      'store': this.state.store.value,
      'customer': this.state.customer === null ? '00000000-0000-0000-0000-000000000000' : this.state.customer.value,
      'itembarcode': this.state.itembarcode === null ? '00000000-0000-0000-0000-000000000000': this.state.itembarcode.value
    };

    Ajax.httpPost(this.props.urlGetPrice,data,true).then(function(response) {
      const result = JSON.parse(response),
            error = JSON.parse(response).ApplicationError;

      this.setState({submitting: false,
                    priceresult: result,
                    loadResults: true
      });

      if(error !== ''){
        this.setState({
          alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'warning',
          loadResults: false
        });
        setJSError(error);
      }
    }.bind(this)).catch(function(error) {
      this.setState({
        alertText: Locale.getMessage(this.props.language,'serverError'),
        alertType: 'danger'
      });
    }.bind(this));

  }
  /**
   * handleAlertDismiss onDismiss handler for Alert
   * Set null value on alertText to empty Alert Text
   */
  handleAlertDismiss() {
    this.setState({alertText: null});
  }
  /**
   * Render PriceCheck Component
   * @return {Object} React View
   */
  render(){
    return (
      <section>
        <form onSubmit={this.handleSubmit}>
            <div className="inputGroup">
               <div className="inputItems">
                 <h3 className="section-heading">{Locale.getMessage(this.props.language,'store')}</h3>
                 <Select  className="section-select"
                   placeholder={ Locale.getMessage(this.props.language,'store') }
                   value= {this.state.store.value}
                   options={this.props.stores}
                   noResultsText= { Locale.getMessage(this.props.language,'noResultsFound') }
                   clearable= {false}
                   onChange={this.storeChanged.bind(this)}
                 />
                 <SelectSingleItem ref="selectSingleItemCustomer"
                   id="Customer"
                   name="Customer"
                   type="Customer"
                   initialValue = {this.state.customer.value}
                   description={ Locale.getMessage(this.props.language,'customer') }
                   searchFields = "CompanyName,Code,Trader.TaxCode"
                   label="FullDescription"
                   urlSearchByType={this.props.urlSearchByType}
                   language={this.props.language}
                   inputItems={inputCustomerItems}
                   value={this.state.customer.value}
                   onChange={this.customerChanged.bind(this)}
                 />
                 <SelectSingleItem
                   id="Code"
                   name="Code"
                   type="ItemBarcode"
                   description={ Locale.getMessage(this.props.language,'item') }
                   searchFields = "Item.Code,Barcode.Code,Item.Name"
                   label="FullDescription"
                   urlSearchByType={this.props.urlSearchByType}
                   language={this.props.language}
                   inputItems={inputBarcodeItems}
                   onChange={this.itemBarcodeChanged}
                 />
               </div>
               <div className="buttonItem">
                  <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                    <Button type="submit" bsStyle="primary" bsSize="lg"  disabled={this.state.submitting}>
                      { Locale.getMessage(this.props.language,'searchingText') }
                      { this.state.submitting ? <Glyphicon glyph="refresh" /> : <Glyphicon glyph="menu-right" /> }
                    </Button>
                  </ReactCSSTransitionGroup>
              </div>
            </div>
        </form>
        { this.state.loadResults && this.state.priceresult !== null ?
          <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
          <section>
            <PriceCheckResult
              key = {this.state.priceresult.key}
              language={this.props.language}
              store = {this.state.priceresult.store}
              storePriceCatalogPolicy={this.state.priceresult.storePriceCatalogPolicy}
              customer= {this.state.priceresult.customer}
              customerPriceCatalogPolicy={this.state.priceresult.customerPriceCatalogPolicy}
              item = {this.state.priceresult.item}
              price= {this.state.priceresult.price}
              vatIncluded= {this.state.priceresult.vatIncluded}
              priceCatalog= {this.state.priceresult.priceCatalog}
              trace= { this.state.priceresult.trace }
              >
            </PriceCheckResult>
        </section>
        </ReactCSSTransitionGroup>
          : null
        }
        <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss} alertType={this.state.alertType}/>
    </section>
    );
  }

}


PriceCheck.propTypes = {
  language:React.PropTypes.string,
  submitting: React.PropTypes.bool,
  handleSubmit: React.PropTypes.func
};

PriceCheck.defaultProps = {
  initialValue: null
};
