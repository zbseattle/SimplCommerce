(function(cmsModule){
    cmsModule.controller('MenuManagementCtrl', MenuManagementCtrl);
    function MenuManagementCtrl($scope, categoryService) {
        var vm = this;
        $scope.status = {
            isCategoriesOpen:true
        };
        function init() {
            debugger;
            categoryService.getCategories().then(function (result) {
                debugger;
                vm.categories = result.data;
            });
        }
        init();
    }
})(angular.module('simplAdmin.cms'));