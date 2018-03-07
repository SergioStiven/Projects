(function () {
    'use strict';
    var dependencies =
        [
                "ngSanitize",
                "com.2fdevs.videogular",
                "com.2fdevs.videogular.plugins.controls",
                "com.2fdevs.videogular.plugins.overlayplay",
                "com.2fdevs.videogular.plugins.poster",
                "oi.select",
                "ngFileUpload",
                "ui.bootstrap.datetimepicker"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    // Controller
    app.controller('eventsController',
        ["$translate", "$sce", "$scope", "$rootScope", "service", "Upload", "$timeout", "$filter", function ($translate, $sce, $scope, $rootScope, service, Upload, $timeout, $filter) {

            var controller = 'Events/';
            $scope.videogularTheme = service.urlBase + 'Scripts/app/lib/bower_components/videogular-themes-default/videogular.css';

            $scope.convertToDate = function (date) {
                return service.formatoFecha(date);
            }

            $rootScope.$on('GetMoreEvents', function (event, obj) {
                $scope.Event.getMoreMyEvents();
            });

            $scope.Profile = {
                info: { UrlImagenPerfilAdmin: '', PersonaDelUsuario: { NombreYApellido: '' } },
                get: function () {
                    service.get('Profile/GetProfile', null, function (res) {
                        if (res.data.Success) {
                            $scope.Profile.info = res.data.obj;
                            $translate.use($scope.Profile.info.PersonaDelUsuario.CodigoIdioma.toString());
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })

                }
            }

            $scope.Event = {
                list: [],
                myEventsList: [],
                myAssistantEventsList: [],
                countries: [],
                categories: [],
                showEventDetail: false,
                eventDetail: {},
                addEvent: false,
                tabAllEvents: true,
                tabMyEvents: false,
                tabAssistantEvents: false,
                tabCreateEvent: false,
                IWillAttend: false,
                viewButtonsAttendEvent: true,
                isBusy: false,
                endList: false,
                post: {
                    CodigoPais: 0, CategoriasEventos: [], CodigoArchivo: 0, Titulo: '', Descripcion: '', FechaInicio: '', FechaTerminacion: '', Pais: { Consecutivo: 0 }
                },
                toDelete: {},
                loadTab: function (tab) {
                    $scope.Event.clearFilters();
                    $scope.Event.disabledTabs();
                    $scope.Event.endList = false;
                    switch (tab) {
                        case 1:
                            $scope.Event.get();
                            break;
                        case 2:
                            $scope.Event.getMyEvents();
                            break;
                        case 3:
                            $scope.Event.getMyAssistantEventsList();
                            break;
                        case 4:
                            $scope.Event.disabledTabs();
                            $scope.Event.tabCreateEvent = true;
                            break;
                        default:
                            $scope.Event.get();
                            break;
                    }
                },
                filter: { TakeIndexBase: 20, SkipIndexBase: 0, CategoriasParaBuscar: [], PaisesParaBuscar: [], IdentificadorParaBuscar: '', CountriesForSearch: [], CategoriesForSearch: [] },
                get: function () {
                    $scope.Event.tabAllEvents = true;
                    $scope.Event.viewButtonsAttendEvent = true;
                    $scope.Event.list = [];
                    $scope.Event.showEventDetail = false;
                    if ($scope.Event.isBusy || $scope.Event.endList) return;
                    $scope.Event.isBusy = true;
                    if ($scope.Event.filter.CategoriesForSearch.length > 0)
                        $scope.Event.filter.CategoriasParaBuscar = $scope.Event.filter.CategoriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.CategoriasParaBuscar = [];
                    if ($scope.Event.filter.CountriesForSearch.length > 0)
                        $scope.Event.filter.PaisesParaBuscar = $scope.Event.filter.CountriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.PaisesParaBuscar = [];

                    $scope.Event.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetAllEvents', $scope.Event.filter, function (res) {
                        if (res.data.Success) {
                            if (($scope.Event.filter.SkipIndexBase + $scope.Event.filter.TakeIndexBase) > res.data.list[0].NumeroRegistrosExistentes)
                                $scope.Event.endList = true;
                            $scope.Event.list = $scope.Event.list.concat(res.data.list);
                            $scope.Event.reloadMasonry();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.Event.isBusy = false;
                    })
                },
                getMyEvents: function () {
                    $scope.Event.tabMyEvents = true;
                    $scope.Event.showEventDetail = false;
                    $scope.Event.viewButtonsAttendEvent = false;

                    if ($scope.Event.isBusy || $scope.Event.endList) return;
                    $scope.Event.isBusy = true;

                    if ($scope.Event.filter.CategoriesForSearch.length > 0)
                        $scope.Event.filter.CategoriasParaBuscar = $scope.Event.filter.CategoriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.CategoriasParaBuscar = [];
                    if ($scope.Event.filter.CountriesForSearch.length > 0)
                        $scope.Event.filter.PaisesParaBuscar = $scope.Event.filter.CountriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.PaisesParaBuscar = [];

                    $scope.Event.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetListEventsByGroup', $scope.Event.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < 20) {
                                $scope.Event.endList = true;
                            }
                            $scope.Event.myEventsList = $scope.Event.myEventsList.concat(res.data.list);
                            //$scope.Event.reloadMasonry();
                        }   
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.Event.isBusy = false;
                    })
                },
                getMyAssistantEventsList: function () {
                    $scope.Event.tabAssistantEvents = true;
                    $scope.Event.showEventDetail = false;
                    $scope.Event.viewButtonsAttendEvent = false;

                    if ($scope.Event.filter.CategoriesForSearch.length > 0)
                        $scope.Event.filter.CategoriasParaBuscar = $scope.Event.filter.CategoriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.CategoriasParaBuscar = [];
                    if ($scope.Event.filter.CountriesForSearch.length > 0)
                        $scope.Event.filter.PaisesParaBuscar = $scope.Event.filter.CountriesForSearch.map(function (a) { return a.Consecutivo; });
                    else
                        $scope.Event.filter.PaisesParaBuscar = [];

                    $scope.Event.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetListAssistantEvents', $scope.Event.filter, function (res) {
                        if (res.data.Success) {
                            $scope.Event.myAssistantEventsList = res.data.list;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getEventDetail: function (event) {
                    service.post(controller + 'GetEventDetail', { Consecutivo: event.Consecutivo, ZonaHorariaGMTBase: service.getTimeZone() }, function (res) {
                        if (res.data.Success) {
                            $scope.Event.eventDetail = res.data.obj;
                            $scope.Event.validateIfIWillAttend(event);
                            $("#ms-countdown").countdown(service.formatoFecha($scope.Event.eventDetail.FechaInicio), function (l) {
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
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getNextEvents: function () {
                    if ($scope.Event.endList) return;
                    $scope.Event.filter.SkipIndexBase += 20;
                    $scope.Event.get();
                },
                getBeforeEvents: function () {
                    if (($scope.Event.filter.SkipIndexBase - 20) < 0) return;
                    $scope.Event.endList = false;
                    $scope.Event.filter.SkipIndexBase -= 20;
                    $scope.Event.get();
                },
                searchEvent: function (callback) {
                    $scope.Event.filter.SkipIndexBase = 0;
                    $scope.Event.endList = false;
                    if ($scope.Event.tabAllEvents) {
                        $scope.Event.list = [];
                    } else if ($scope.Event.tabMyEvents) {
                        $scope.Event.myEventsList = [];
                    } else if ($scope.Event.tabMyEvents) {
                        $scope.Event.myAssistantEventsList = [];
                    }
                    callback();
                },
                getMoreMyEvents: function () {
                    if ($scope.Event.tabMyEvents) {
                        $scope.Event.filter.SkipIndexBase += 20;
                        $scope.Event.getMyEvents();
                    } else if ($scope.Event.tabMyEvents) {
                        $scope.Event.filter.SkipIndexBase += 20;
                        $scope.Event.tabAssistantEvents();
                    }
                },
                validateIfIWillAttend: function (event) {
                    service.post(controller + 'ValidateIfIWillAttend', { CodigoEvento: event.Consecutivo, CodigoPersona: event.Grupos.CodigoPersona }, function (res) {
                        if (res.data.Success) {
                            $scope.Event.IWillAttend = res.data.obj.Existe;
                            $scope.Event.viewButtonsAttendEvent = true;
                        }
                        else {
                            $scope.Event.viewButtonsAttendEvent = false;
                        }
                        $scope.Event.showEventDetail = true;
                    })
                },
                uploadFile: function () {
                    if ($scope.Event.isBusy) return;
                    if (!$scope.Event.validateForm($scope.Event.post))
                        return;
                    if (!$scope.Event.picFilePost) {
                        $scope.Event.post.CodigoArchivo = null;
                        $scope.Event.save();
                        return;
                    }
                    $scope.Event.isBusy = true;
                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadVideoToControlDuration',
                        data: { file: $scope.Event.picFilePost }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.Event.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                            $scope.Event.save();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }

                        $scope.Event.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                updateFile: function (post, picFilePost) {
                    $scope.Event.isBusy = true;
                    if (!$scope.Event.validateForm(post)) {
                        $scope.Event.isBusy = false;
                        return;
                    }
                    if (!picFilePost) {
                        $scope.Event.update(post);
                        return;
                    }

                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadFile',
                        data: { file: picFilePost, ConsecutivoArchivo: post.CodigoArchivo }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.Event.update(post);
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }

                        $scope.Event.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                save: function () {
                    $scope.Event.isBusy = true;
                    $scope.Event.post.CodigoPais = $scope.Event.post.Pais.Consecutivo;
                    $scope.Event.post.FechaInicio = $filter('date')($scope.Event.post.FechaInicio, 'yyyy-MM-dd HH:mm:ss');
                    service.post(controller + 'CreateEvent', $scope.Event.post, function (res) {
                        if (res.data.Success) {
                            $scope.Event.clearFields();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.Event.isBusy = false;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.Event.isBusy = false;
                        }
                    })
                },
                update: function (post) {
                    service.post(controller + 'CreatePost', post, function (res) {
                        if (res.data.Success) {
                            $scope.New.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.New.isBusy = false;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.New.isBusy = false;
                        }
                    })
                },
                delete: function () {
                    service.post(controller + 'DeletePost', { Consecutivo: $scope.New.toDelete.Consecutivo }, function (res) {
                        if (res.data.Success) {
                            $scope.New.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                saveAssistantEvent: function (event) {
                    service.post(controller + 'CreateAssistantEvent', { CodigoEvento: event.Consecutivo }, function (res) {
                        if (res.data.Success) {
                            $scope.Event.eventDetail.NumeroEventosAsistentes += 1;
                            $scope.Event.IWillAttend = true;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                deleteAssistantEvent: function (event) {
                    service.post(controller + 'DeleteAssistantEvent', { CodigoEvento: event.Consecutivo }, function (res) {
                        if (res.data.Success) {
                            $scope.Event.eventDetail.NumeroEventosAsistentes -= 1;
                            $scope.Event.IWillAttend = false;
                            $scope.Event.filter.ZonaHorariaGMTBase = service.getTimeZone();
                            service.post(controller + 'GetListAssistantEvents', $scope.Event.filter, function (res) {
                                if (res.data.Success) $scope.Event.myAssistantEventsList = res.data.list;
                            })
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                validateIfIsGroup: function () {
                    service.get(controller + 'IsGroup', null, function (res) {
                        $scope.Event.isGroup = res.data.Success;
                    })
                },
                returnToList: function () {
                    if ($scope.Event.tabAllEvents)
                        $scope.Event.get();
                    $scope.Event.showEventDetail = false;
                },
                filteredCategories: function (toFilter) {
                    var idsCategories = toFilter.map(function (a) { return a.CodigoCategoria; });
                    return $scope.lists.categories.filter(function (category) { return idsCategories.indexOf(category.Consecutivo) === -1 });
                },
                filteredCountries: function (toFilter) {
                    var idsCountries = toFilter.map(function (a) { return a.CodigoPais; });
                    return $scope.lists.countries.filter(function (country) { return idsCountries.indexOf(country.Consecutivo) === -1 });
                },
                validateForm: function (form) {
                    var today = new Date();
                    if (form.Titulo === '') {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_TITLE'), service.getTypeMessage('error'));
                        return false;
                    }
                    else if (form.CategoriasEventos.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                        return false;
                    } else if (form.Pais.Consecutivo === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_COUNTRIES'), service.getTypeMessage('error'));
                        return false;
                    } else if (form.FechaInicio === '' || form.FechaTerminacion === '') {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_DATE'), service.getTypeMessage('error'));
                        return false;
                    }
                    else if ($scope.Event.post.FechaInicio < today) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_RAGE_DATE'), service.getTypeMessage('error'));
                        return false;
                    }
                    else if ($scope.Event.post.FechaTerminacion < $scope.Event.post.FechaInicio) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_END_DATE'), service.getTypeMessage('error'));
                        return false;
                    }
                    return true;
                },
                getCountries: function () {
                    service.post('Administration/GetListCountries', { CodigoIdiomaUsuarioBase: 1 }, function (res) {
                        if (res.data.Success)
                            $scope.Event.countries = res.data.list;
                        else
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getCategories: function () {
                    service.get('Administration/GetListCategories', null, function (res) {
                        if (res.data.Success) {
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].CodigoCategoria = res.data.list[i].Consecutivo;
                            }
                            $scope.Event.categories = res.data.list;
                        }
                        else
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                clearFields: function () {
                    $scope.Event.post = {
                        CodigoPais: 0, CategoriasEventos: [], CodigoArchivo: 0, Titulo: '', Descripcion: '', FechaInicio: '', FechaTerminacion: '', Pais: { Consecutivo: 0 }
                    };
                    $scope.Event.picFilePost = null;
                },
                clearFilters: function () {
                    $scope.Event.filter = { TakeIndexBase: 20, SkipIndexBase: 0, CategoriasParaBuscar: [], PaisesParaBuscar: [], IdentificadorParaBuscar: '', CountriesForSearch: [], CategoriesForSearch: [] };
                },
                disabledTabs: function () {
                    $scope.Event.tabAllEvents = false;
                    $scope.Event.tabMyEvents = false;
                    $scope.Event.tabAssistantEvents = false;
                },
                reloadMasonry: function () {
                    var $container = $('.masonry-container').masonry();
                    $container.masonry('destroy');
                    setTimeout(function () {
                        var $container = $('.masonry-container').masonry();

                        function resizeContainer() {
                            $('.masonry-container').animate({ width: '300px' }, function () {
                                $container.masonry();
                            });
                        }
                    }, 3000);
                }
            }
            $scope.Event.validateIfIsGroup();
            $scope.Profile.get();
            $scope.Event.getCountries();
            $scope.Event.getCategories();
        }]);

    // Directives
    app
        .directive('listOfAllEvents', ['service', function (service) {
            return {
                restrict: 'E',
                compile: function (tElem, tAttrs) {
                    return {
                        pre: function (scope, iElem, iAttrs) {
                            scope.Event.get();
                            //scope.lists.fillList();
                        },
                        post: function (scope, iElem, iAttrs) {
                            //console.log(name + ': post link');
                        }
                    }
                },
                templateUrl: service.urlBase + 'Events/ListOfAllEvents'
            }
        }])
        .directive('listOfMyEvents', ['service', function (service) {
            return {
                restrict: 'E',
                compile: function (tElem, tAttrs) {
                    return {
                        pre: function (scope, iElem, iAttrs) {
                            scope.Event.getMyEvents();
                        },
                        post: function (scope, iElem, iAttrs) {
                            //console.log(name + ': post link');
                        }
                    }
                },
                templateUrl: service.urlBase + 'Events/ListOfMyEvents'
            }
        }])
        .directive('assistantEvents', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Events/AssistantEvents'
            }
        }])
        .directive('createEvents', ['service', function (service) {
            return {
                restrict: 'E',
                compile: function (tElem, tAttrs) {
                    return {
                        pre: function (scope, iElem, iAttrs) {
                            scope.Event.getCountries();
                        }
                    }
                },
                link: function (scope, element, attrs) {

                    /* Bindable functions
                    -----------------------------------------------*/
                    scope.endDateBeforeRender = endDateBeforeRender;
                    scope.endDateOnSetTime = endDateOnSetTime;
                    scope.startDateBeforeRender = startDateBeforeRender;
                    scope.startDateOnSetTime = startDateOnSetTime;

                    function startDateOnSetTime() {
                        scope.$broadcast('start-date-changed');
                    };

                    function endDateOnSetTime() {
                        scope.$broadcast('end-date-changed');
                    };

                    function startDateBeforeRender($dates) {
                        if (scope.dateRangeEnd) {
                            var activeDate = moment(scope.dateRangeEnd);

                            $dates.filter(function (date) {
                                return date.localDateValue() >= activeDate.valueOf()
                            }).forEach(function (date) {
                                date.selectable = false;
                            })
                        }
                    };

                    function endDateBeforeRender($view, $dates) {
                        if (scope.dateRangeStart) {
                            var activeDate = moment(scope.dateRangeStart).subtract(1, $view).add(1, 'minute');

                            $dates.filter(function (date) {
                                return date.localDateValue() <= activeDate.valueOf()
                            }).forEach(function (date) {
                                date.selectable = false;
                            })
                        }
                    };

                },
                templateUrl: service.urlBase + 'Events/CreateEvents'
            }
        }])
        .directive('eventDetail', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Events/EventDetail'
            }
        }])
        .directive('filtersEvents', ['service', function (service) {
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
                                    service.post('Administration/GetListCountries', { CodigoIdiomaUsuarioBase: 1 }, function (res) {
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
        }])
        .directive("scroll", function ($window, $rootScope) {
            return function (scope, element, attrs) {
                angular.element($window).bind("scroll", function () {
                    if ($(window).scrollTop() + $(window).height() > $(document).height() - 500) {
                        $rootScope.$emit("GetMoreEvents", {});
                        //scope.$apply(attrs.scroll);
                    }
                    scope.$apply();
                });
            };
        });
})();