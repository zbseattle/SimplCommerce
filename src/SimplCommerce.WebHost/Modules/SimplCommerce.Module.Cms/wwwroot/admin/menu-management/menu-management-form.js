(function($){
    angular
        .module('simplAdmin.cms')
        .controller('MenuManagementCtrl',MenuManagementCtrl);
    function MenuManagementCtrl($state, $stateParams) {
        var vm = this;
        vm.widgetZones = [];
        vm.widgetInstance = { widgetZoneId: 1, publishStart : new Date() };
        vm.widgetInstanceId = $stateParams.id;
        vm.isEditMode = vm.widgetInstanceId > 0;

        vm.datePickerPublishStart = {};
        vm.datePickerPublishEnd = {};
    }
})(jQuery);