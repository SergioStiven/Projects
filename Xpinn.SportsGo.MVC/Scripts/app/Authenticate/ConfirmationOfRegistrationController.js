(function () {

    'use strict';

    angular
        .module('app', ['pascalprecht.translate'])
        .config(['$translateProvider', function ($translateProvider) {
            // add translation table
            $translateProvider.translations('1', translationsES);
            $translateProvider.translations('2', translationsEN);
            $translateProvider.translations('3', translationsPOR);
            $translateProvider.preferredLanguage('1');
            $translateProvider.fallbackLanguage('1');
        }])
        .controller('ConfirmationOfRegistrationController', ConfirmationOfRegistrationController);

    ConfirmationOfRegistrationController.$inject = ['$translate', '$scope', 'service'];

    function ConfirmationOfRegistrationController($translate, $scope, service) {

        $scope.changeLanguage = function () {
            $translate.use(String($scope.getParameterByName('Language')));
        };

        $scope.getParameterByName = function(name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
            return results === null ? "1" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        $scope.changeLanguage();
    }

})();