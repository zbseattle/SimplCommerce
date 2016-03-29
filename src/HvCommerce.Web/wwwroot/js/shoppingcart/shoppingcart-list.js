(function () {
    angular
        .module('hv.shoppingCart', [])
        .controller('shoppingCartListCtrl', [
            'shoppingCartService',
            function (shoppingCartService) {
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
            }
        ]);
})();