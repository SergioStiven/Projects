(function () {
    'use strict';
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

            $scope.myPersonId = 0;
            $scope.personIdActiveChat = 0;
            $scope.showButtonSearch = false;
            //$scope.titleSearchButton = "Buscar noticia";
            $scope.titleSearchButton = "Buscar en mi biografia";

            $scope.$parent.$on('onGetMessage', function (e, chatReceived) {
                if (chatReceived.CodigoPersonaOwner === $scope.personIdActiveChat) return;
                if (chatReceived.CodigoPersonaNoOwner === $scope.myPersonId) {
                    var urlImage = chatReceived.PersonasOwner.UrlImagenPerfil === '' ? service.urlImageProfileDefault : chatReceived.PersonasOwner.UrlImagenPerfil;
                    service.showNotiChat(chatReceived.UltimoMensaje.Mensaje, chatReceived.PersonasOwner.NombreYApellido, urlImage);
                }
            });

            $scope.$parent.$on('onGetProfile', function (e, profile) {
                $scope.myPersonId = profile.profile.PersonaDelUsuario.Consecutivo;
            });

            $scope.$parent.$on('onGetActiveChat', function (e, person) {
                $scope.personIdActiveChat = person.personId;
            });

            $scope.$parent.$on('viewSearchBar', function (e, person) {
                $scope.showButtonSearch = true;
            });

            $scope.Search = {
                filter: { SkipIndexBase: 0, TakeIndexBase: 0, IdentificadorParaBuscar: '' },
                list: [],
                searchInfo: function () {
                    $scope.Search.filter.IdentificadorParaBuscar = $scope.toSearch;
                    $rootScope.$emit("onSearchNews", { filter: $scope.Search.filter });
                }
            }

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
                                if (res.data.list[i].Descripcion !== null && res.data.list[i].Descripcion.length > 130) {
                                    res.data.list[i].Descripcion = res.data.list[i].Descripcion.substring(0, 130) + '...';
                                }
                                if (res.data.list[i].Titulo !== null && res.data.list[i].Titulo.length > 90) {
                                    res.data.list[i].Titulo = res.data.list[i].Titulo.substring(0, 90) + '...';
                                }
                                if (res.data.list[i].UrlArchivo === '') {
                                    res.data.list[i].UrlArchivo = service.urlImageProfileDefault;
                                }
                                if (res.data.list[i].TipoDeLaNotificacion === 1) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_PLAN_NEW');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_PLAN_NEW_DESCRIPTION', {
                                        name: res.data.list[i].NombreApellidoPersona
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 2) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_ADD_PERSON');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_ADD_PERSON_DESCRIPTION', {
                                        name: res.data.list[i].NombreApellidoPersona
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 3) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_REMOVE_PERSON');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_REMOVE_PERSON_DESCRIPTION', {
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
                                } else if (res.data.list[i].TipoDeLaNotificacion === 9) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_EVENT_REGISTRATION');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_EVENT_REGISTRATION_DESCRIPTION', {
                                        name: res.data.list[i].NombreApellidoPersona,
                                        event: res.data.list[i].TituloEvento
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 10) {
                                    res.data.list[i].Titulo = $translate.instant('LBL_EVENT_UNSUBSCRIPTION');
                                    res.data.list[i].Descripcion = $translate.instant('LBL_EVENT_UNSUBSCRIPTION_DESCRIPCION', {
                                        name: res.data.list[i].NombreApellidoPersona,
                                        event: res.data.list[i].TituloEvento
                                    });
                                } else if (res.data.list[i].TipoDeLaNotificacion === 11) {
                                    res.data.list[i].UrlArchivo = service.urlBase + '/Content/assets/img/Rss-feed.svg';
                                }
                            }
                            $scope.Notifications.list = $scope.Notifications.list.concat(res.data.list);
                        }
                        else {
                            //console.log(res.data.Message);
                        }
                    })
                },
                redirectToUrl: function (obj) {
                    if (obj.TipoDeLaNotificacion === 4 || obj.TipoDeLaNotificacion === 11) {
                        if (obj.UrlPublicidad !== '' && obj.UrlPublicidad !== null)
                            window.open(obj.UrlPublicidad, '_blank');
                    } else if (obj.TipoDeLaNotificacion === 2 || obj.TipoDeLaNotificacion === 3) {
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
        ['$translate', "$sce", "$scope", "$rootScope", "service", "$filter", "$timeout", function ($translate, $sce, $scope, $rootScope, service, $filter, $timeout) {

            var controller = 'Profile/';

            $scope.Profile = {
                info: {},
                defaulImageProfile: service.urlImageProfileDefault,
                defaulImageBanner: service.urlImageBannerDefault,
                get: function () {
                    service.get(controller + 'GetProfile', null, function (res) {
                        if (res.data.Success) {
                            $translate.use(res.data.obj.PersonaDelUsuario.CodigoIdioma.toString()); // Translate by language user
                            service.initializeSignalR(res.data.obj.PersonaDelUsuario.Consecutivo); // Initialize SignalR
                            $scope.Profile.info = res.data.obj;
                            $scope.Profile.setImageBanner();
                            $rootScope.$emit('onGetProfile', { profile: $scope.Profile.info });
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                setImageBanner: function () {
                    if ($scope.Profile.info.PersonaDelUsuario.UrlImagenBanner != '')
                        $('#divBannerLeftMenu').css("background-image", "url(" + $scope.Profile.info.PersonaDelUsuario.UrlImagenBanner + ")");
                    else
                        $('#divBannerLeftMenu').css("background-image", "url(" + $scope.Profile.defaulImageBanner + ")");
                },
                changeImagePRofile: function (image) {
                    $scope.Profile.info.PersonaDelUsuario.UrlImagenPerfil = image;
                },
                changeImageBanner: function (image) {
                    $scope.Profile.info.PersonaDelUsuario.UrlImagenBanner = image;
                    $scope.Profile.setImageBanner();
                }
            }
            
            $scope.$parent.$on('changeImageProfile', function (e, obj) {
                $scope.Profile.changeImagePRofile(obj.image);
            });

            $scope.$parent.$on('changeImageBanner', function (e, obj) {
                $scope.Profile.changeImageBanner(obj.image);
            });

            $scope.Profile.get();

        }]);
})();