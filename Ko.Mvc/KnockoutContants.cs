namespace Ko.Mvc
{
    /// <summary>
    /// Contains all Knockout essential contants
    /// </summary>
    public static class KnockoutContants
    {
        /// <summary>
        /// Caption text for drop down list control
        /// </summary>
        public const string OptionsCaption = "Please select...";

        /// <summary>
        /// Caption text for drop down list control
        /// </summary>
        public const string OptionsText = "Text";

        /// <summary>
        /// Caption text for drop down list control
        /// </summary>
        public const string OptionsValue = "Value";

        /// <summary>
        /// Name of variable $data
        /// </summary>
        public const string CurrentContext = "$data";

        /// <summary>
        /// Name of variable $parent
        /// </summary>
        public const string ParentContext = "$parent";

        /// <summary>
        /// Name of variable $root
        /// </summary>
        public const string RootContext = "$root";

        /// <summary>
        /// Name of variable $index
        /// </summary>
        public const string Index = "$index";

        /// <summary>
        /// Uses when coverting C# member calling chain to Knockout member binding chain
        /// </summary>
        public const string FunctionEnclosing = "()";

        /// <summary>
        /// Uses when coverting C# member calling chain to Knockout member binding chain
        /// </summary>
        public const string Dot = ".";

        /// <summary>
        /// A tab of four spaces
        /// </summary>
        public const string Tab = "    ";

        /// <summary>
        /// New line character
        /// </summary>
        public const string NewLine = "\r\n";
        
        /// <summary>
        /// Comma character
        /// </summary>
        public const string Comma = ",";

        /// <summary>
        /// Default extension for KnockoutViewModel files
        /// </summary>
        public const string DefaultExtension = ".generated.js";

        /// <summary>
        /// Regular expression for extension for KnockoutViewModel files
        /// </summary>
        public const string ExtensionRegex = @"\w{0,10}.js";

        /// <summary>
        /// Default folder path for KnockoutViewModel files
        /// </summary>
        public const string DefaultFolderPath = "../../KnockoutViewModels";

        /// <summary>
        /// Regular expression for folder path for KnockoutViewModel files
        /// </summary>
        public const string FolderPathRegex = @"((([A-Za-z]:\\)|(..\\)*)?([^<>:""\/|?*]+[\\]*)*)|((..\/)*([^<>:""\\|?*]+[\/]*]*)*)";

    }

    /// <summary>
    /// Defines all common operators.
    /// Uses when converting C# Expression tree to javascript expression
    /// </summary>
    public static class JavaScriptOperator
    {
        public const string Add = "+";
        public const string LogicAnd = "&&";
        public const string BitwiseAnd = "&";

        public const string Divide = "/";
        public const string Equal = "==";
        public const string ExclusiveOr = "^";

        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";
        public const string LessThan = "<";

        public const string LessThanOrEqual = "<=";
        public const string Modulo = "%";
        public const string Multiply = "*";

        public const string Not = "!";
        public const string NotEqual = "!=";
        public const string BistwireOr = "|";

        public const string LogicOr = "||";
        public const string Power = "^";
        public const string Subtract = "-";

        public const string DoublePlus = "++";
        public const string DoubleMinus = "--";
    }

    public static class CommonRegex
    {
        public const string KnockValidBindingName = @"([a-zA-Z_]+[a-zA-Z0-9_]*)|('-?[a-zA-Z_]+[-a-zA-Z0-9_]*')";

        public const string DataBindRegex = @"\s*data-bind\s*=\s*""([^""]*)""\s*";

        public const string ValueRegex = @"\s*value\s*=\s*""([^""]*)""\s*";

        public const string HtmlStartTagRegex = @"<([a-zA-Z_]+[a-zA-Z0-9_]*)((([a-zA-Z_]+[a-zA-Z0-9_]*)\s*)*(([a-zA-Z_]+[a-zA-Z0-9_]*)|([a-zA-Z_]+[-a-zA-Z0-9_]*)\s*=\s*""([^\""]*)""\s*)*)*\/?>";
    }

    public static class KoViewModelTemplate
    {
        public const string EventHandler = @"
        self.{0} = function (){{
            //TODO: Add custom logic here
        }};";

        public const string ComputedProperty = @"
        self.{0} = ko.computed(function (){{
            //TODO: Add custom logic here
            return undefined;
        }});";

        public const string DefaultProperty = @"{0}:{1}";

        public const string OnewayProperty = @"
        self.{0} = data.{0};";

        public const string StaticProperty = @"
    {0}.{1} = {2};";

        public const string ObservableProperty = @"
        self.{0} = ko.observable(data.{0});";

        public const string ObservableArrayProperty = @"
        self.{0} = ko.observableArray(data.{0});";

        public const string IgnoredProperty = @"
                '{0}'";

        public const string CustomProperty = @"
            ,{0} : {{
                create: function(options){{
                    return ko.observable(new {1}(options.data));
                }}
            }}";

        public const string ViewModelExtensions = @"
/// <reference path=""{0}.generated.js"" />

(function($){{
    {0}.prototype.InitExtensions = function () {
        var self = this;  
        /**********************************************
        *               Additional Properties
        ***********************************************/

        /**********************************************
        *               Computeds
        ***********************************************/
        {1}

        /**********************************************
        *               Event Handlers
        ***********************************************/
        {2}
        
    };
}}(jQuery));";

        public const string ViewModel = @"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

{0}
(function($){{
    {1} = function(data){{
        var defaults = {2};
        data = $.extend({{}}, defaults, data);
        
        var mappingOptions = {{
            ignore: [{3}
            ]{4}
        }};
        var self = this;
        
        ko.mapping.fromJS(data, mappingOptions, self);
        
        /**********************************************
        *               One-way Mapping
        ***********************************************/
        {5}
        
        if(self.InitExtensions && $.isFunction(self.InitExtensions)){{
            self.InitExtensions();
        }}
    }};

    /**********************************************
    *                Static Property
    ***********************************************/
    {6}
}}(jQuery));
        ";

        public const string RootNamspace = "{0} = {0} || {{}};\r\n";
        public const string Namspace = "{0} = {0} || {{}};\r\n";
    }
}