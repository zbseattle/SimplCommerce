(function () {
    angular
        .module('hv.shoppingCart', [])
        .controller('shoppingCartListCtrl', [
            '$scope',
            'shoppingCartService',
            function ($scope, shoppingCartService) {
                var vm = this;
                vm.shoppingCartItems = [];
                vm.totalPrice = 0;
                getShoppingCartItems();

                function getShoppingCartItems() {
                    shoppingCartService.getShoppingCartItems().then(function (result) {
                        vm.shoppingCartItems = result.data;
                        $.each(result.data, function () {
                            vm.totalPrice += this.totalPrice;
                        });
                    });
                };

                vm.removeShoppingCartItem = function removeShoppingCartItem(item) {
                    var index = vm.shoppingCartItems.indexOf(item);
                    vm.shoppingCartItems.splice(index, 1);

                    var resetedTotalPrice = 0;
                    $.each(vm.shoppingCartItems, function () {
                        resetedTotalPrice += this.totalPrice;
                    });
                    vm.totalPrice = resetedTotalPrice;

                    shoppingCartService.removeShoppingCartItem(item.id);
                }

                $scope.$watch(angular.bind(this, function () {
                    return this.shoppingCartItems.map(function (item) {
                        return item.quantity;
                    });
                }), function (newVal, oldVal) {
                    var resetedTotalPrice = 0;
                    $.each(vm.shoppingCartItems, function () {
                        this.totalPrice = this.quantity * this.price;
                        resetedTotalPrice += this.totalPrice;
                    });
                    vm.totalPrice = resetedTotalPrice;
                });
            }
        ]);
})();