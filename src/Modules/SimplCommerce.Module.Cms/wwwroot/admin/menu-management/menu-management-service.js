/*global angular*/
(function () {
    angular
        .module('simplAdmin.cms')
        .factory('menuService', MenuService);

    /* @ngInject */
    function MenuService($http) {
        var service = {
        };
        return service;

        function getPage(id) {
            return $http.get('api/pages/' + id);
        }
    }
})();