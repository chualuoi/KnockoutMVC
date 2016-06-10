Ko.PoC.ViewModels.HelloWorldViewModel.prototype.InitExtensions = function () {
    var self = this;    

    /*------------------computed properties------------------*/    
    self.FullName = ko.computed(function(){
        //TODO: Write computing logic here:
        return self.FirstName() + " " + self.LastName();
    });
    /*-------------------------------------------------------*/
    return self;
};
