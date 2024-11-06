

/**
 * 
 * @method grdCompanyEndCallback
 * @param {Object} s
 * @param {Object} e
 */
function grdCompanyEndCallback(s, e) {
    RefreshCompaniesAndStoresMenu(s, e);

    grdOnEndCalback(s, e);
}

