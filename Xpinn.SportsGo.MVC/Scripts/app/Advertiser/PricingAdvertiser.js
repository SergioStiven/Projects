(function () {
    'use strict';
    var dependencies =
        [
            "ngFileUpload"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
    .controller('PricingController',
        ["$translate", "$scope", '$filter', "Upload", "service", "$timeout", function ($translate, $scope, $filter, Upload, service, $timeout) {

            var controller = 'Advertisements/';

            $scope.currentDate = new Date();

            $scope.ConverToDate = function (date) {
                return service.formatoFecha(date);
            }

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

            $scope.Plans = {
                isBusy: false,
                isPlanInProcess: false,
                planInProcess: {},
                isMyPlan: true,
                manualPlan: false,
                btnBefore: false,
                btnNext: true,
                myPlan: { Planes: { PlanDefault: 0 } },
                planToPrint: {},
                invoiceFormat: {},
                filter: { SkipIndexBase: 0, TakeIndexBase: 4, CodigoTipoPerfil: 0, IdiomaBase: 0, CodigoPaisParaBuscarMoneda: 0 },
                list: [],
                paymentHistory: { Consecutivo: 0 },
                getMyPlan: function(){
                    service.get('Settings/GetMyPlan', null, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.myPlan = res.data.obj;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                get: function () {
                    service.post('Settings/GetListPlansByProfile', $scope.Plans.filter, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.list = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getNext: function () {
                    if (!$scope.Plans.btnNext) return;
                    $scope.Plans.filter.SkipIndexBase = $scope.Plans.filter.TakeIndexBase;
                    $scope.Plans.filter.TakeIndexBase += 4;
                    service.post('Settings/GetListPlansByProfile', $scope.Plans.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length > 0)
                                $scope.Plans.list = res.data.list;
                            else {
                                $scope.Plans.filter.SkipIndexBase -= 4;
                                $scope.Plans.btnNext = false;
                            }
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getBefore: function () {
                    if ($scope.Plans.filter.SkipIndexBase === 0) {
                        $scope.Plans.btnBefore = false;
                        return;
                    }
                    $scope.Plans.filter.SkipIndexBase -= 4;
                    $scope.Plans.filter.TakeIndexBase = 4;
                    service.post('Settings/GetListPlansByProfile', $scope.Plans.filter, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.list = res.data.list;
                            $scope.Plans.btnNext = true;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                searchInvoiceFormat: function (savePlan) {
                    service.get('Settings/SearchInvoiceFormat', null, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.invoiceFormat = res.data.obj;
                            if ($scope.Plans.isMyPlan) {
                                $scope.Plans.isMyPlan = false;
                                return;
                            }
                            $scope.Plans.paymentHistory.TextoFacturaFormato = $scope.Plans.invoiceFormat.Texto;
                            $scope.Plans.paymentHistory.CodigoPlan = $scope.Plans.planToPrint.Consecutivo;
                            $scope.Plans.paymentHistory.Precio = $scope.Plans.planToPrint.Precio;
                            if (savePlan) $scope.Plans.savePaymentHistory($scope.Plans.paymentHistory);
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                save: function () {
                    $scope.Advertiser.info.Personas.CodigoIdioma = service.getLenguageFromNavigator();
                    service.post(controller + 'CreateAdvertiser', $scope.Advertiser.info, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                uploadFile: function () {
                    if ($scope.Plans.planInProcess.CodigoArchivo === null) $scope.Plans.planInProcess.CodigoArchivo = 0;
                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadFile',
                        data: { file: $scope.file, ConsecutivoArchivo: $scope.Plans.planInProcess.CodigoArchivo }
                    }).then(function (resp) {
                        $scope.Plans.planInProcess.CodigoArchivo = resp.data.obj.ConsecutivoArchivoCreado;
                        $scope.Plans.updateStatus();
                    }, function (resp) {
                        service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                        //console.log('progress: ' + progressPercentage + '% ' + evt.config.data.file.name);
                    });
                },
                updateStatus: function () {
                    service.post('Settings/UpdatePaymentStatus', $scope.Plans.planInProcess, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.validatePlanInProcess();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                savePaymentHistory: function (paymentHistory) {
                    service.post('Settings/SavePaymentHistory', paymentHistory, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage($translate.instant('NOTI_THANKS_PAYMENT'), service.getTypeMessage('success'));
                            $scope.Plans.paymentHistory.Consecutivo = res.data.obj.ConsecutivoCreado;
                            $scope.Plans.validatePlanInProcess();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                showManualPlan: function (plan, savePlan) {
                    $scope.Plans.planToPrint = plan;
                    $scope.Plans.searchInvoiceFormat(savePlan);
                    $scope.Plans.manualPlan = true;
                },
                printPlan: function (divName) {
                    $('#btnPrint').hide();
                    var printContents = document.getElementById('divManualPlan').innerHTML;
                    var popupWin = window.open('', '_blank', 'width=700,height=500');
                    popupWin.document.open();
                    popupWin.document.write('<html><head><link rel="stylesheet" type="text/css" href="' + service.urlBase + '/Content/assets/css/style.light-blue-500.min.css" /></head><body onload="window.print()">' + printContents + '</body></html>');
                    popupWin.document.close();
                    $('#btnPrint').show();
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
                validatePlanInProcess: function () {
                    service.get('Settings/ValidateIfIHaveAPlanInProcess', null, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.isPlanInProcess = true;
                            service.post('Settings/GetMyPlanInProcess', { ZonaHorariaGMTBase: service.getTimeZone() }, function (res) {
                                if (res.data.Success) {
                                    $scope.Plans.planInProcess = res.data.obj;
                                    $scope.Plans.planInProcess.Planes.Precio = res.data.obj.Precio;
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                }
                            })
                        }
                        else {
                            $scope.Plans.isPlanInProcess = false;
                        }
                    })
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
                },
                deletePlanInProcess: function (obj) {
                    service.post('Settings/DeletePlanInProcess', $scope.Plans.planInProcess, function (res) {
                        if (res.data.Success) {
                            $scope.Plans.validatePlanInProcess();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                showMyPlan: function () {
                    $scope.Plans.getMyPlan();
                    $scope.Plans.isMyPlan = true;
                    $scope.Plans.manualPlan = false;
                    $scope.Plans.paymentHistory.Consecutivo = 0;
                },
                redirectToPayU: function () {
                    $('#modalConfirmPayment').modal('toggle');
                    window.open(service.urlBase + 'payment/index/' + $scope.Plans.paymentHistory.Consecutivo, '_blank');
                }
            }; 

            $scope.Plans.validatePlanInProcess();
            $scope.Plans.getMyPlan();
            $scope.Plans.get();

        }]
    )
    .directive('payManual', payManual)
    .directive('listOfPlans', listOfPlans)
    .directive('myPlan', myPlan);
    payManual.$inject = ['service'];
    listOfPlans.$inject = ['service'];
    myPlan.$inject = ['service'];
    function payManual(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Settings/PayManual'
        }
    };
    function listOfPlans(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Settings/ListOfPlans'
        }
    };
    function myPlan(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Settings/MyPlan'
        }
    };
})();
