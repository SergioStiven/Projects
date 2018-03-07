(function () {
    'use strict';
    var dependencies =
        [
            "oi.select",
            "ngFileUpload",
            "uiCropper"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
        .controller('PaymentPlansController', ["$translate","$sce", "$scope", "Upload", "$timeout", "service",
        function ($translate, $sce, $scope, Upload, $timeout, service) {

            var controller = 'Administration/';

            $scope.imageDefault = '../Content/assets/img/no-image.png';

            $scope.convertToDate = function (date) {
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
            }

            $scope.PaymentPlans = {
                list: [],
                addNew: false,
                isBusy: false,
                imageToSaveTemp: { dataImage: null, nameImage: '' },
                planToSave: {
                    Consecutivo: 0, PlanDefault: false, CodigoTipoPerfil: 1,
                    PlanesContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }, { Descripcion: '', CodigoIdioma: 3 }], Precio: 0, CodigoPeriodicidad: 1
                },
                planTemp: {
                    Consecutivo: 0, PlanDefault: false, CodigoTipoPerfil: 1,
                    PlanesContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }, { Descripcion: '', CodigoIdioma: 3 }], Precio: 0, CodigoPeriodicidad: 1
                },
                options: { VideosPerfil: false, ServiciosChat: false, ConsultaCandidatos: false, DetalleCandidatos: false, ConsultaGrupos: false, DetalleGrupos: false, ConsultaEventos: false, CreacionAnuncios: false, EstadisticasAnuncios: false, Modificacion: false, TiempoPermitidoVideo: 1 },
                optionsTemp: { VideosPerfil: false, ServiciosChat: false, ConsultaCandidatos: false, DetalleCandidatos: false, ConsultaGrupos: false, DetalleGrupos: false, ConsultaEventos: false, CreacionAnuncios: false, EstadisticasAnuncios: false, Modificacion: false, TiempoPermitidoVideo: 1 },
                filter: { TakeIndexBase: 100, SkipIndexBase: 0 },
                getPlans: function () {
                    service.post(controller + 'GetListPlansForAdmin', this.filter, function (res) {
                        if (res.data.Success) {
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].PlanDefaultChk = res.data.list[i].PlanDefault === 1 ? true : false;
                            }
                            $scope.PaymentPlans.list = res.data.list;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                viewFormToAddNewPlan: function () {
                    $scope.PaymentPlans.planTemp = {
                        PlanesContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }],
                        Precio: 0, CodigoPeriodicidad: 1
                    };
                    $scope.PaymentPlans.addNew = !$scope.PaymentPlans.addNew;
                },
                save: function (plan) {
                    if (!$scope.PaymentPlans.validateForm($scope.PaymentPlans.planToSave, true)) return;
                    if ($scope.PaymentPlans.isBusy) return;
                    $scope.PaymentPlans.isBusy = true;
                    plan.VideosPerfil = $scope.PaymentPlans.options.VideosPerfil ? 1 : 0;
                    plan.ServiciosChat = $scope.PaymentPlans.options.ServiciosChat ? 1 : 0;
                    plan.ConsultaCandidatos = $scope.PaymentPlans.options.ConsultaCandidatos ? 1 : 0;
                    plan.DetalleCandidatos = $scope.PaymentPlans.options.DetalleCandidatos ? 1 : 0;
                    plan.ConsultaGrupos = $scope.PaymentPlans.options.ConsultaGrupos ? 1 : 0;
                    plan.DetalleGrupos = $scope.PaymentPlans.options.DetalleGrupos ? 1 : 0;
                    plan.ConsultaEventos = $scope.PaymentPlans.options.ConsultaEventos ? 1 : 0;
                    plan.CreacionAnuncios = $scope.PaymentPlans.options.CreacionAnuncios ? 1 : 0;
                    plan.EstadisticasAnuncios = $scope.PaymentPlans.options.EstadisticasAnuncios ? 1 : 0;
                    plan.PlanDefault = plan.PlanDefault ? 1 : 0;
                    plan.NumeroAparicionesAnuncio = $scope.PaymentPlans.options.NumeroAparicionesAnuncio;
                    plan.NumeroDiasVigenciaAnuncio = $scope.PaymentPlans.options.NumeroDiasVigenciaAnuncio;
                    plan.TiempoPermitidoVideo = $scope.PaymentPlans.options.TiempoPermitidoVideo;

                    Upload.upload({
                        url: service.urlBase + controller + 'SavePlan',
                        data: {
                            file: Upload.dataUrltoBlob($scope.PaymentPlans.imageToSaveTemp.dataImage, $scope.PaymentPlans.imageToSaveTemp.nameImage),
                            planToSave: JSON.stringify(plan)
                        }
                    }).then(function (res) {
                        $scope.PaymentPlans.isBusy = false;
                        if (res.data.Success) {
                            $scope.PaymentPlans.addNew = false;
                            $scope.PaymentPlans.getPlans();
                            $scope.PaymentPlans.cleanData();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                update: function (plan) {
                    plan.PlanDefault = plan.PlanDefaultChk ? 1 : 0;
                    if (!$scope.PaymentPlans.validateForm(plan, false)) return;
                    service.post(controller + 'UpdatePlan', plan, function (res) {
                        if (res.data.Success) {
                            $scope.PaymentPlans.getPlans();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $('#modalOptions').modal('hide');
                    })
                },
                delete: function (plan) {
                    service.post(controller + 'DeletePlan', plan, function (res) {
                        if (res.data.Success) {
                            $scope.PaymentPlans.getPlans();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                viewOptions: function (plan) {
                    if (plan === null) {
                        $scope.PaymentPlans.options =
                            {
                                VideosPerfil: false, ServiciosChat: false, ConsultaCandidatos: false, DetalleCandidatos: false,
                                ConsultaGrupos: false, DetalleGrupos: false, ConsultaEventos: false, CreacionAnuncios: false,
                                EstadisticasAnuncios: false, Modificacion: false, TiempoPermitidoVideo: 1
                            };
                        $('#modalOptions').modal('toggle');
                        return;
                    }
                    $scope.PaymentPlans.planTemp = {};
                    $scope.PaymentPlans.options.VideosPerfil = plan.VideosPerfil === 1 ? true : false;
                    $scope.PaymentPlans.options.ServiciosChat = plan.ServiciosChat === 1 ? true : false;
                    $scope.PaymentPlans.options.ConsultaCandidatos = plan.ConsultaCandidatos === 1 ? true : false;
                    $scope.PaymentPlans.options.DetalleCandidatos = plan.DetalleCandidatos === 1 ? true : false;
                    $scope.PaymentPlans.options.ConsultaGrupos = plan.ConsultaGrupos === 1 ? true : false;
                    $scope.PaymentPlans.options.DetalleGrupos = plan.DetalleGrupos === 1 ? true : false;
                    $scope.PaymentPlans.options.ConsultaEventos = plan.ConsultaEventos === 1 ? true : false;
                    $scope.PaymentPlans.options.CreacionAnuncios = plan.CreacionAnuncios === 1 ? true : false;
                    $scope.PaymentPlans.options.EstadisticasAnuncios = plan.EstadisticasAnuncios === 1 ? true : false;
                    $scope.PaymentPlans.options.NumeroAparicionesAnuncio = plan.NumeroAparicionesAnuncio;
                    $scope.PaymentPlans.options.NumeroDiasVigenciaAnuncio = plan.NumeroDiasVigenciaAnuncio;
                    $scope.PaymentPlans.options.TiempoPermitidoVideo = plan.TiempoPermitidoVideo;
                    $scope.PaymentPlans.planTemp = plan;
                    $('#modalOptions').modal('toggle');
                },
                updateOptions: function () {
                    if ($scope.PaymentPlans.addNew) {
                        $('#modalOptions').modal('hide');
                        return;
                    }
                    $scope.PaymentPlans.planTemp.VideosPerfil = $scope.PaymentPlans.options.VideosPerfil;
                    $scope.PaymentPlans.planTemp.ServiciosChat = $scope.PaymentPlans.options.ServiciosChat;
                    $scope.PaymentPlans.planTemp.ConsultaCandidatos = $scope.PaymentPlans.options.ConsultaCandidatos;
                    $scope.PaymentPlans.planTemp.DetalleCandidatos = $scope.PaymentPlans.options.DetalleCandidatos;
                    $scope.PaymentPlans.planTemp.ConsultaGrupos = $scope.PaymentPlans.options.ConsultaGrupos;
                    $scope.PaymentPlans.planTemp.DetalleGrupos = $scope.PaymentPlans.options.DetalleGrupos;
                    $scope.PaymentPlans.planTemp.ConsultaEventos = $scope.PaymentPlans.options.ConsultaEventos;
                    $scope.PaymentPlans.planTemp.CreacionAnuncios = $scope.PaymentPlans.options.CreacionAnuncios;
                    $scope.PaymentPlans.planTemp.EstadisticasAnuncios = $scope.PaymentPlans.options.EstadisticasAnuncios;
                    $scope.PaymentPlans.planTemp.NumeroAparicionesAnuncio = $scope.PaymentPlans.options.NumeroAparicionesAnuncio;
                    $scope.PaymentPlans.planTemp.NumeroDiasVigenciaAnuncio = $scope.PaymentPlans.options.NumeroDiasVigenciaAnuncio;
                    $scope.PaymentPlans.planTemp.TiempoPermitidoVideo = $scope.PaymentPlans.options.TiempoPermitidoVideo;
                    $scope.PaymentPlans.update($scope.PaymentPlans.planTemp);
                },
                updateVideoDuration: function (keyCode) {
                    if (keyCode === 8) return;
                    if ($scope.PaymentPlans.options.TiempoPermitidoVideo < 1)
                        $scope.PaymentPlans.options.TiempoPermitidoVideo = 1;
                    else if ($scope.PaymentPlans.options.TiempoPermitidoVideo > 600) {
                        $scope.PaymentPlans.options.TiempoPermitidoVideo = 600;
                    }
                },
                updateImage: function (dataImage, nameImage) {
                    Upload.upload({
                        url: service.urlBase + controller + 'UpdateImagePlan',
                        data: {
                            file: Upload.dataUrltoBlob(dataImage, nameImage),
                            plan: JSON.stringify({ CodigoArchivo: $scope.PaymentPlans.planTemp.CodigoArchivo })
                        }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $('#modalImagePlan').modal('hide');
                            console.log($scope.PaymentPlans.list);
                            console.log($scope.Indice);
                            $scope.PaymentPlans.list[$scope.Indice].UrlArchivo = dataImage;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                saveImageTemp: function (dataImage, nameImage) {
                    $scope.PaymentPlans.imageToSaveTemp.dataImage = dataImage;
                    $scope.PaymentPlans.imageToSaveTemp.nameImage = nameImage;
                    $scope.imageDefault = dataImage;
                    $('#modalNewImagePlan').modal('hide');
                },
                getProfileById: function (profileId) {
                    switch (profileId) {
                        case 1:
                            return 'Candidato';
                        case 2:
                            return 'Grupo';
                        case 3:
                            return 'Representante';
                        case 4:
                            return 'Anunciante';
                        case 5:
                            return 'Administrador';
                        default:
                            return '';
                    }
                },
                showModalImagePlan: function (plan,index) {
                    $scope.croppedDataUrlPlan = '';
                    $scope.picFileCategory = '';
                    $scope.picFilePlan = undefined;
                    $('#imageNewPlan').attr('src', '');
                    $('#imagePlan').attr('src', '');
                    if (plan !== null) {
                        $scope.PaymentPlans.planTemp.CodigoArchivo = plan.CodigoArchivo;
                        $scope.Indice = index;
                    }
                },
                validateForm: function (plan, isNew) {
                    if (plan.PlanesContenidos.length !== 3) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    }else if (plan.PlanesContenidos[0].Descripcion === '' || plan.PlanesContenidos[1].Descripcion === ''
                        || plan.PlanesContenidos[2].Descripcion === '') {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else if (plan.Precio === '' || plan.Precio === null || plan.Precio === undefined) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else if ($scope.PaymentPlans.imageToSaveTemp.dataImage === null && isNew) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_IMAGE'), service.getTypeMessage('error'));
                        return false;
                    }
                    if (plan.NumeroCategoriasPermisibles === 0 || plan.NumeroCategoriasPermisibles === null
                        || plan.NumeroCategoriasPermisibles === undefined) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    }
                    return true;

                },
                cleanData: function () {
                    $scope.PaymentPlans.planToSave = {
                        PlanesContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }, { Descripcion: '', CodigoIdioma: 3 }],
                        Precio: 0, CodigoPeriodicidad: 1, CodigoTipoPerfil: 1
                    };
                    $scope.imageDefault = '../Content/assets/img/no-image.png';
                }
            }

            $scope.PaymentHistory = {
                list: [],
                historyToUpdate: {},
                status: [{ Consecutivo: 3, Descripcion: 'Rechazado' }, { Consecutivo: 4, Descripcion: 'Aprobado' }],
                filter: { TakeIndexBase: 15, SkipIndexBase: 0 },
                btnBefore: false,
                btnNext: true,
                get: function () {
                    $scope.PaymentHistory.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetPaymentHistory', $scope.PaymentHistory.filter, function (res) {
                        if (res.data.Success) {
                            $scope.PaymentHistory.list = res.data.list;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getNext: function () {
                    if (!$scope.PaymentHistory.btnNext) return;
                    $scope.PaymentHistory.filter.SkipIndexBase = $scope.PaymentHistory.filter.TakeIndexBase;
                    $scope.PaymentHistory.filter.TakeIndexBase += 15;
                    $scope.PaymentHistory.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetPaymentHistory', $scope.PaymentHistory.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length > 0)
                                $scope.PaymentHistory.list = res.data.list;
                            else {
                                $scope.PaymentHistory.filter.SkipIndexBase -= 15;
                                $scope.PaymentHistory.btnNext = false;
                            }
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                getBefore: function () {
                    if ($scope.PaymentHistory.filter.SkipIndexBase === 0) {
                        $scope.PaymentHistory.btnBefore = false;
                        return;
                    }
                    $scope.PaymentHistory.filter.SkipIndexBase -= 15;
                    $scope.PaymentHistory.filter.TakeIndexBase = 15;
                    $scope.PaymentHistory.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetPaymentHistory', $scope.PaymentHistory.filter, function (res) {
                        if (res.data.Success) {
                            $scope.PaymentHistory.list = res.data.list;
                            $scope.PaymentHistory.btnNext = true;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                update: function () {
                    $scope.PaymentHistory.historyToUpdate.CodigoEstado = $scope.PaymentHistory.historyToUpdate.status.Consecutivo;
                    service.post(controller + 'UpdatePaymentHistory', $scope.PaymentHistory.historyToUpdate, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.PaymentHistory.filter.TakeIndexBase = 15;
                            $scope.PaymentHistory.filter.SkipIndexBase = 0;
                            $scope.PaymentHistory.btnBefore = false;
                            $scope.PaymentHistory.btnNext = true;
                            $('#modalHistory').modal('hide');
                            $scope.PaymentHistory.get();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                showModalHistory: function (history) {
                    $scope.PaymentHistory.historyToUpdate = history;
                    $scope.PaymentHistory.historyToUpdate.status = { Consecutivo: history.CodigoEstado, Descripcion: $scope.PaymentHistory.getStatusById(history.CodigoEstado) };
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

            $scope.PaymentPlans.getPlans();
            $scope.PaymentHistory.get();

        }]
    )
})();