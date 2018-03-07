(function () {
    'use strict';

    var dependencies = [
            "ngSanitize",
            "com.2fdevs.videogular",
            "com.2fdevs.videogular.plugins.controls",
            "com.2fdevs.videogular.plugins.overlayplay",
            "com.2fdevs.videogular.plugins.poster"
    ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
        .controller('HomeCtrl',
            ['$translate', "$scope", "service", "$sce", "$rootScope", function ($translate, $scope, service, $sce, $rootScope) {

                var controller = 'Home/';

                $scope.loading = true;
                $scope.videogularTheme = service.urlBase + 'Scripts/app/lib/bower_components/videogular-themes-default/videogular.css';

                $scope.ConverToDate = function (date) {
                    return service.formatoFecha(date);
                }

                $rootScope.$on('GetNews', function (event, obj) {
                    $scope.New.get();
                });

                $rootScope.$on('onSearchNews', function (event, obj) {
                    $scope.New.searchNew(obj.filter.IdentificadorParaBuscar);
                });

                $scope.New = {
                    isBusy: false,
                    UrlImagePerfilDefault: '../Content/assets/img/demo/avatar.png',
                    list: [],
                    adsRight:[],
                    endList: false,
                    filter: { SkipIndexBase: 0, TakeIndexBase: 0 },
                    get: function () {
                        if ($scope.New.isBusy || $scope.New.endList) return;
                        $scope.New.isBusy = true;
                        $scope.New.filter.SkipIndexBase = $scope.New.filter.TakeIndexBase;
                        $scope.New.filter.TakeIndexBase += 4;
                        $scope.New.filter.ZonaHorariaGMTBase = service.getTimeZone();
                        service.post(controller + 'GetNewsTimeLine', $scope.New.filter, function (res) {
                            if (res.data.Success) {
                                if (res.data.list.length === 0){
                                    $scope.New.endList = true;
                                    return;
                                }
                                if ($scope.New.filter.SkipIndexBase === 0) {
                                    for (var i = 0; i < res.data.list.length; i++) {
                                        if (res.data.list[i].NoticiaLateral) {
                                            if (res.data.list[i].UrlArchivoPublicacion != '' && !res.data.list[i].EsVideo) {
                                                $scope.New.adsRight.push(res.data.list[i]);
                                                res.data.list.splice(i, 1);
                                                i--;
                                            }
                                        }
                                    }
                                }
                                $scope.New.list = $scope.New.list.concat(res.data.list);
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                            $scope.New.isBusy = false;
                        });
                    },
                    updateCounterAdSeen: function(AdId){
                        service.post(controller + 'UpdateCounterAdSeen', { Consecutivo: AdId }, function (res) {
                            if (!res.data.Success)
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    },
                    searchNew: function (name) {
                        $scope.New.endList = false;
                        $scope.New.filter.SkipIndexBase = 0;
                        $scope.New.filter.TakeIndexBase = 0;
                        $scope.New.filter.IdentificadorParaBuscar = name;
                        $scope.New.list = [];
                        $scope.New.get();
                    },
                    redirectToUrl: function (obj) {
                        if (obj.EsNoticia || obj.EsPublicidad) {
                            if (obj.UrlRedireccionar !== '' && obj.UrlRedireccionar !== null) {
                                if (obj.EsPublicidad)
                                    $scope.New.updateCounterAdSeen(obj.ConsecutivoPublicacion);
                                window.open(obj.UrlRedireccionar, '_blank');
                            }
                        } else if (obj.EsEvento) {
                            $scope.Event.getEventDetail(obj.ConsecutivoPublicacion);
                        } else {
                            var persons = { Consecutivo: 0, CodigoIdioma: 0 };
                            persons.Consecutivo = obj.ConsecutivoPersona;
                            persons.CodigoIdioma = service.getLenguageFromNavigator();
                            service.post('Search/SaveSearchIdInSession', persons, function (res) {
                                if (!res.data.Success)
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'))
                            });
                        }
                    }
                }

                $scope.Event = {
                    eventDetail: {},
                    IWillAttend: false,
                    viewButtonsAttendEvent: true,
                    getEventDetail: function (eventId) {
                        service.post('Events/GetEventDetail', { Consecutivo: eventId, ZonaHorariaGMTBase: service.getTimeZone() }, function (res) {
                            if (res.data.Success) {
                                $scope.Event.eventDetail = res.data.obj;
                                $scope.Event.validateIfIWillAttend(eventId);
                                $("#ms-countdown").countdown(service.formatoFecha($scope.Event.eventDetail.FechaInicio), function (l) {
                                    $(this).html(l.strftime(
                                        '<ul class="coming-date coming-date-black">' +
                                            '<li style="font-size:15px;">%D <span>Dias</span></li>' +
                                            '<li style="font-size:15px;" class="colon">:</li>' +
                                            '<li style="font-size:15px;">%H <span>Horas</span></li>' +
                                            '<li style="font-size:15px;" class="colon">:</li>' +
                                            '<li style="font-size:15px;">%M <span>Minutos</span></li>' +
                                            '<li style="font-size:15px;" class="colon">:</li>' +
                                            '<li style="font-size:15px;">%S <span>Seg</span></li>' +
                                        '</ul>'))
                                });
                            }
                            else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    },
                    validateIfIWillAttend: function (eventId) {
                        service.post('Events/ValidateIfIWillAttend', { CodigoEvento: eventId }, function (res) {
                            if (res.data.Success) {
                                $scope.Event.IWillAttend = res.data.obj.Existe;
                                $scope.Event.viewButtonsAttendEvent = true;
                            }
                            else {
                                $scope.Event.viewButtonsAttendEvent = false;
                            }
                            $('#modalEventDetail').modal('toggle');
                        })
                    },
                    saveAssistantEvent: function (event) {
                        service.post('Events/CreateAssistantEvent', { CodigoEvento: event.Consecutivo }, function (res) {
                            if (res.data.Success) {
                                $scope.Event.eventDetail.NumeroEventosAsistentes += 1;
                                $scope.Event.IWillAttend = true;
                            }
                            else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    },
                    deleteAssistantEvent: function (event) {
                        service.post('Events/DeleteAssistantEvent', { CodigoEvento: event.Consecutivo }, function (res) {
                            if (res.data.Success) {
                                $scope.Event.eventDetail.NumeroEventosAsistentes -= 1;
                                $scope.Event.IWillAttend = false;
                            }
                            else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    }
                }

                $scope.New.get();

            }]
        )
        .directive('newsList', newsList)
        .directive("scroll", function ($window, $rootScope) {
            return function (scope, element, attrs) {
                angular.element($window).bind("scroll", function () {
                    if ($(window).scrollTop() + $(window).height() > $(document).height() - 1200) {
                        $rootScope.$emit("GetNews", { toSearch: 'kk' });
                        //scope.$apply(attrs.scroll);
                    }
                    scope.$apply();
                });
            };
        })
        .directive('eventDetail', eventDetail);
        newsList.$inject = ['service'];
        eventDetail.$inject = ['service'];
        function newsList (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Home/News'
            }
        };
        function eventDetail(service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Events/EventDetail'
            }
        };
})();
