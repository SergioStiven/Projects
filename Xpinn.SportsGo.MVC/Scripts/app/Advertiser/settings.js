(function () {
    'use strict';
    app
    .controller('SettingsController',
        ["$translate", "$sce", "$scope", "$rootScope", "service", function ($translate, $sce, $scope, $rootScope, service) {

            var controller = 'Settings/';

            $rootScope.$on('onGetProfile', function (event, obj) {
                $scope.User.userBefore.Usuario = obj.profile.Usuario;
                $scope.User.userToUpdate.Usuario = obj.profile.Usuario;
                $scope.User.userBefore.Email = obj.profile.Email;
                $scope.User.userToUpdate.Email = obj.profile.Email;
                $scope.Language.selectedLanguage = obj.profile.PersonaDelUsuario.CodigoIdioma.toString();
            });

            $scope.User = {
                userBefore: { Usuario: '', Email: '' },
                userToUpdate: { Usuario: '', Email: '', Clave: '', Password: '' },
                updateInfo: function () {
                    service.post(controller + 'ChangeUser', $scope.User.userToUpdate, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                update: function () {
                    if (!$scope.User.validateForm()) return;

                    if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                        $scope.User.updateInfo();
                    }
                    else if (($scope.User.userBefore.Usuario.trim() !== $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                        service.post('Authenticate/ValidateIfUserExist', { Usuario: $scope.User.userToUpdate.Usuario }, function (res) {
                            if (res.data.Success) $scope.User.updateInfo();
                            else service.showErrorMessage($translate.instant('LBL_USER_UNAVAILABLE'), service.getTypeMessage('error'));
                        })
                    } else if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() !== $scope.User.userToUpdate.Email.trim())) {
                        service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.User.userToUpdate.Email }, function (res) {
                            if (res.data.Success) $scope.User.updateInfo();
                            else service.showErrorMessage($translate.instant('LBL_EMAIL_UNAVAILABLE'), service.getTypeMessage('error'));
                        })
                    } else if (($scope.User.userBefore.Usuario.trim() !== $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() !== $scope.User.userToUpdate.Email.trim())) {
                        service.post('Authenticate/ValidateIfUserExist', { Usuario: $scope.User.userToUpdate.Usuario }, function (res) {
                            if (res.data.Success) {
                                service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.User.userToUpdate.Email }, function (res) {
                                    if (res.data.Success) $scope.User.updateInfo();
                                    else service.showErrorMessage($translate.instant('LBL_EMAIL_UNAVAILABLE'), service.getTypeMessage('error'));
                                })
                            }
                            else service.showErrorMessage($translate.instant('LBL_USER_UNAVAILABLE'), service.getTypeMessage('error'));
                        })
                    }
                },
                validateForm: function () {
                    if ($scope.User.userToUpdate.Usuario === '' || $scope.User.userToUpdate.Email === ''
                        || $scope.User.userToUpdate.Clave === '' || $scope.User.userToUpdate.Password === '') {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else if ($scope.User.userToUpdate.Clave !== $scope.User.userToUpdate.Password) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_RETYPE_PASS'), service.getTypeMessage('error'));
                        return false;
                    } else {
                        return true;
                    }
                }
            }

            $scope.Language = {
                selectedLanguage: '1',
                updateLanguage: function (langKey) {
                    $translate.use(langKey);
                    $scope.Language.selectedLanguage = langKey;
                },
                update: function () {
                    service.post(controller + 'ChangeLanguage', { CodigoIdioma: $scope.Language.selectedLanguage }, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                        $scope.Settings.isBusy = false;
                    })
                }
            }

        }]
    )
})();
