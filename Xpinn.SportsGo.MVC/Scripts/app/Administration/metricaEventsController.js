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

    app.controller('metricaEventsController', ["$sce", "$scope", "service",
        function ($sce, $scope, service) {

            var controller = 'Administration/';

            $scope.lists = {
                countries: [],
                categories: [],
                lenguages: [{ Consecutivo: 1, DescripcionIdiomaBuscado: 'Español' }, { Consecutivo: 2, DescripcionIdiomaBuscado: 'Inglés' }, { Consecutivo: 3, DescripcionIdiomaBuscado: 'Portugués' }],
                plans: [],
                filter: { IdiomaBase: service.getLenguageFromNavigator() },
                getCountries: function () {
                    service.post(controller + 'GetListCountries', $scope.lists.filter, function (res) {
                        if (res.data.Success) $scope.lists.countries = res.data.list;
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getCategories: function () {
                    service.get(controller + 'GetListCategories', null, function (res) {
                        if (res.data.Success) {
                            $scope.lists.categories = res.data.list;
                            $scope.metricas.get();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                fillList: function () {
                    $scope.lists.getCountries();
                    $scope.lists.getCategories();
                }
            }
            $scope.metricas = {
                metrica: {},
                registeredEvents: 0,
                registeredEventsLastMonth: 0,
                filtro: { CategoriasParaBuscar: [], PaisesParaBuscar: [], IdiomasParaBuscar: [], PlanesParaBuscar: [] },
                get: function () {
                    service.post(controller + 'GetInfoMetricaEvents', $scope.metricas.filtro, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.metrica = res.data.obj;
                            $scope.chart.updateChart();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredEvents: function () {
                    service.get(controller + 'GetRegisteredEvents', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredEvents = res.data.obj.NumeroRegistrosExistentes;
                            $scope.lists.fillList();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredEventsLastMonth: function () {
                    service.get(controller + 'GetRegisteredEventsLastMonth', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredEventsLastMonth = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            };
            $scope.chart = {
                updateChart: function () {
                    var myCircle = Circles.create({
                        id: 'circles-1',
                        radius: 150,
                        value: $scope.metricas.metrica.NumeroEventos,
                        maxValue: $scope.metricas.registeredEvents,
                        width: 45,
                        text: function (value) { return Math.round(((value * 100) / $scope.metricas.registeredEvents)) + '%'; },
                        colors: ['#ddd', '#00b48a'],
                        duration: 1000,
                        wrpClass: 'circles-wrp',
                        textClass: 'circles-text'
                    });
                }
            }

            $scope.metricas.getRegisteredEvents();
            $scope.metricas.getRegisteredEventsLastMonth();
        }]
    )
})();