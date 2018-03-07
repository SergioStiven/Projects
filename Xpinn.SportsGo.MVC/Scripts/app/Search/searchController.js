(function () {
    'use strict';
    var dependencies = ["oi.select", "rzModule"];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
        .controller('searchController',
        ["$rootScope", "$translate", "$sce", "$scope", "service", '$http', '$timeout', function ($rootScope, $translate, $sce, $scope, service, $http, $timeout) {

            var controller = 'Search/';

            $rootScope.$on('GetMoreRows', function (event, obj) {
                $scope.Search.getMore();
            });

            $scope.loadingPage = true;

            $scope.Events = {
                showEvent: false,
                imageDefault: '../Content/assets/img/eventsDefault.png',
                selectedEvent: {},
                getEvent: function (event) {
                    if (event.UrlImagenPerfil === '')
                        event.UrlArchivo = $scope.Events.Personas.UrlImagenPerfil;

                    $scope.Events.selectedEvent = event;
                    $("#ms-countdown").countdown(service.formatoFecha(event.FechaTerminacion), function (l) {
                        $(this).html(l.strftime(
                            '<ul class="coming-date coming-date-black">' +
                            '<li style="font-size:25px;">%D <span>Dias</span></li>' +
                            '<li style="font-size:25px;" class="colon">:</li>' +
                            '<li style="font-size:25px;">%H <span>Horas</span></li>' +
                            '<li style="font-size:25px;" class="colon">:</li>' +
                            '<li style="font-size:25px;">%M <span>Minutos</span></li>' +
                            '<li style="font-size:25px;" class="colon">:</li>' +
                            '<li style="font-size:25px;">%S <span>Seg</span></li>' +
                            '</ul>'))
                    });

                    $scope.Events.showEvent = true;
                }
            }

            $scope.Search = {
                filter: {
                    TakeIndexBase: 50, SkipIndexBase: -50, CategoriasParaBuscar: [], CountriesForSearch: [], CategoriesForSearch: [],
                    PaisesParaBuscar: [], EstaturaInicial: 120, EstaturaFinal: 220, PesoInicial: 40, PesoFinal: 120
                },
                list: [],
                currentProfile: 1,
                showFilters: false,
                OperationIsAuthorizedByPlan: true,
                tabCandidates: true,
                tabGroups: false,
                tabEvents: false,
                endList: false,
                isBusy: false,
                get: function (profileId) {
                    $scope.Search.list = [];
                    $scope.loadingPage = true;
                    $scope.Search.OperationIsAuthorizedByPlan = true;
                    $scope.Search.tabCandidates = false;
                    $scope.Search.tabGroups = false;
                    $scope.Search.tabEvents = false;
                    $scope.Search.isBusy = false;
                    $scope.Search.endList = false;
                    $scope.Search.filter.SkipIndexBase = -50;
                    if ($scope.Search.filter.CategoriesForSearch.length > 0)
                        $scope.Search.filter.CategoriasParaBuscar = $scope.Search.filter.CategoriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Search.filter.CategoriasParaBuscar = [];

                    if ($scope.Search.filter.CountriesForSearch.length > 0)
                        $scope.Search.filter.PaisesParaBuscar = $scope.Search.filter.CountriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Search.filter.PaisesParaBuscar = [];

                    switch (profileId) {
                        case 1: // Candidates
                            $scope.Search.tabCandidates = true;
                            $scope.Search.getCandidates();
                            break;
                        case 2: // Groups
                            $scope.Search.tabGroups = true;
                            $scope.Search.getGroups();
                            break;
                        case 3: // Events
                            $scope.Search.tabEvents = true;
                            $scope.Search.getEvents();
                            break;
                        default:
                            break;
                    }

                    $scope.Search.currentProfile = profileId;
                },
                getCandidates: function () {
                    if ($scope.Search.isBusy || $scope.Search.endList) return;
                    $scope.Search.isBusy = true;
                    $scope.Search.filter.SkipIndexBase += 50;
                    service.post(controller + 'listarCandidates', $scope.Search.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < 50)
                                $scope.Search.endList = true;
                            $scope.Search.list = $scope.Search.list.concat(res.data.list);
                        } else if (!res.data.AuthorizedByPlan) {
                            $scope.Search.OperationIsAuthorizedByPlan = false;
                        } else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'));
                        }
                        $scope.loadingPage = false;
                        $scope.Search.isBusy = false;
                    })
                },
                getGroups: function () {
                    if ($scope.Search.isBusy || $scope.Search.endList) return;
                    $scope.Search.isBusy = true;
                    $scope.Search.filter.SkipIndexBase += 50;
                    service.post(controller + 'listarGrupos', $scope.Search.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < 50)
                                $scope.Search.endList = true;
                            $scope.Search.list = $scope.Search.list.concat(res.data.list);
                        } else if (!res.data.AuthorizedByPlan) {
                            $scope.Search.OperationIsAuthorizedByPlan = false;
                        } else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'));
                        }
                        $scope.loadingPage = false;
                        $scope.Search.isBusy = false;
                    })
                },
                getEvents: function () {
                    if ($scope.Search.isBusy || $scope.Search.endList) return;
                    $scope.Search.isBusy = true;
                    $scope.Search.filter.SkipIndexBase += 50;
                    $scope.Search.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'listarEvents', $scope.Search.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < 50)
                                $scope.Search.endList = true;
                            $scope.Search.list = $scope.Search.list.concat(res.data.list);
                            angular.forEach($scope.Search.list, function (value, key) {
                                value.Personas = {};
                                value.Personas.Nombres = value.Titulo;
                                value.Personas.CiudadResidencia = value.Descripcion;
                                value.Personas.UrlImagenPerfil = value.Grupos.Personas.UrlImagenPerfil === '' ? $scope.Events.imageDefault : value.Grupos.Personas.UrlImagenPerfil;
                            });

                        } else if (!res.data.AuthorizedByPlan) {
                            $scope.Search.OperationIsAuthorizedByPlan = false;
                        } else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'));
                        }
                        $scope.loadingPage = false;
                        $scope.Search.isBusy = false;
                    })
                },
                getMore: function () {
                    if ($scope.Search.tabCandidates){
                        $scope.Search.getCandidates();
                    } else if ($scope.Search.tabGroups){
                        $scope.Search.getGroups();
                    } else if ($scope.Search.tabEvents){
                        $scope.Search.getEvents();
                    }
                },
                clearFiltersAndSearchByProfile: function (ProfileId) {
                    $scope.Search.filter = {
                        TakeIndexBase: 50, SkipIndexBase: -50, IdiomaBase: service.getLenguageFromNavigator(), CategoriasParaBuscar: [],
                        PaisesParaBuscar: [], EstaturaInicial: 120, EstaturaFinal: 220, PesoInicial: 40, PesoFinal: 120, CountriesForSearch: [], CategoriesForSearch: []
                    };
                    $scope.Search.get(ProfileId);
                    $scope.Events.showEvent = false;
                },
                viewProfile: function (obj) {
                    if ($scope.Search.currentProfile === 3) {
                        $scope.Events.getEvent(obj);
                        return;
                    }

                    var persons = { Consecutivo: 0, CodigoIdioma: 0 };
                    persons.Consecutivo = obj.Personas.Consecutivo;
                    persons.CodigoIdioma = obj.Personas.IdiomaDeLaPersona;

                    service.post(controller + 'SaveSearchIdInSession', persons, function (res) {
                        if (!res.data.Success)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('warning'))
                    });
                }
            }

            $scope.Search.getCandidates();

        }])
        .directive('filtersGroupsAndEvents', filtersGroupsAndEvents)
        .directive("scroll", function ($window, $rootScope) {
            return function (scope, element, attrs) {
                angular.element($window).bind("scroll", function () {
                    if ($(window).scrollTop() + $(window).height() > $(document).height() - 1200) {
                        $rootScope.$emit("GetMoreRows", {});
                    }
                    scope.$apply();
                });
            };
        })
        .directive('filtersCandidate', filtersCandidate);
    filtersGroupsAndEvents.$inject = ['service'];
    filtersCandidate.$inject = ['service', '$timeout'];
    function filtersGroupsAndEvents(service) {
        return {
            restrict: 'E',
            scope: {
                onFilter: '&',
                onClearFilter: '&',
                countryForSearch: '=',
                categoryForSearch: '=',
                nameForSearch: '='
            },
            compile: function (tElem, tAttrs) {
                return {
                    pre: function (scope, element, attr) {
                        scope.lists = {
                            countries: [],
                            categories: [],
                            filter: { TakeIndexBase: 100, SkipIndexBase: 0 },
                            getCountries: function () {
                                service.post('Administration/GetListCountries', null, function (res) {
                                    if (res.data.Success)
                                        scope.lists.countries = res.data.list;
                                    else
                                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                })
                            },
                            getCategories: function () {
                                service.get('Administration/GetListCategories', null, function (res) {
                                    if (res.data.Success)
                                        scope.lists.categories = res.data.list;
                                    else
                                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                })
                            },
                            fillList: function () {
                                scope.lists.getCountries();
                                scope.lists.getCategories();
                            }
                        }
                        element.on('load', scope.lists.fillList());
                    },
                    post: function (scope, iElem, iAttrs) {
                        //console.log(name + ': post link');
                    }
                }
            },
            templateUrl: service.urlBase + 'Events/FiltersEvents',
            replace: true
        }
    };
    function filtersCandidate(service, $timeout) {
       
        return {
            restrict: 'E',
            scope: {
                onFilter: '&',
                onClearFilter: '&',
                countryForSearch: '=',
                categoryForSearch: '=',
                nameForSearch: '=',
                weightStartForSearch: '=',
                weightEndForSearch: '=',
                heightStartForSearch: '=',
                heightEndForSearch: '='
            },
            compile: function (tElem, tAttrs) {
                return {
                    pre: function (scope, element, attr) {
                        scope.lists = {
                            countries: [],
                            categories: [],
                            filter: { TakeIndexBase: 100, SkipIndexBase: 0 },
                            getCountries: function () {
                                service.post('Administration/GetListCountries', null, function (res) {
                                    if (res.data.Success)
                                        scope.lists.countries = res.data.list;
                                    else
                                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                })
                            },
                            getCategories: function () {
                                service.get('Administration/GetListCategories', null, function (res) {
                                    if (res.data.Success)
                                        scope.lists.categories = res.data.list;
                                    else
                                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                })
                            },
                            fillList: function () {
                                scope.lists.getCountries();
                                scope.lists.getCategories();
                            }
                        }

                        scope.minRangeSlider = {
                            optionsHeight: {
                                floor: 120,
                                ceil: 220,
                                step: 1
                            },
                            optionsWeight : {
                                floor: 40,
                                ceil: 120,
                                step: 1
                            }
                        };
                        
                        if (scope.heightStartForSearch == undefined)
                            scope.heightStartForSearch = 0;
                        if (scope.heightEndForSearch == undefined)
                            scope.heightEndForSearch = 200;
                        
                        if (scope.weightStartForSearch == undefined)
                            scope.weightStartForSearch = 0;
                        if (scope.weightEndForSearch == undefined)
                            scope.weightEndForSearch = 200;

                        element.on('load', scope.lists.fillList());
                    },
                    post: function (scope, iElem, iAttrs) {
                        scope.refreshSlider = function () {
                            $timeout(function () {
                                scope.$broadcast('rzSliderForceRender');
                            },300);
                        }
                        scope.refreshSlider();
                    }
                }
            },
            templateUrl: service.urlBase + 'Candidate/FiltersCandidate',
            replace: true
        }
    };
})();