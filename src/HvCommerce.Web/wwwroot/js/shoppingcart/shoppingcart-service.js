(function() {
    angular
        .module('hv.shoppingCart')
        .factory('shoppingCartService', [
            '$http',
            function ($http) {
                function getShoppingCartItems() {
                    return $http.get('ShoppingCart/List');
                }
                
                function removeShoppingCartItem(itemId) {
                    return $http.post('ShoppingCart/Remove', itemId);
                }

                return {
                    getShoppingCartItems: getShoppingCartItems,
                    removeShoppingCartItem: removeShoppingCartItem
                };
            }
        ]);
})();