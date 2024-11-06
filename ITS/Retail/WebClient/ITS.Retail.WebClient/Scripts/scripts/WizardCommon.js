var WizardCommon = {

    ShowWizard: function (args) {
        WizardCallbackPanel.PerformCallback(args);
    },
    MoveNext: function () {
        $('#StepCommand').val('NEXT');
        $('#WizardStepForm').submit();
    },
    GoBack: function () {
        $('#StepCommand').val('BACK');
        $('#WizardStepForm').submit();
    },
    Cancel: function () {
        $('#StepCommand').val('CANCEL');
        $('#WizardStepForm').submit();
    },
    Finish: function () {
        $('#StepCommand').val('FINISH');
        $('#WizardStepForm').submit();
        grdPromotions.PerformCallback();
    }
};