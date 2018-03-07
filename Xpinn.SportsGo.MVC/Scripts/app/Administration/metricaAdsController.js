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

    app.controller('metricaAdsController', ["$sce", "$scope", "service",
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
                registeredAdvertisers: 0,
                registeredAdvertisersLastMonth: 0,
                ads: 0,
                adsLastMonth: 0,
                filtro: { CategoriasParaBuscar: [], PaisesParaBuscar: [], IdiomasParaBuscar: [], PlanesParaBuscar: [] },
                get: function () {
                    service.post(controller + 'GetInfoMetricaAds', $scope.metricas.filtro, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.metrica = res.data.obj;
                            $scope.chart.updateChart();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredAdvertisers: function () {
                    service.get(controller + 'GetRegisteredAdvertisers', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredAdvertisers = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredAdvertisersLastMonth: function () {
                    service.get(controller + 'GetRegisteredAdvertisersLastMonth', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredAdvertisersLastMonth = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getAds: function () {
                    service.get(controller + 'GetAds', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.ads = res.data.obj.NumeroRegistrosExistentes;
                            $scope.lists.fillList();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getAdsLastMonth: function () {
                    service.get(controller + 'GetAdsLastMonth', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.adsLastMonth = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            };
            $scope.chart = {
                updateChart: function () {
                    if ($scope.metricas.ads === 0) {
                        $scope.metricas.getAds();
                    }
                    var myCircle = Circles.create({
                        id: 'circles-1',
                        radius: 150,
                        value: $scope.metricas.metrica.NumeroAnuncios,
                        maxValue: $scope.metricas.ads,
                        width: 45,
                        text: function (value) { return Math.round(((value * 100) / $scope.metricas.ads)) + '%'; },
                        colors: ['#ddd', '#00b48a'],
                        duration: 1000,
                        wrpClass: 'circles-wrp',
                        textClass: 'circles-text'
                    });
                }
            }

            $scope.metricas.getAds();
            $scope.metricas.getRegisteredAdvertisers();
            $scope.metricas.getRegisteredAdvertisersLastMonth();
            $scope.metricas.getAdsLastMonth();

        }]
    )
})();