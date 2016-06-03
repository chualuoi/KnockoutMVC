//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


koLib.namespace('Ko.PoC.Models');

Ko.PoC.Models.RequiredModel = function (rawData, parent, defaults, mappingOptions) {
    var self = this;
    
    defaults = $.extend({}, {"RequiredField":null}, defaults);
    
    mappingOptions = $.extend({}, {
        'ignore': []    
    }, mappingOptions);	
    		
	self.MappingCompleted = ko.observable(false);
	
	rawData = $.extend({}, defaults, rawData);	
	
	ko.mapping.fromJS(rawData, mappingOptions, self);
	
	self.parent = ko.observable(parent);
	self.root = ko.observable();
	self.ResetRoot = ko.computed(function(){
		var xparent = self.parent();
		if(xparent){
			self.root(xparent.root());
		}
	});		
		
	self.InitExtensions(); 	
	
	self.MappingCompleted(true);	
            
    return self;
};

