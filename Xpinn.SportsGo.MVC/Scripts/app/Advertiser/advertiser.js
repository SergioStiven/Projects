(function () {
    'use strict';
    var dependencies =
        [
            "ngSanitize",
            "com.2fdevs.videogular",
            "com.2fdevs.videogular.plugins.controls",
            "com.2fdevs.videogular.plugins.overlayplay",
            "com.2fdevs.videogular.plugins.poster",
            "ngFileUpload",
            "uiCropper",
            "oi.select"
            //"ngImgCrop"
        ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
    .controller('AdvertiserController',
        ["$translate", "$sce", "$scope", '$filter', "service", "Upload", "$timeout", "$rootScope", function ($translate,$sce, $scope, $filter, service, Upload, $timeout, $rootScope) {

            var controller = 'Advertisements/';

            $scope.ConverToDate = function (date) {
                return service.formatoFecha(date);
            }
            
            $rootScope.$on('GetMoreAds', function (event, obj) {
                $scope.Posts.get();
            });

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

            $scope.lists = {
                countries: [],
                categories: [],
                lenguages: [{ id: 1, idioma: 'Español' }, { id: 2, idioma: 'Inglés' }, { id: 3, idioma: 'Portugués' }],
                typesAds: [{ Consecutivo: 1, DescripcionIdiomaBuscado: 'Timeline' }, { Consecutivo: 2, DescripcionIdiomaBuscado: 'Lateral' }],
                https: [{ value: 'http://', name: 'http' }, { value: 'https://', name: 'https' }],
                getCountries: function () {
                    service.post('Administration/GetListCountries', { CodigoIdiomaUsuarioBase: 1 }, function (res) {
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

            $scope.Posts = {
                filter: { SkipIndexBase: -3, TakeIndexBase: 3 },
                list: [],
                post: { CodigoArchivo: 0, CodigoCandidato: 0, Titulo: '', Descripcion: '', CategoriasEventos: [] },
                isBusy: false,
                endList: false,
                uploadFile: function () {
                    if (!$scope.Posts.validateForm($scope.Posts.post)) {
                        $scope.Posts.isBusy = false;
                        return;
                    }
                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadFile',
                        data: { file: $scope.picFilePost, ConsecutivoArchivo: 0 }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.Posts.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                            $scope.Posts.createPost();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }

                        $scope.Posts.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                updateFile: function (post, picFilePost) {
                    $scope.Posts.isBusy = true;
                    if (!$scope.Posts.validateForm(post)) {
                        $scope.Posts.isBusy = false;
                        return;
                    }

                    if (!picFilePost) {
                        $scope.Posts.post = post;
                        $scope.Posts.createPost();
                        return;
                    }
                    post.CodigoArchivo = post.CodigoArchivo === null ? 0 : post.CodigoArchivo;
                    Upload.upload({
                        url: service.urlBase + controller + 'UpdateFile',
                        data: {
                            file: picFilePost,
                            Consecutivo: post.Consecutivo, 
                            CodigoArchivo: post.CodigoArchivo
                        }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.Posts.post = post;
                            $scope.Posts.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                            $scope.Posts.createPost();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }

                        $scope.Posts.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                createPost: function () {
                    $scope.Posts.isBusy = true;
                    if ($scope.Posts.post.UrlPublicidad != '') {
                        if (!$scope.Posts.post.UrlPublicidad.toLowerCase().match("^http")) {
                            $scope.Posts.post.UrlPublicidad = $scope.Posts.post.http + $scope.Posts.post.UrlPublicidad;
                        }
                    }
                    service.post(controller + 'CreatePostsByAdvertiser', $scope.Posts.post, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.Posts.clear();
                            $scope.Posts.get();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                        $scope.Posts.isBusy = false;
                    })
                },
                get: function () {
                    if ($scope.Posts.isBusy || $scope.Posts.endList) return;
                    $scope.Posts.isBusy = true;
                    $scope.Posts.filter.SkipIndexBase += 3;
                    service.post(controller + 'GetListPostsByAdvertiser', $scope.Posts.filter, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length < 3) {
                                $scope.Posts.endList = true;
                            }
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].FechaInicio = $scope.ConverToDate(res.data.list[i].FechaInicio);
                                res.data.list[i].Vencimiento = $scope.ConverToDate(res.data.list[i].Vencimiento);
                                if (res.data.list[i].UrlPublicidad.includes('https://')) {
                                    res.data.list[i].UrlPublicidad = res.data.list[i].UrlPublicidad.replace('https://', '');
                                    res.data.list[i].http = 'https://';
                                }                                    
                                else {
                                    res.data.list[i].UrlPublicidad = res.data.list[i].UrlPublicidad.replace('http://', '');
                                    res.data.list[i].http = 'http://';
                                }                                    
                            }
                            $scope.Posts.list = $scope.Posts.list.concat(res.data.list);
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.Posts.isBusy = false;
                    })
                        
                },
                getById: function (obj) {   
                    service.post(controller + 'GetPostsByAdvertiser', obj, function (res) {
                        if (res.data.Success) {
                            res.data.obj.Vencimiento = $scope.ConverToDate(res.data.obj.Vencimiento);
                            $scope.Posts.post = res.data.obj;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                delete: function(ad){
                    service.post(controller + 'DeletePostsByAdvertiser', ad, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.Posts.clear();
                            $scope.Posts.get();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                validateForm: function (post) {
                    if (post.AnunciosContenidos.length < 3) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else if (post.AnunciosContenidos[0].Descripcion === '' || post.AnunciosContenidos[1].Descripcion === ''
                        || post.AnunciosContenidos[2].Descripcion === '' || post.AnunciosContenidos[0].Titulo === ''
                        || post.AnunciosContenidos[1].Titulo === '' || post.AnunciosContenidos[2].Titulo === ''
                        || post.FechaInicio === 'mm/dd/yyyy' || post.FechaInicio === '' || post.FechaInicio === null) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else if (post.AnunciosPaises.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_COUNTRIES'), service.getTypeMessage('error'));
                        return false;
                    } else if (post.CategoriasAnuncios.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                        return false;
                    } else {
                        var today = new Date().toJSON().slice(0, 10).replace(/-/g, '/');
                        var FechaInicio = post.FechaInicio.toJSON().slice(0, 10).replace(/-/g, '/');
                        if (FechaInicio < today) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_RAGE_DATE'), service.getTypeMessage('error'));
                            return false;
                        }
                    }
                    return true;
                },
                clear: function () {
                    $scope.isBusy = false;
                    $scope.endList = false;
                    $scope.Posts.filter = { SkipIndexBase: -3, TakeIndexBase: 3 };
                    $scope.Posts.list = [];
                }
            };

            // Carga lista de categorias, luego lista de paises y por último la información del perfil. Se necesita en ese orden
            $scope.lists.fillList();
            $scope.Posts.get();
        }]
    )
    .directive("scroll", function ($window, $rootScope) {
        return function (scope, element, attrs) {
            angular.element($('#containerDark')).bind("scroll", function () {
                if ($('#containerDark').scrollTop() + $(window).height() > $('.content-responsive').height()) {
                    $rootScope.$emit("GetMoreAds", {});
                }
                scope.$apply();
            });
        };
    })
    .directive('newsList', newsList);
    newsList.$inject = ['service'];
    function newsList(service) {
        return {
            restrict: 'E',
            templateUrl: service.urlBase + 'Advertisements/Posts'
        }
    };
})();
