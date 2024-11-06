/**
 * Add Application Log
 * @class ApplicationLog
 */
var ApplicationLog = {
    /**
     * @method GrdLogViewBeginCallback
     * @param {Object} s
     * @param {Object} e
     */
    GrdLogViewBeginCallback: function (s, e) {
        e.customArgs = {
            "IP": ip.GetValue(),
            "controller": controller.GetValue(),
            "created": created.GetText(),
            "user": user.GetValue(),
        };
    },
    /**
     * @method ClearLogOnClick
     * @param {Object} s
     * @param {Object} e
     */
    ClearLogOnClick: function (s, e) {
        var answer = confirm(deleteAllConfirmation);
        if (answer === true) {
            grdLogView.PerformCallback("CLEARLOG");
        }
    }
};
/**
 * @method ClearLogSearchCriteria
 * @param {Object} s
 * @param {Object} e
 */
function ClearLogSearchCriteria(s, e) {
    $("#created_I").val('');
    $("#user_I").val('');
    $("#controller_I").val('');
    $("#ip_I").val('');
}
/**
 * @method OnClickClearLog
 * @param {Object} s
 * @param {Object} e
 */
function OnClickClearLog(s, e) {
    var answer = confirm(deleteAllConfirmation);
    if (answer === true) {
        grdLogView.PerformCallback({ action: 'CLEARLOG' });
    }
}