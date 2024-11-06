import React, { Component,PropTypes } from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import Locale from '../extensions/dist/Locale';
import Ajax from '../extensions/dist/Ajax';
import { Button,Glyphicon } from 'react-bootstrap';
import AlertDismissable from './AlertDismissable.jsx';
import ResultsGrid from './ResultsGrid.jsx';

/**
 * Class Representing Pos Recovery
 * @extends React.Component
 */
export default class PosRecovery extends React.Component {
  /**
   * Initialize props and this.state
   * @param  { Object } props - props
   */
  constructor(props) {
    super(props);
    this.state = {
      fields: props.initialData,
      submitting:false,
      language:props.language,
      fieldPaths:[],
      alertText:'',
      alertType:'',
      filesResult:[],
      loadResults:false
    };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
  }
  /**
   * handleSubmit onSubmit Form event handler
   * @param  {Object} element Form Submit Element
   */
  handleSubmit(element){
    element.preventDefault();

    this.setState({submitting: true});

    let data = {};
    data.selectedFiles = this.state.fieldPaths;

    Ajax.httpPost(this.props.urlGetDocumentHeaders,data,true).then(function(response) {
      const result = JSON.parse(response),
            error = JSON.parse(response).error;

      this.setState({submitting: false});

      if(error){
        this.setState({
          alertText: typeof error != 'undefined' ? error : Locale.getMessage(this.props.language,'noResultsFound'),
          alertType: 'warning'
        })
      }
      else{
        this.setState({
          filesResult: result,
          loadResults:true
        });
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
   * onChange onChange Input event handler
   * @param  {Object} event    onChange event
   * @param  {Object} filepath
   */
  onChange(event,filepath){

    let newfieldPaths = this.state.fieldPaths.slice();
    if(event.target.checked){
      newfieldPaths.push(filepath);
      this.setState({fieldPaths:newfieldPaths})
    }
    else{
        let finalFieldPaths = this.state.fieldPaths;
        finalFieldPaths = finalFieldPaths.filter(item => item !== filepath);
        this.setState({fieldPaths:finalFieldPaths});
    }
  }
  /**
   * Render PosRecovery Component
   * @return {Object} React View
   */
  render(){
    return (
      <section>
        <form onSubmit={this.handleSubmit}>
            <div className="Checkbox-Group">
                {Object.keys(this.state.fields).map(name => {
                  const filepath = this.state.fields[name].Filepath;
                  return (
                    <div className="Checkbox" key={filepath}>
                      <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                        <label className="Checkbox-Label">
                          <input className="Checkbox-Checkbox" type="checkbox"
                            checked={this.state[filepath]}
                            onChange={event => this.onChange(event,filepath)}/> {filepath}
                        </label>
                      </ReactCSSTransitionGroup>
                    </div>
                  );
                })}
             </div>
             <div className="buttonItem">
                <ReactCSSTransitionGroup transitionName="opacityEaseInOutExpo" transitionAppear={true} transitionAppearTimeout={200} transitionEnterTimeout={200} transitionLeaveTimeout={200}>
                  <Button type="submit" bsStyle="primary" bsSize="lg"  disabled={this.state.submitting}>
                    { Locale.getMessage(this.props.language,'select') }
                    { this.state.submitting ? <Glyphicon glyph="refresh" /> : <Glyphicon glyph="menu-right" /> }
                  </Button>
                </ReactCSSTransitionGroup>
            </div>
        </form>
        {this.state.loadResults ?
          <ResultsGrid
            results={this.state.filesResult}
            language={this.props.language}
            urlGetPOSTransactionFiles={this.props.urlGetPOSTransactionFiles}
            >
          </ResultsGrid>
          :null
        }
        <AlertDismissable alertText={this.state.alertText}  handleAlertDismiss={this.handleAlertDismiss} alertType={this.state.alertType}/>
    </section>
    );
  }
}


PosRecovery.propTypes = {
  fields:React.PropTypes.object,
  language:React.PropTypes.string,
  submitting: React.PropTypes.bool,
  handleSubmit: React.PropTypes.func,
  fieldPaths: React.PropTypes.array,
  filteredDataList: React.PropTypes.object
};
