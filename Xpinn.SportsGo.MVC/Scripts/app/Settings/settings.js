(function () {
    'use strict';
    var dependencies =
        [
            "ngSanitize",
            "ngFileUpload"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
    .controller('settingsController',
    ["$translate", "$sce", "$scope", "$rootScope", "Upload", "service", "$filter", function ($translate, $sce, $scope, $rootScope, Upload, service, $filter) {

        var controller = 'Settings/';

        $rootScope.$on('onGetProfile', function (event, obj) {
            $scope.User.userBefore.Usuario = obj.profile.Usuario;
            $scope.User.userToUpdate.Usuario = obj.profile.Usuario;
            $scope.User.userBefore.Email = obj.profile.Email;
            $scope.User.userToUpdate.Email = obj.profile.Email;

        });

        $scope.convertToDate = function (date) {
            return service.formatoFecha(date);
        }
        $scope.currentDate = new Date();

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

        $scope.User = {
            userBefore: { Usuario:'', Email:''},
            userToUpdate: { Usuario: '', Email: '', Clave: '', Password: '' },
            updateInfo: function(){
                service.post(controller + 'ChangeUser', $scope.User.userToUpdate, function (res) {
                    if (res.data.Success) {
                        service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                    }
                    else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                })
            },
            update: function (){
                if (!$scope.User.validateForm()) return;

                if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                    $scope.User.updateInfo();
                }
                else if (($scope.User.userBefore.Usuario.trim() !== $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() === $scope.User.userToUpdate.Email.trim())) {
                    service.post('Authenticate/ValidateIfUserExist', { Usuario: $scope.User.userToUpdate.Usuario }, function (res) {
                        if (res.data.Success) $scope.User.updateInfo();
                        else service.showErrorMessage($translate.instant('LBL_USER_UNAVAILABLE'), service.getTypeMessage('error'));
                    })
                } else if (($scope.User.userBefore.Usuario.trim() === $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() !== $scope.User.userToUpdate.Email.trim())) {
                    service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.User.userToUpdate.Email }, function (res) {
                        if (res.data.Success) $scope.User.updateInfo();
                        else service.showErrorMessage($translate.instant('LBL_EMAIL_UNAVAILABLE'), service.getTypeMessage('error'));
                    })
                } else if (($scope.User.userBefore.Usuario.trim() !== $scope.User.userToUpdate.Usuario.trim()) && ($scope.User.userBefore.Email.trim() !== $scope.User.userToUpdate.Email.trim())) {
                    service.post('Authenticate/ValidateIfUserExist', { Usuario: $scope.User.userToUpdate.Usuario }, function (res) {
                        if (res.data.Success) {
                            service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.User.userToUpdate.Email }, function (res) {
                                if (res.data.Success) $scope.User.updateInfo();
                                else service.showErrorMessage($translate.instant('LBL_EMAIL_UNAVAILABLE'), service.getTypeMessage('error'));
                            })
                        }
                        else service.showErrorMessage($translate.instant('LBL_USER_UNAVAILABLE'), service.getTypeMessage('error'));
                    })
                }                 
            },
            validateForm: function () {
                if($scope.User.userToUpdate.Usuario === '' || $scope.User.userToUpdate.Email === '' 
                    || $scope.User.userToUpdate.Clave === '' || $scope.User.userToUpdate.Password === '') {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                    return false;
                } else if (!$scope.User.validateEmail()) {
                    service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                    return false;
                } else if ($scope.User.userToUpdate.Clave !== $scope.User.userToUpdate.Password) {
                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_RETYPE_PASS'), service.getTypeMessage('error'));
                    return false;
                } else {
                    return true;
                }

            },
            validateEmail: function () {
                if ($scope.User.userToUpdate.Email === '') return true;
                var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                return re.test($scope.User.userToUpdate.Email);
            }
        }

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
            filterHistory: { SkipIndexBase: 0, TakeIndexBase: 10 },
            list: [],
            historyPlansList: [],
            paymentHistory: { Consecutivo : 0 },
            getMyPlan: function () {
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
        $scope.Plans.getHistoryPlans();
        $scope.Plans.get();
    }])
    .directive('payManual', payManual)
    .directive('listOfPlans', listOfPlans)
    .directive('myPlan', myPlan)
    .directive('historyOfMyPlans', historyOfMyPlans);
    payManual.$inject = ['service'];
    listOfPlans.$inject = ['service'];
    myPlan.$inject = ['service'];
    historyOfMyPlans.$inject = ['service'];
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
    function historyOfMyPlans(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Settings/HistoryOfMyPlans'
        }
    };
})();