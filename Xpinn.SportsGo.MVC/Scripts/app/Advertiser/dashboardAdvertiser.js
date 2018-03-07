(function () {
    'use strict';
    var dependencies =
        [
            "ngFileUpload",
            "uiCropper",
            "oi.select"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
        .controller('metricaAdsController',
            ["$sce", "$scope", "service", function ($sce, $scope, service) {

                var controller = 'Advertisements/';
                $scope.loadPage = true;

                $scope.lists = {
                    countries: [],
                    categories: [],
                    languages: [{ Consecutivo: 1, DescripcionIdiomaBuscado: 'Español' }, { Consecutivo: 2, DescripcionIdiomaBuscado: 'Inglés' }, { Consecutivo: 3, DescripcionIdiomaBuscado: 'Portugués' }],
                    filter: { },
                    getCountries: function () {
                        service.post('Administration/GetListCountries', $scope.lists.filter, function (res) {
                            if (res.data.Success) $scope.lists.countries = res.data.list;
                            else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    },
                    getCategories: function () {
                        service.get('Administration/GetListCategories', null, function (res) {
                            if (res.data.Success) $scope.lists.categories = res.data.list;
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
                    ads: 0,
                    adsLastMonth: 0,
                    clicks: 0,
                    clicksLastMonth: 0,
                    seen: 0,
                    seenLastMonth: 0,
                    filter: {
                        CategoriasParaBuscar: [], PaisesParaBuscar: [], IdiomasParaBuscar: [], PlanesParaBuscar: [],
                        ListCategories: [], ListCountries: [], ListLanguages: []
                    },
                    get: function () {
                        service.post(controller + 'GetDashboard', $scope.metricas.filter, function (res) {
                            if (res.data.Success) {
                                $scope.metricas.metrica = res.data.obj;
                                $scope.chart.updateChart();
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
                                if ($scope.loadPage) {
                                    $scope.metricas.get();
                                    $scope.loadPage = false;
                                }
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
                    },
                    getClicks: function () {
                        service.get(controller + 'GetClicks', null, function (res) {
                            if (res.data.Success) {
                                $scope.metricas.clicks = res.data.obj.NumeroRegistrosExistentes;
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    getClicksLastMonth: function () {
                        service.get(controller + 'GetClicksLastMonth', null, function (res) {
                            if (res.data.Success) {
                                $scope.metricas.clicksLastMonth = res.data.obj.NumeroRegistrosExistentes;
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    getSeen: function () {
                        service.get(controller + 'GetSeen', null, function (res) {
                            if (res.data.Success)
                                $scope.metricas.seen = res.data.obj.NumeroRegistrosExistentes;
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        })
                    },
                    getSeenLastMonth: function () {
                        service.get(controller + 'GetSeenLastMonth', null, function (res) {
                            if (res.data.Success)
                                $scope.metricas.seenLastMonth = res.data.obj.NumeroRegistrosExistentes;
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        })
                    }
                };
                $scope.chart = {
                    updateChart: function () {
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

                $scope.lists.fillList();
                $scope.metricas.getAds();
                $scope.metricas.getAdsLastMonth();
                $scope.metricas.getClicks();
                $scope.metricas.getClicksLastMonth();
                $scope.metricas.getSeen();
                $scope.metricas.getSeenLastMonth();

            }]
        )
        .directive('newsList', newsList);
        newsList.$inject = ['service'];
        function newsList(service) {

        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Profile/Posts'
        }
    };
})();
