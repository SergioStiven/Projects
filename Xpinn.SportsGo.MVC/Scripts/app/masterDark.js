(function () {
    'use strict';
    var dependencies =
        [
            "ngFileUpload",
            "uiCropper"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
    .config(['$translateProvider', function ($translateProvider) {
            // add translation table
            $translateProvider.translations('1', translationsES);
            $translateProvider.translations('2', translationsEN);
            $translateProvider.translations('3', translationsPOR);
            $translateProvider.preferredLanguage('1');
            $translateProvider.fallbackLanguage('1');
            $translateProvider.useSanitizeValueStrategy('escape');
        }])
    .controller('MasterController',
        ['$translate', "$sce", "$scope", "$rootScope", "service", "$filter", "$timeout", function ($translate, $sce, $scope, $rootScope, service, $filter, $timeout) {

            var controller = 'Home/';

            $scope.ConverToDate = function (date) {
                return $filter('date')(service.formatoFecha(date), 'MM/dd/yyyy');
            }

            $scope.Notifications = {
                list: [],
                showButtonSeeMore: true,
                filter: { SkipIndexBase: 0, TakeIndexBase: 0 },
                get: function (seeMore) {
                    if (seeMore) $('.popover_parent').addClass('active');
                    $scope.Notifications.filter.SkipIndexBase = $scope.Notifications.filter.TakeIndexBase;
                    $scope.Notifications.filter.TakeIndexBase += 2;
                    $scope.Notifications.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetNotifications', $scope.Notifications.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < $scope.Notifications.filter.TakeIndexBase) $scope.Notifications.showButtonSeeMore = false;
                            for (var i = 0; i < res.data.list.length; i++) {
                                if (res.data.list[i].UrlArchivo === '') {
                                    res.data.list[i].UrlArchivo = service.urlImageProfileDefault;
                                }
                                if (res.data.list[i].TipoDeLaNotificacion === 1) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_NEW');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_NEW_DESCRIPTION', {
                                        name: res.data.list[i].NombreApellidoPersona
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 5) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_ALMOST_EXPIRED');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_ALMOST_EXPIRED_DESCRIPTION', {
                                        name: res.data.list[i].DescripcionPlan,
                                        date: $scope.ConverToDate(res.data.list[i].FechaVencimientoPlan)
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 6) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_EXPIRED');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_EXPIRED_DESCRIPTION', {
                                        name: res.data.list[i].DescripcionPlan,
                                        date: $scope.ConverToDate(res.data.list[i].FechaVencimientoPlan)
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 7) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_REJECTED');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_REJECTED_DESCRIPTION', {
                                        name: res.data.list[i].DescripcionPlan
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 8) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_APPROVED');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_APPROVED_DESCRIPTION', {
                                        name: res.data.list[i].DescripcionPlan
                                    });
                                } else {
                                    res.data.list.splice(i, 1);
                                    i--;
                                }
                            }
                            $scope.Notifications.list = $scope.Notifications.list.concat(res.data.list);
                        }
                        else {
                            console.log(res.data.Message);
                        }
                    })
                },
                redirectToUrl: function (obj) {
                    if (obj.TipoDeLaNotificacion === 4) {
                        if (obj.UrlPublicidad !== '' && obj.UrlPublicidad !== null)
                            window.open(obj.UrlPublicidad, '_blank');
                    }
                    else if (obj.TipoDeLaNotificacion === 2 || obj.TipoDeLaNotificacion === 3) {
                        service.post('Search/SaveSearchIdInSession', { Consecutivo: obj.CodigoPersonaDestino }, function (res) {
                            if (!res.data.Success)
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'))
                        });
                    }
                }
            }

            $scope.Notifications.get();
            
        }])
    .controller('MenuController',
        ['$translate', "$sce", "$scope", "$rootScope", "Upload", "service", "$filter", "$timeout", function ($translate, $sce, $scope, $rootScope, Upload, service, $filter, $timeout) {

            var controller = 'Administration/';

            $scope.menu = {
                imageProfileDefault: service.urlImageProfileDefault,
                profile: { UrlImagenPerfilAdmin: '', PersonaDelUsuario: { NombreYApellido: '' } },
                isBusy: false,
                uploadImageProfile: function (dataImage, nameImage) {
                    if ($scope.menu.isBusy) return;
                    $scope.menu.isBusy = true;

                    Upload.upload({
                        url: service.urlBase + controller + 'UpdateImageProfileAdministrator',
                        data: { file: Upload.dataUrltoBlob(dataImage, nameImage) }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.menu.profile.UrlImagenPerfilAdmin = dataImage;
                            $('#modalImageProfile').modal('hide');
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                        $scope.menu.isBusy = false;
                    })
                },
                getInfoProfile: function () {
                    service.get('Profile/GetProfile', null, function (res) {
                        if (res.data.Success) {
                            $scope.menu.profile = res.data.obj;
                            $scope.User.userBefore.Usuario = res.data.obj.Usuario;
                            $scope.User.userToUpdate.Usuario = res.data.obj.Usuario;
                            $scope.User.userBefore.Email = res.data.obj.Email;
                            $scope.User.userToUpdate.Email = res.data.obj.Email;
                            if (res.data.obj.CodigoTipoPerfil !== 5)
                                $translate.use(res.data.obj.PersonaDelUsuario.CodigoIdioma.toString()); // Translate by language user

                            $rootScope.$emit('onGetProfile', { profile: res.data.obj });
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })

                }
            }

            $scope.User = {
                isBusy: false,
                userBefore: { Usuario: '', Email: '' },
                userToUpdate: { Usuario: '', Email: '', Clave: '', Password: '' },
                updateInfo: function () {
                    service.post('Settings/ChangeUser', $scope.User.userToUpdate, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.User.isBusy = false;
                    })
                },
                update: function () {
                    if ($scope.User.isBusy) return;
                    if (!$scope.User.validateForm()) return;
                    $scope.User.isBusy = true;
                    if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                        $scope.User.updateInfo();
                    }
                    else if (($scope.User.userBefore.Usuario.trim() !== $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                        service.post('Authenticate/ValidateIfUserExist', { Usuario: $scope.User.userToUpdate.Usuario }, function (res) {
                            if (res.data.Success) $scope.User.updateInfo();
                            else service.showErrorMessage($translate.instant('LBL_USER_UNAVAILABLE'), service.getTypeMessage('error'));
                            $scope.User.isBusy = false;
                        })
                    } else if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() !== $scope.User.userToUpdate.Email.trim())) {
                        service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.User.userToUpdate.Email }, function (res) {
                            if (res.data.Success) $scope.User.updateInfo();
                            else service.showErrorMessage($translate.instant('LBL_EMAIL_UNAVAILABLE'), service.getTypeMessage('error'));
                            $scope.User.isBusy = false;
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
                            $scope.User.isBusy = false;
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
            $translate.use(service.getLenguageFromNavigator().toString());
            $scope.menu.getInfoProfile();

        }]);
})();