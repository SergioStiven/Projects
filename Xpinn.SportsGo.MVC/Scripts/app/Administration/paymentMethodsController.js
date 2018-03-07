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

    app
    .controller('PaymentMethodsController', ["$translate", "$sce", "$scope", "$filter", "Upload", "$timeout", "service",
        function ($translate, $sce, $scope, $filter, Upload, $timeout, service) {

            var controller = 'Administration/';

            $scope.imageDefault = '../Content/assets/img/no-image.png';

            $scope.getLanguageById = function (langKey) {
                switch (langKey) {
                    case 1:
                        return $translate.instant('LBL_SPANISH');
                    case 2:
                        return $translate.instant('LBL_ENGLISH');
                    case 3:
                        return $translate.instant('LBL_PORTUGUESE');
                    default:
                        return $translate.instant('LBL_ENGLISH');
                }
            }

            $scope.Lists = {
                countries: [],
                getCountries: function () {
                    service.post(controller + 'GetListCountries', {}, function (res) {
                        if (res.data.Success) {
                            $scope.Lists.countries = res.data.list;
                            $scope.InvoiceFormat.get();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                }
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

            $scope.InvoiceFormat = {
                list: [],
                addNew: false,
                imageToSaveTemp: { dataImage: null, nameImage: '' },
                countryToSave: 0,
                tempToSave: [{ CodigoIdioma: 1 }, { CodigoIdioma: 2 }, { CodigoIdioma: 3 }],
                get: function () {
                    service.get(controller + 'GetInvoiceFormat', null, function (res) {
                        if (res.data.Success) {
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].Paises = $filter('filter')($scope.Lists.countries, { Consecutivo: res.data.list[i].CodigoPais })[0];
                            }
                            $scope.InvoiceFormat.list = res.data.list;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                save: function () {
                    if (!$scope.InvoiceFormat.validateForm()) return;
                    for (var i = 0; i < $scope.InvoiceFormat.tempToSave.length; i++) {
                        $scope.InvoiceFormat.tempToSave[i].CodigoPais = $scope.InvoiceFormat.countryToSave;
                    }
                    service.post(controller + 'SaveInvoiceFormat', $scope.InvoiceFormat.tempToSave, function (res) {
                        if (res.data.Success) {
                            $scope.InvoiceFormat.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                update: function (invoiceFormat) {
                    service.post(controller + 'UpdateInvoiceFormat', invoiceFormat, function (res) {
                        if (res.data.Success) {
                            $scope.InvoiceFormat.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                delete: function (invoiceFormat) {
                    service.post(controller + 'DeleteInvoiceFormat', invoiceFormat, function (res) {
                        if (res.data.Success) {
                            $scope.PaymentPlans.getPlans();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                validateForm: function () {
                    if ($scope.InvoiceFormat.tempToSave[0].Texto === '' || $scope.InvoiceFormat.tempToSave[1].Texto === ''
                        || $scope.InvoiceFormat.tempToSave[2].Texto === '' || $scope.InvoiceFormat.countryToSave === 0) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    }
                    return true;

                }
            }

            $scope.Lists.getCountries();
        }]
    )
})();