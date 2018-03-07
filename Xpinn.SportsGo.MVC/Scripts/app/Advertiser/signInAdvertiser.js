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
    .controller('SignInAvertiserController',
        ["$translate", "$scope", '$filter', "service", "Upload", "$timeout", function ($translate, $scope, $filter, service, Upload, $timeout) {

            var controller = 'Advertisements/';

            $scope.ConverToDate = function (date) {
                return service.formatoFecha(date);
            }

            $scope.Countries = {
                list: [],
                get: function () {
                    service.post('Administration/GetListCountries', null, function (res) {
                        if (res.data.Success) {
                            $scope.Countries.list = res.data.list;
                            $scope.Advertiser.get();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                }
            };

            $scope.Advertiser = {
                isBusy: false,
                info: { Consecutivo: 0, Personas: { UrlImagenPerfil: ''} },
                imageProfileTemp: { nameImage: '', dataImage: null },
                archivosPerfilDefault: service.urlImageProfileDefault,
                get: function(){
                    service.post(controller + 'GetProfile', null, function (res) {
                        if (res.data.Success) {
                            $scope.headerPanel = $translate.instant('LBL_MY_PROFILE_INFO');
                            $scope.Advertiser.info = res.data.obj;
                            $scope.Advertiser.info.Personas.Paises = $filter('filter')($scope.Countries.list, { Consecutivo: $scope.Advertiser.info.Personas.CodigoPais })[0];
                        }
                        else {
                            $scope.headerPanel = $translate.instant('LBL_MY_PROFILE_INFO');
                        }
                    })
                },
                save: function () {
                    if (!$scope.formPersonalInfo.$valid) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return;
                    }

                    var saveImage = $scope.Advertiser.info.Consecutivo === 0 ? true : false;
                    $scope.Advertiser.info.Personas.CodigoPais = $scope.Advertiser.info.Personas.Paises.Consecutivo;
                    service.post(controller + 'SaveAdvertiser', $scope.Advertiser.info, function (res) {
                        if (res.data.Success) {

                            if ($scope.Advertiser.info.Consecutivo !== null && $scope.Advertiser.info.Consecutivo > 0) {
                                service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                            } else {
                                $('#alertConfirmationRegistration').modal('toggle');
                            }

                            $scope.Advertiser.info = res.data.obj;
                            if (saveImage) {
                                if ($scope.Advertiser.imageProfileTemp.dataImage !== null)
                                    $scope.Advertiser.updateImage($scope.Advertiser.imageProfileTemp.dataImage, $scope.Advertiser.imageProfileTemp.nameImage);
                            }
                            
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                updateImage: function (dataImage, nameImage) {
                    if ($scope.Advertiser.isBusy) return;
                    $scope.Advertiser.isBusy = true;

                    if ($scope.Advertiser.info.Consecutivo === 0) {
                        $scope.Advertiser.imageProfileTemp.dataImage = dataImage;
                        $scope.Advertiser.imageProfileTemp.nameImage = nameImage;
                        $scope.Advertiser.info.Personas.UrlImagenPerfil = dataImage;
                        $scope.Advertiser.isBusy = false;
                        $('#modalImageProfile').modal('hide');
                        return;
                    }

                    Upload.upload({
                        url: service.urlBase + 'Profile/uploadImageProfile',
                        data: {
                            file: Upload.dataUrltoBlob(dataImage, nameImage),
                            Person: JSON.stringify({ Consecutivo: $scope.Advertiser.info.CodigoPersona })
                        },
                    }).then(function (res) {
                        if (res.data.Success) {

                            $scope.Advertiser.info.Personas.UrlImagenPerfil = dataImage;
                            $scope.Advertiser.isBusy = false;
                            $('#modalImageProfile').modal('hide');
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
            };
            
            $scope.Countries.get();
        }]
    )
})();
