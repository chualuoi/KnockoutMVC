(function () {
    if (typeof (ko) === undefined) { throw 'Knockout is required, please ensure it is loaded before loading this validation plug-in'; }
    if (typeof (ko.validation) === undefined) { throw 'Knockout validation is required, please ensure it is loaded before loading this plug-in'; }

    var htmlAttributes = {
        required: "data-val-required",
        min: "min",
        max: "max",
        pattern: "pattern",
        step: "step"
    };
    ko.validation.addRuleAttribute = (function () {
        return function (ruleName, attributeName) {
            htmlAttributes[ruleName] = attributeName;
        };
    })();

    ko.validation.parseInputValidationAttributes = (function () {
        return function (element, valueAccessor) {
            for (var ruleAttr in htmlAttributes) {
                
                //If we defined custom parse method, use it otherwise use the default
                if (typeof htmlAttributes[ruleAttr] == "function") {
                    htmlAttributes[ruleAttr](element, valueAccessor);
                }
                else if (ko.validation.utils.hasAttribute(element, htmlAttributes[ruleAttr])) {
                    ko.validation.addRule(valueAccessor(), {
                        rule: ruleAttr,
                        message: element.getAttribute(htmlAttributes[ruleAttr])
                    });
                }
            }
        };
    })();

    //// writes html5 validation attributes on the element passed in
    //ko.validation.writeInputValidationAttributes = (function () {

    //    return function (element, valueAccessor) {
    //        var observable = valueAccessor();

    //        if (!observable || !observable.rules) {
    //            return;
    //        }

    //        var contexts = observable.rules(); // observable array
    //        var $el = $(element);

    //        // loop through the attributes and add the information needed
    //        for (var ruleAttr in htmlAttributes) {
    //            var params;
    //            var ctx = ko.utils.arrayFirst(contexts, function (ctx) {
    //                return ctx.rule.toLowerCase() === attr.toLowerCase();
    //            });

    //            if (!ctx)
    //                return;

    //            params = ctx.params;

    //            // we have to do some special things for the pattern validation
    //            if (ctx.rule == "pattern") {
    //                if (ctx.params instanceof RegExp) {
    //                    params = ctx.params.source; // we need the pure string representation of the RegExpr without the //gi stuff
    //                }
    //            }

    //            // we have a rule matching a validation attribute at this point
    //            // so lets add it to the element along with the params
    //            $el.attr(attr, htmlAttributes[ruleAttr]);
    //        }
    //        contexts = null;
    //        $el = null;
    //    };
    //})();
    
    //#region Parsing rules

    ko.validation.addRuleAttribute("pattern", "data-val-regex");
    ko.validation.addRuleAttribute("digit", "data-val-digits");
    ko.validation.addRuleAttribute("email", "data-val-email");
    ko.validation.addRuleAttribute("number", "data-val-number");
    ko.validation.addRuleAttribute("date", "data-val-date");
    
    //TODO: Implement function to parse equal to validation
    ko.validation.addRuleAttribute("equal", "data-val-equalto");
    
    ko.validation.addRuleAttribute("min", "data-val-min");
    ko.validation.addRuleAttribute("max", "data-val-max");
    ko.validation.addRuleAttribute("minLength", "data-val-minlength");
    ko.validation.addRuleAttribute("maxLength", "data-val-maxlength");    
    //#endregion
})();