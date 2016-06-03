(function () {
    var update = ko.bindingHandlers.checked.update;
    
    //Override the default checked binding of knockout
    ko.bindingHandlers.update = function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        //Call the original update
        update(element, valueAccessor, allBindingsAccessor, viewModel, bindingContext);
        
        //Add value attribute to html element
        var value = ko.utils.unwrapObservable(valueAccessor());
        if (element.type == "checkbox" && !value instanceof Array) {
            element.value = element.checked;
        }
    };
})();