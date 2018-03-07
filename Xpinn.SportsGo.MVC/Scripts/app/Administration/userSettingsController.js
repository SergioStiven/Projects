(function () {
    'use strict';
    var dependencies =
        [
            "oi.select",
            "ngFileUpload"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app.controller('userSettingsController', ["$translate", "$sce", "$scope", "service",
        function ($translate, $sce, $scope, service) {

            var controller = 'Administration/';
                
            $scope.ToDelete = {
                obj: {},
                callback: null,
                assign: function (obj, callback) {
                    this.obj = obj;
                    this.callback = callback;
                },
                ressed: function () {
                    this.obj = {};
                    this.callback = null;
                },
                executeCallback: function () {
                    this.callback(this.obj);
                    $('#alertWarning').modal('hide');
                }
            };

            $scope.propertyName = 'Usuarios.Usuario';
            $scope.reverse = true;

            $scope.lists = {
                planes: {},
                getPlanes: function (user) {
                    var filter = {};
                    filter.IdiomaBase = 1;
                    filter.CodigoTipoPerfil = user.CodigoTipoPerfil;
                    filter.TakeIndexBase = 999;
                    service.post(controller + 'GetListPlansForAdmin', filter, function (res) {
                        if (res.data.Success) {
                            $scope.lists.planes = res.data.list;
                            $scope.userSettings.userDetails.PlanesUsuarios.CodigoPlanDeseado = $scope.userSettings.userDetails.PlanesUsuarios.CodigoPlan;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            }

            $scope.userSettings = {
                list: [],
                userDetails: {},
                addNew: false,
                noDataFound: false,
                currentPage: 1,
                range: 0,
                steps: 10,
                totalRecords: 0,
                nextButton: true,
                userTemp: { Consecutivo: 0, PlanesContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }], Precio: 0, CodigoPeriodicidad: 1 },
                options: { VideosPerfil: false, ServiciosChat: false, ConsultaCandidatos: false, DetalleCandidatos: false, ConsultaGrupos: false, DetalleGrupos: false, ConsultaEventos: false, CreacionAnuncios: false, EstadisticasAnuncios: false, Modificacion: false },
                filter: { TakeIndexBase: 10, SkipIndexBase: 0 },
                get: function (isSearch) {
                    $scope.userSettings.noDataFound = false;
                    $scope.userSettings.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetUsers', $scope.userSettings.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length === 0) {
                                if (isSearch) {
                                    $scope.userSettings.noDataFound = true;
                                }
                                $scope.userSettings.nextButton = false;
                                return;
                            }
                            else {
                                $scope.userSettings.nextButton = true;
                                $scope.userSettings.totalRecords = res.data.list[0].NumeroRegistrosExistentes;
                            }
                            $scope.userSettings.list = res.data.list;
                        }
                        else
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                openModalToBlock: function (user,isDelete) {
                    if (isDelete) {
                        $scope.ToDelete.assign(user, $scope.userSettings.delete);
                        $scope.messageModal = $translate.instant('LBL_MESSAGE_CONFIRM_DELETE');
                        $('#alertWarning').modal('toggle');
                    }else if (user.Usuarios.CuentaActiva === 1) {
                        $scope.ToDelete.assign(user, $scope.userSettings.block);
                        $scope.messageModal = $translate.instant('LBL_MESSAGE_CONFIRM_LOCK');
                        $('#alertWarning').modal('toggle');
                    } else {
                        $scope.userSettings.unlock(user);
                    }
                },
                block: function (userToblock) {
                    service.post(controller + 'BlockUser', { Consecutivo: userToblock.CodigoUsuario }, function (res) {
                        if (res.data.Success) {
                            $scope.userSettings.get(false);
                            service.showErrorMessage($translate.instant('NOTI_BLOCKUSER_SUCCESS'), service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                unlock: function (userToUnlock) {
                    service.post(controller + 'UnlockUser', { Consecutivo: userToUnlock.CodigoUsuario }, function (res) {
                        if (res.data.Success) {
                            userToUnlock.Usuarios.CuentaActiva = 1;
                            service.showErrorMessage($translate.instant('NOTI_UNLOCKUSER_SUCCESS'), service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                delete: function (userToDelete) {
                    service.post(controller + 'DeleteUser', { Consecutivo: userToDelete.CodigoUsuario }, function (res) {
                        if (res.data.Success) {
                            $scope.userSettings.list.splice($scope.userSettings.list.indexOf(userToDelete), 1);
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                update: function () {
                    service.post(controller + 'UpdateUser', $scope.userSettings.userDetails, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                        $('#alertUserDetails').modal('hide');
                    })
                },
                searchUser: function(){
                    $scope.userSettings.filter.TakeIndexBase = $scope.userSettings.steps;
                    $scope.userSettings.filter.SkipIndexBase = 0;
                    $scope.userSettings.get(true);
                },
                next: function () {
                    if (($scope.userSettings.filter.SkipIndexBase + $scope.userSettings.steps) >= $scope.userSettings.totalRecords) return;
                    $scope.userSettings.filter.SkipIndexBase += $scope.userSettings.steps;
                    $scope.userSettings.get(false);
                },
                before: function () {
                    if ($scope.userSettings.filter.SkipIndexBase <= 0 || $scope.userSettings.totalRecords === $scope.userSettings.list.length) return;
                    $scope.userSettings.filter.TakeIndexBase = $scope.userSettings.steps;
                    $scope.userSettings.filter.SkipIndexBase -= $scope.userSettings.steps;
                    $scope.userSettings.get(false);
                },
                first: function (){
                    $scope.userSettings.filter.TakeIndexBase = $scope.userSettings.steps;
                    $scope.userSettings.filter.SkipIndexBase = 0;
                    $scope.userSettings.get(false);
                },
                last: function () {
                    if (($scope.userSettings.totalRecords - $scope.userSettings.steps) < 0) return;

                    var totalRecords = String($scope.userSettings.totalRecords);
                    var take = totalRecords.substring(totalRecords.length - 1);
                    var skipe = $scope.userSettings.totalRecords - parseInt(take);

                    $scope.userSettings.filter.TakeIndexBase = parseInt(take);
                    $scope.userSettings.filter.SkipIndexBase = parseInt(skipe);
                    
                    $scope.userSettings.get(false);
                },
                goToPage: function (page) {
                    $scope.userSettings.currentPage = page;
                    $scope.userSettings.filter.SkipIndexBase = (page - 1) * $scope.userSettings.steps;
                    $scope.userSettings.get(false);
                },
                sortBy: function(propertyName) {
                    $scope.reverse = ($scope.propertyName === propertyName) ? !$scope.reverse : false;
                    $scope.propertyName = propertyName;
                },
                getProfileDescriptionById(id) {
                    switch (id) {
                        case 1:
                            return $translate.instant('LBL_ATHLETES');
                        case 2:
                            return $translate.instant('LBL_GROUPS');
                        case 3:
                            return $translate.instant('LBL_REPRESENTATIVE');
                        case 4:
                            return $translate.instant('LBL_ADMINISTRATOR');
                        default:
                    }
                },
                showUserDetail: function (user) {
                    $scope.userSettings.userDetails = user.Usuarios;
                    $scope.userSettings.userDetails.emailBefore = user.Usuarios.Email;
                    $scope.lists.getPlanes(user.Usuarios);
                    $('#alertUserDetails').modal('toggle');
                },
                validateIfEmailExist: function () {
                    if (!$scope.userSettings.validate()) return;
                    if ($scope.userSettings.userDetails.emailBefore === $scope.userSettings.userDetails.Email) {
                        $scope.userSettings.update();
                    } else {
                        service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.userSettings.userDetails.Email }, function (res) {
                            if (!res.data.Success) {
                                service.showErrorMessage($translate.instant('NOTI_VALIDATION_MAIL_REGISTERED'), service.getTypeMessage('error'));
                                return;
                            }
                            $scope.userSettings.update();
                        })
                    }
                },
                validate: function () {
                    if ($scope.userSettings.userDetails.Usuario === '') {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_USER'), service.getTypeMessage('error'));
                        return false;
                    } else if (!$scope.userSettings.validateEmail($scope.userSettings.userDetails.Email)) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                        return false;
                    } else if ($scope.userSettings.userDetails.Clave !== null && $scope.userSettings.userDetails.Clave !== '') {
                        if (!$scope.userSettings.validatePass($scope.userSettings.userDetails.Clave.trim())) {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_PASSWORD'), service.getTypeMessage('error'));
                            return false;
                        } else if ($scope.userSettings.userDetails.Clave !== $scope.userSettings.userDetails.ClaveConfirmacion ) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_RETYPE_PASS'), service.getTypeMessage('error'));
                            return false;
                        }
                    }
                    return true;
                },
                validateEmail: function (email) {
                    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    return re.test(email);
                },
                validatePass: function (pass) {
                    var re = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/;
                    return re.test(pass);
                }
            }

            $scope.convertToDate = function (date)
            {
                return service.formatoFecha(date);
            }

            $scope.userSettings.get(false);

        }]
    )
})();