(function () {
    'use strict';
    app
    .controller('historyPlansController', ["$translate", "$sce", "$scope", "service",
        function ($translate, $sce, $scope, service) {

            var controller = 'Administration/';

            $scope.convertToDate = function (date) {
                return service.formatoFecha(date);
            }

            $scope.Plans = {
                historyPlansList: [],
                filterHistory: { SkipIndexBase: 0, TakeIndexBase: 10 },
                getHistoryPlans: function () {
                    $scope.Plans.filterHistory.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post('Settings/GetHistoryOfMyPlans', $scope.Plans.filterHistory, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.historyPlansList = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getPeriodicityById: function (id) {
                    switch (id) {
                        case 1:
                            return $translate.instant('LBL_MONTHLY');
                        case 2:
                            return $translate.instant('LBL_BIMONTHLY');
                        case 3:
                            return $translate.instant('LBL_BIANNUAL');
                        case 4:
                            return $translate.instant('LBL_ANNUAL');
                        default:
                    }
                },
                getStatusById: function (id) {
                    switch (id) {
                        case 1:
                            return 'Espera pago';
                        case 2:
                            return 'Pendiente por aprobar';
                        case 3:
                            return 'Rechazado';
                        case 4:
                            return 'Aprobado';
                        default:
                            return '';

                    }
                }
            }

            $scope.Plans.getHistoryPlans();

        }]
    )
    .directive('historyOfMyPlans', historyOfMyPlans);
    historyOfMyPlans.$inject = ['service'];
    function historyOfMyPlans(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Settings/HistoryOfMyPlans'
        }
    };
})();