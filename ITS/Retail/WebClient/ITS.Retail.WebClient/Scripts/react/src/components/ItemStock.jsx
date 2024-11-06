import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import { Button,Glyphicon } from 'react-bootstrap';
import Ajax from '../extensions/dist/Ajax';
import Select from 'react-select';
import Locale from '../extensions/dist/Locale';
import InputItemsArray from '../extensions/dist/InputItemsArray';
import InputDatePickerWithOptionalValue from './InputDatePickerWithOptionalValue.jsx'
import AlertDismissable from './AlertDismissable.jsx';
import SelectSingleItem from './SelectSingleItem.jsx';


let inputItemBarcodes = new InputItemsArray();

/**
 * Class for Manual ItemStock reprocessing
 * @extends React.Component
 */
export default class ItemStock extends React.Component {
   /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      submitting:false,
      language:props.language,
      store: props.storeValue,
      itemBarcode: props.itemBarcode,
      fromDate : props.fromDate,
      alertText:'',
      alertType:''
    };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
    this.storeChanged = this.storeChanged.bind(this);
    this.itemBarcodeChanged = this.itemBarcodeChanged.bind(this);
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
      'fromDate': this.state.fromDate,
      'itemBarcode': this.state.itemBarcode === null ? '00000000-0000-0000-0000-000000000000': this.state.itemBarcode.value
    };

    Ajax.httpPost(this.props.urlRecalculateItemStock,data,true).then(function(response) {
      const result = JSON.parse(response),
            error = JSON.parse(response).ApplicationError;

      this.setState({submitting: false});

      if(error !== ''){
        this.setState({
          alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'warning'
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

  storeChanged(selectedStore){
    this.setState({store: selectedStore});
  };

  itemBarcodeChanged(selectedItemBarcode){
    this.setState({itemBarcode: selectedItemBarcode});
  };

  fromDateChanged(newFromDate){
    this.setState({fromDate: newFromDate});
  };

  /**
   * handleAlertDismiss onDismiss handler for Alert
   * Set null value on alertText to empty Alert Text
   */
  handleAlertDismiss() {
    this.setState({alertText: null});
  };

  render(){
    return(
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
                        <SelectSingleItem
                          id="Item"
                          name="Item"
                          type="ItemBarcode"
                          description={ Locale.getMessage(this.props.language,'item') }
                          searchFields = "Item.Code,Barcode.Code,Item.Name"
                          label="FullDescription"
                          urlSearchByType={this.props.urlSearchByType}
                          language={this.props.language}
                          inputItems={inputItemBarcodes}
                          onChange={this.itemBarcodeChanged}
                        />
                        <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                          <InputDatePickerWithOptionalValue
                            language={this.props.language}
                            onDayChange={this.fromDateChanged.bind(this)}
                            ref={ this.props.name }
                            name = {this.props.name}
                            caption = {Locale.getMessage(this.props.language, 'fromInventoryDate')}
                          />
                        </ReactCSSTransitionGroup>
                    </div>
                     <div className="buttonItem">
                        <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                          <Button type="submit" bsStyle="primary" bsSize="lg"  disabled={this.state.submitting}>
                            { Locale.getMessage(this.props.language,'recalculate') }
                            { this.state.submitting ? <Glyphicon glyph="refresh" /> : <Glyphicon glyph="menu-right" /> }
                          </Button>
                        </ReactCSSTransitionGroup>
                    </div>
                </div>
            </form>
            <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss} alertType={this.state.alertType}/>
        </section>
    );
  }
}

ItemStock.propTypes = {
  language:React.PropTypes.string,
  submitting: React.PropTypes.bool,
  handleSubmit: React.PropTypes.func
};

ItemStock.defaultProps = {
  initialValue: null
};
