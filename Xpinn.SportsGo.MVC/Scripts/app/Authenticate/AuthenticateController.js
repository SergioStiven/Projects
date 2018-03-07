(function () {

    'use strict';

    angular
        .module('app', ['pascalprecht.translate', "oi.select"])
        .config(['$translateProvider', function ($translateProvider) {
            // add translation table
            $translateProvider.translations('1', translationsES);
            $translateProvider.translations('2', translationsEN);
            $translateProvider.translations('3', translationsPOR);
            $translateProvider.preferredLanguage('1');
            $translateProvider.fallbackLanguage('1');
        }])
        .controller('AuthenticateController', Authenticate);

    Authenticate.$inject = ['$translate', '$scope', '$filter', "$timeout", 'service'];

    function Authenticate($translate, $scope, $filter, $timeout, service) {

        var controller = 'Authenticate/';

        $scope.changeLanguage = function (langKey) {
            $translate.use(String($scope.User.userForRegistration.PersonaDelUsuario.CodigoIdioma));
            $scope.User.getTermsAndConditions($scope.User.userForRegistration.PersonaDelUsuario.CodigoIdioma);
            $scope.Lists.profiles = [{ Consecutivo: 1, DescripcionIdiomaBuscado: $translate.instant('LBL_ATHLETES') }, { Consecutivo: 2, DescripcionIdiomaBuscado: $translate.instant('LBL_GROUPS') }, { Consecutivo: 3, DescripcionIdiomaBuscado: $translate.instant('LBL_REPRESENTATIVE') }, { Consecutivo: 4, DescripcionIdiomaBuscado: $translate.instant('LBL_ADVERTISERS') }];
            $scope.Lists.languages = [{ Consecutivo: 1, DescripcionIdiomaBuscado: $translate.instant('LBL_SPANISH') }, { Consecutivo: 2, DescripcionIdiomaBuscado: $translate.instant('LBL_ENGLISH') }, { Consecutivo: 3, DescripcionIdiomaBuscado: $translate.instant('LBL_PORTUGUESE') }];
            $scope.User.userForRegistration.Profile = $filter('filter')($scope.Lists.profiles, { Consecutivo: $scope.User.userForRegistration.Profile.Consecutivo })[0];;
        };

        $scope.validateEmail = function(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }

        $scope.Lists = {
            profiles: [{ Consecutivo: 1, DescripcionIdiomaBuscado: $translate.instant('LBL_ATHLETES') }, { Consecutivo: 2, DescripcionIdiomaBuscado: $translate.instant('LBL_GROUPS') }, { Consecutivo: 3, DescripcionIdiomaBuscado: $translate.instant('LBL_REPRESENTATIVE') }, { Consecutivo: 4, DescripcionIdiomaBuscado: $translate.instant('LBL_ADVERTISERS') }],
            languages: [{ Consecutivo: 1, DescripcionIdiomaBuscado: $translate.instant('LBL_SPANISH') }, { Consecutivo: 2, DescripcionIdiomaBuscado: $translate.instant('LBL_ENGLISH') }, { Consecutivo: 3, DescripcionIdiomaBuscado: $translate.instant('LBL_PORTUGUESE') }]
        }

        $scope.User = {
            typeProfiles: [],
            infoUser: { EsRecordarClave: false, Usuario: '', Email: '' },
            userForRegistration: { Usuario: '', Email: '', CodigoTipoPerfil: '', PersonaDelUsuario: { CodigoIdioma: 1 }, Clave: '', ClaveConfirmacion: '', AcceptTerms: false, Profile: { Consecutivo : 0 } },
            termsAndConditions: { Texto: '' },
            isBusy: false,
            register: function () {
                if ($scope.User.isBusy) return;
                if (!$scope.User.validateFormRegister()) return;
                $scope.User.isBusy = true;

                $scope.User.userForRegistration.CodigoTipoPerfil = $scope.User.userForRegistration.Profile.Consecutivo;
                service.post(controller + 'ConfirmRegistration', $scope.User.userForRegistration, function (res) {
                    if (res.data.Success)
                        service.post(controller + 'SingInTemporaly', $scope.User.userForRegistration, null);
                    else
                        service.showErrorMessage($translate.instant(res.data.Message), service.getTypeMessage('error'));
                    $scope.User.isBusy = false;
                })
            },
            login: function () {
                if ($scope.User.isBusy) return;
                $scope.User.isBusy = true;
                service.post(controller + 'Login', $scope.User.infoUser, function (res) {
                    if (!res.data.success) {
                        service.showErrorMessage($translate.instant(res.data.Message), service.getTypeMessage('error'));
                    }
                    $scope.User.isBusy = false;
                })
            },
            recoverPassword: function () {
                if ($scope.User.isBusy) return;
                $scope.User.isBusy = true;
                if ($scope.User.infoUser.Usuario.trim() === '') {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_USER'), service.getTypeMessage('error'));
                    $scope.User.isBusy = false;
                    return;
                }
                if (!$scope.User.validateEmail($scope.User.infoUser.Email)) {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                    $scope.User.isBusy = false;
                    return;
                }
                service.post(controller + 'RecoverPassword', $scope.User.infoUser, function (res) {
                    if (res.data.Success)
                        service.showErrorMessage($translate.instant('NOTI_SUCCESS_SEND_MAIL'), service.getTypeMessage('success'));
                    else
                        service.showErrorMessage($translate.instant('NOTI_ERROR_SEND_MAIL'), service.getTypeMessage('error'));

                    $scope.User.isBusy = false;
                })
            },
            getTermsAndConditions: function (langKey) {
                service.post('Administration/GetTermsAndCondiions', { CodigoIdioma: langKey }, function (res) {
                    if (res.data.Success)
                        $scope.User.termsAndConditions = res.data.obj;
                    else 
                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                })
            },
            validateFormRegister: function () {
                if ($scope.User.userForRegistration.Usuario === '') {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_USER'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userForRegistration.Email === '') {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                    return false;
                } else if (!$scope.User.validateEmail($scope.User.userForRegistration.Email)) {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userForRegistration.Profile.Consecutivo === '') {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_PROFILE'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userForRegistration.PersonaDelUsuario.CodigoIdioma === '') {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_LANGUAGE'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userForRegistration.Clave === '') {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_RETYPE_PASS'), service.getTypeMessage('error'));
                    return false;
                } else if (!$scope.User.validatePass()) {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_PASSWORD'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userForRegistration.Clave !== $scope.User.userForRegistration.ClaveConfirmacion) {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_RETYPE_PASS'), service.getTypeMessage('error'));
                    return false;
                }
                else if (!$scope.User.userForRegistration.AcceptTerms) {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_TERMS'), service.getTypeMessage('error'));
                    return false;
                }
                    
                return true;
            },
            validateEmail: function (email) {
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test(email);
            },
            validatePass: function () {
                var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
                return re.test($scope.User.userForRegistration.Clave);
            }
        }

        // Set image banner
        $('#divBanner').css("background-image", "url(" + service.urlBase + "Content/assets/img/LogoSportsGo.png)");
        //Set image header form
        $('#divHeaderForm').css("background-image", "url(" + service.urlBase + "Content/assets/img/form-banner.png)");
        // Get terms and Conditions
        $scope.User.getTermsAndConditions(String(service.getLenguageFromNavigator()));
    }

})();