import React from 'react';
import ReactCSSTransitionGroup from 'react-addons-css-transition-group';
import TestUtils from 'react/lib/ReactTestUtils';
import { expect } from 'chai';
import VariableValuesDisplay from '../VariableValuesDisplay.jsx';

/**
 * INITIAL_DATA
 * @type {Array}
 */
const INITIAL_DATA = [
  {
    'label':'CategoryName1',
    'value':'1000-0000-AAAA-1111'
  },
  {
    'label':'CategoryName2',
    'value':'2000-0000-AAAA-1111'
  }
],
  PLACEHOLDER = 'Select',
  TITLE_DATA_VIEW_CATEGORY = 'Data View Category',
  TITLE_DATA_VIEW = "Data View",
  URL_GET_DATA_VIEW = "http://localhost.dev/GetDataView",
  TITLE_CUSTOM_DATA_VIEW_PARAMETERS = "",
  URL_GET_VIEW_PARAMETERS = "http://localhost.dev/GetViewParameters",
  LANGUAGE = "en";

describe('VariableValuesDisplay', function () {

    const variableValuesDisplay = TestUtils.renderIntoDocument(
        <VariableValuesDisplay initialData={INITIAL_DATA}
        placeholder={PLACEHOLDER}

        noResultsText={PLACEHOLDER}
        titleDataView={TITLE_DATA_VIEW}
        urlGetDataView={URL_GET_DATA_VIEW}
        titleCustomDataViewParameters={TITLE_CUSTOM_DATA_VIEW_PARAMETERS}
        urlGetViewParameters={URL_GET_VIEW_PARAMETERS}
        language={LANGUAGE}
        />
    );

    it('loads without error', () => {
        expect(variableValuesDisplay).to.exist;
        const result = TestUtils.isElementOfType (
          variableValuesDisplay,'VariableValuesDisplay'
        );
    });

    it('renders an h3', () => {
        const h3 = TestUtils.findRenderedDOMComponentWithTag(
            variableValuesDisplay, 'h3'
        );
        expect(h3.textContent).to.equal(TITLE_DATA_VIEW_CATEGORY);
    });

    it('render a Select Component', () =>{
         const select = TestUtils.findRenderedDOMComponentWithClass (
           variableValuesDisplay, 'Select'
         );
         expect(select.className).to.equal("Select section-select is-searchable opacityEaseInOutExpo-appear");
    });
});
