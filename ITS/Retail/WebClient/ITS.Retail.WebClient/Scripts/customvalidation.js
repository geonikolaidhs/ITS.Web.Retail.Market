
$(function () {
    // Registering  custom adapter
    $.validator.unobtrusive.adapters.add('compareto', ['otherproperty', 'operatortype'],
        function (options) {
            console.log('unobtrusive compareto');
            var params = {
                otherproperty: options.params.otherproperty,                   
                operatortype: options.params.operatortype,
            };
            options.rules['compareto'] = params;
            if (options.message) {
                options.messages['compareto'] = options.message;
            }
        });


    $.validator.unobtrusive.adapters.add('requiredif', ['dependentproperty', 'targetvalue'],
        function (options) {
            options.rules['requiredif'] = {
                dependentproperty: options.params['dependentproperty'],
                targetvalue: options.params['targetvalue']
            };
            options.messages['requiredif'] = options.message;
        });

    // Add validator function
    $.validator.addMethod('compareto',
        function (value, element, param) {
            //console.log('unobtrusive compareto');
            var leftValue, rightValue;
            var collection = ASPxClientControl.GetControlCollection();
            var leftElement = collection.GetByName(element.name);
            var rightElement = collection.GetByName(param.otherproperty);
            if (leftElement != null) {
                leftValue = leftElement.GetValue();
            }
            else
            {
                leftValue = $('#' + element.name).val();
            }
            var rightValue = rightElement.GetValue();
            switch (param.operatortype) {
                case "LESS":
                    return (leftValue < rightValue);
                case "EQUAL":
                    return (leftValue == rightValue);
                case "GREATER":
                    return (leftValue > rightValue);
                case "EQUAL, GREATER":
                    return (leftValue >= rightValue);

            }
            return false;
        }
    );


    $.validator.addMethod('requiredif',
        function (value, element, parameters) {
            var id = '#' + parameters['dependentproperty'];

            // get the target value (as a string, 
            // as that's what actual value will be)
            var targetvalue = parameters['targetvalue'];
            targetvalue =
              (targetvalue == null ? '' : targetvalue).toString();

            // get the actual value of the target control
            // note - this probably needs to cater for more 
            // control types, e.g. radios
            var control = $(id);
            var controltype = control.attr('type');
            var actualvalue =
                controltype === 'checkbox' ?
                control.attr('checked').toString() :
                control.val();

            // if the condition is true, reuse the existing 
            // required field validator functionality
            if (targetvalue === actualvalue)
                return $.validator.methods.required.call(
                  this, value, element, parameters);

            return true;
        }
         );
}(jQuery));


