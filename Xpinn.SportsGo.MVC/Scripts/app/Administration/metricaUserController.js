(function () {
    'use strict';
    var dependencies =
        [
            "oi.select",
            "ngFileUpload",
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app.controller('metricaUserController', ["$translate", "$sce", "$scope", "service",
        function ($translate, $sce, $scope, service) {

            var controller = 'Administration/';

            $scope.lists = {
                countries: [],
                categories: [],
                languages: [{ Consecutivo: 1, DescripcionIdiomaBuscado: 'Español' }, { Consecutivo: 2, DescripcionIdiomaBuscado: 'Inglés' }, { Consecutivo: 3, DescripcionIdiomaBuscado: 'Portugués' }],
                plans: [],
                filter: { TakeIndexBase: 100 },
                getCountries: function () {
                    service.post(controller + 'GetListCountries', { CodigoIdiomaUsuarioBase: 1 }, function (res) {
                        if (res.data.Success) $scope.lists.countries = res.data.list;
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getCategories: function () {
                    service.get(controller + 'GetListCategories', null, function (res) {
                        if (res.data.Success) $scope.lists.categories = res.data.list;
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getPlans: function () {
                    service.post(controller + 'GetListPlans', $scope.lists.filter, function (res) {
                        if (res.data.Success) $scope.lists.plans = res.data.list;
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                fillList: function () {
                    $scope.lists.getCountries();
                    $scope.lists.getCategories();
                    $scope.lists.getPlans();
                }
            }

            $scope.metricas = {
                metrica: {},
                registeredUsers: 0,
                registeredUsersLastMonth: 0,
                plansLastMonth: 0,
                movileDownloads: 0,
                filtro: { CategoriasParaBuscar: [], PaisesParaBuscar: [], IdiomasParaBuscar: [], PlanesParaBuscar: [] },
                get: function () {
                    service.post(controller + 'GetInfoMetrica', $scope.metricas.filtro, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.metrica = res.data.obj;
                            $scope.chart.updateChart();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredUser: function () {
                    service.get(controller + 'GetRegisteredUsers', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredUsers = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getRegisteredUserLastMonth: function () {
                    service.get(controller + 'GetRegisteredUsersLastMonth', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.registeredUsersLastMonth = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                GetPlansLastMonth: function () {
                    service.get(controller + 'GetPlansLastMonth', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.plansLastMonth = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                GetMovileDownloads: function () {
                    service.get(controller + 'GetMovileDownloads', null, function (res) {
                        if (res.data.Success) {
                            $scope.metricas.movileDownloads = res.data.obj.NumeroRegistrosExistentes;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            };
            $scope.chart = {
                updateChart: function () {
                    // reset chart
                    $('#myChart').remove();
                    //buid new chart
                    $('#chart').append('<canvas id="myChart"><canvas>');
                    var ctx = $("#myChart");
                    var data = {
                        labels: [
                            $translate.instant('LBL_ATHLETES'),
                            $translate.instant('LBL_REPRESENTATIVE'),
                            $translate.instant('LBL_GROUPS'),
                            "Anunciantes"
                        ],
                        datasets: [{
                            data: [$scope.metricas.metrica.NumeroCandidatos, $scope.metricas.metrica.NumeroRepresentantes, $scope.metricas.metrica.NumeroGrupos],
                            backgroundColor: [
                                "#FF6384",
                                "#36A2EB",
                                "#FFCE56"
                            ],
                            hoverBackgroundColor: [
                                "#FF6384",
                                "#36A2EB",
                                "#FFCE56"
                            ],
                            borderWidth: 1
                        }]
                    }
                    var options = {
                        responsive: true,
                        animation: { animateRotate: true, animateScale: true },
                        cutoutPercentage: 55,
                        legend: false,
                        legendCallback: function (chart) {
                            var text = [];
                            text.push('<ul class="' + chart.id + '-legend">');
                            for (var i = 0; i < chart.data.datasets[0].data.length; i++) {
                                text.push('<li><span style="background-color:' + chart.data.datasets[0].backgroundColor[i] + '">');
                                if (chart.data.labels[i]) {
                                    text.push(chart.data.labels[i]);
                                }
                                text.push('</span></li>');
                            }
                            text.push('</ul>');
                            return text.join("");
                        },
                        tooltips: {
                            custom: function (tooltip) {
                                //tooltip.x = 0;
                                //tooltip.y = 0;
                            },
                            mode: 'single',
                            callbacks: {
                                label: function (tooltipItems, data) {
                                    var sum = data.datasets[0].data.reduce(add, 0);
                                    function add(a, b) {
                                        return a + b;
                                    }

                                    return parseInt((data.datasets[0].data[tooltipItems.index] / sum * 100), 10) + ' %';
                                },
                                beforeLabel: function (tooltipItems, data) {
                                    return data.datasets[0].data[tooltipItems.index] + '';
                                }
                            }
                        }
                    };
                    var myChart = new Chart(ctx, {
                        type: 'doughnut',
                        data: data,
                        options: options
                    });
                    $("#chartjs-legend").html(myChart.generateLegend());
                    $("#chartjs-legend").on('click', "li", function (e) {
                        var index = $(this).index();
                        if (myChart.data.datasets[0].data[index] === 0) {
                            if (index === 0) myChart.data.datasets[0].data[index] = $scope.metricas.metrica.NumeroCandidatos
                            else if (index === 1) myChart.data.datasets[0].data[index] = $scope.metricas.metrica.NumeroRepresentantes
                            else myChart.data.datasets[0].data[index] = $scope.metricas.metrica.NumeroGrupos;
                        }
                        else {
                            myChart.data.datasets[0].data[$(this).index()] = 0;
                        }
                        myChart.update();
                    })
                }
            }
            $scope.lists.fillList();
            $scope.metricas.get();
            $scope.metricas.getRegisteredUser();
            $scope.metricas.getRegisteredUserLastMonth();
            $scope.metricas.GetPlansLastMonth();
            $scope.metricas.GetMovileDownloads();
        }]
    )
})();