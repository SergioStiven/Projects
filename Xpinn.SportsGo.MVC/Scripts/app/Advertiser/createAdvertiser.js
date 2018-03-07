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
    .config(['$translateProvider', function ($translateProvider) {
        // add translation table
        $translateProvider.translations('1', translationsES);
        $translateProvider.translations('2', translationsEN);
        $translateProvider.translations('3', translationsPOR);
        $translateProvider.preferredLanguage('1');
        $translateProvider.fallbackLanguage('1');
        $translateProvider.useSanitizeValueStrategy('escape');
    }])
    .controller('MasterController', ["$translate", "$sce", "$scope", "$rootScope", "service", function ($translate, $sce, $scope, $rootScope, service) {
        $scope.Profile = {
            get: function () {
                service.get('Profile/GetProfile', null, function (res) {
                    if (res.data.Success) {
                        $translate.use(res.data.obj.PersonaDelUsuario.CodigoIdioma.toString());
                        $rootScope.$emit("onLanguage", { langKey: res.data.obj.PersonaDelUsuario.CodigoIdioma.toString() });
                    }
                    else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                })
            }
        }

        $scope.Profile.get();

    }])
    .controller('CreateAdvertiserController',
        ["$scope", '$filter', "service", "Upload", "$timeout", "$rootScope", function ($scope, $filter, service, Upload, $timeout, $rootScope) {

            var controller = 'Advertisements/';

            $scope.ConverToDate = function (date) {
                return service.formatoFecha(date);
            }

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
                
            $scope.lenguages = {
                list: [{ Consecutivo: 1, Descripcion: 'Español' }, { Consecutivo: 2, Descripcion: 'Inglés' }, { Consecutivo: 3, Descripcion: 'Portugués' }],
                idLenguageNavigator: service.getLenguageFromNavigator(),
                lenguageText: function (id) {
                    return service.getLenguageById(id);
                }
            };

            $scope.Advertiser = {
                filter: { SkipIndxBase: 0, TakeIndexBase: 30 },
                list: [],
                post: {
                    AnunciosContenidos: [
                        { CodigoIdioma: 1, Titulo: '', Descripcion: '' },
                        { CodigoIdioma: 2, Titulo: '', Descripcion: '' },
                        { CodigoIdioma: 3, Titulo: '', Descripcion: '' }]
                    , AnunciosPaises: [], CategoriasAnuncios: [], CodigoArchivo: 0, NumeroApariciones: 0, FechaInicio: new Date()
                    , Categories: [], Countries: [], UrlPublicidad: '', http: 'https://', Vencimiento: new Date()
                },
                isBusy: false,
                uploadFile: function () {
                    if (!$scope.Advertiser.validateForm()) return;
                    if ($scope.Advertiser.post.Categories.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                        return;
                    } else if ($scope.Advertiser.post.Countries.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_COUNTRIES'), service.getTypeMessage('error'));
                        return;
                    }
                    if (!$scope.picFilePost) {
                        $scope.Advertiser.post.CodigoArchivo = null;
                        $scope.Advertiser.createPost();
                        return;
                    }
                    $scope.Advertiser.isBusy = true;
                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadVideoToControlDuration',
                        data: { file: $scope.picFilePost }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.Advertiser.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                            $scope.Advertiser.createPost();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }

                        $scope.Advertiser.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                createPost: function () {
                    $scope.Advertiser.isBusy = true;
                    if ($scope.Advertiser.post.UrlPublicidad != '') {
                        if (!$scope.Advertiser.post.UrlPublicidad.toLowerCase().match("^http")) {
                            $scope.Advertiser.post.UrlPublicidad = $scope.Advertiser.post.http + $scope.Advertiser.post.UrlPublicidad;
                        }
                    }
                    $scope.Advertiser.post.CategoriasAnuncios = $scope.Advertiser.post.Categories.map(function (x) {
                        return { CodigoCategoria: parseInt(x.Consecutivo, 10) };
                    });

                    $scope.Advertiser.post.AnunciosPaises = $scope.Advertiser.post.Countries.map(function (x) {
                        return { CodigoPais: parseInt(x.Consecutivo, 10) };
                    });

                    service.post(controller + 'CreatePostsByAdvertiser', $scope.Advertiser.post, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.Advertiser.clearFields();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.Advertiser.isBusy = false;
                    })                        
                },
                showTitle: function () {
                    if ($scope.Advertiser.post.AnunciosContenidos[1].Titulo === '' || $scope.Advertiser.post.AnunciosContenidos[1].Titulo == undefined
                        || $scope.Advertiser.post.AnunciosContenidos[2].Titulo === '' || $scope.Advertiser.post.AnunciosContenidos[2].Titulo == undefined
                        || $scope.Advertiser.post.AnunciosContenidos[1].Descripcion === '' || $scope.Advertiser.post.AnunciosContenidos[1].Descripcion == undefined
                        || $scope.Advertiser.post.AnunciosContenidos[2].Descripcion === '' || $scope.Advertiser.post.AnunciosContenidos[2].Descripcion == undefined) {
                        $scope.addTitleAndDescription = true;
                    }
                    else
                        $scope.addTitleAndDescription = false;
                },
                validateForm: function () {

                    if ($scope.Advertiser.post.AnunciosContenidos[0].Descripcion === '' || $scope.Advertiser.post.AnunciosContenidos[1].Descripcion === ''
                        || $scope.Advertiser.post.AnunciosContenidos[2].Descripcion === '' || $scope.Advertiser.post.AnunciosContenidos[0].Titulo === ''
                        || $scope.Advertiser.post.AnunciosContenidos[1].Titulo === '' || $scope.Advertiser.post.AnunciosContenidos[2].Titulo === ''
                        || $scope.Advertiser.post.FechaInicio === 'mm/dd/yyyy' || $scope.Advertiser.post.FechaInicio === '' || $scope.Advertiser.post.FechaInicio === null) {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    } else{
                        var today = new Date().toJSON().slice(0, 10).replace(/-/g, '/');
                        var FechaInicio = $scope.Advertiser.post.FechaInicio.toJSON().slice(0, 10).replace(/-/g, '/');
                        if (FechaInicio < today) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_RAGE_DATE'), service.getTypeMessage('error'));
                            return false;
                        }
                    }
                    return true;
                },
                clearFields: function () {
                    $scope.Advertiser.post = {
                            AnunciosContenidos: [
                                { CodigoIdioma: 1, Titulo: '', Descripcion: '' },
                                { CodigoIdioma: 2, Titulo: '', Descripcion: '' },
                                { CodigoIdioma: 3, Titulo: '', Descripcion: '' }]
                            , AnunciosPaises: [], CategoriasAnuncios: [], CodigoArchivo: 0, NumeroApariciones: 0, FechaInicio: new Date()
                            , Categories: [], Countries: [], UrlPublicidad: '', http: 'https://', Vencimiento: new Date()
                    }
                    $scope.picFilePost = undefined;
                },
                addDays: function () {
                    var newDate = new Date($scope.Advertiser.post.FechaInicio);
                    var result = newDate.setDate(newDate.getDate() + $scope.Planes.plan.Planes.NumeroDiasVigenciaAnuncio);
                    $scope.Advertiser.post.Vencimiento = result;
                }
            };

            $scope.Planes = {
                plan: {},
                getMyPlan: function () {
                    service.get('Settings/GetMyPlan', null, function (res) {
                        if(res.data.Success){
                            $scope.Planes.plan = res.data.obj;
                            console.log($scope.Planes.plan);
                            $scope.Advertiser.post.NumeroApariciones = $scope.Planes.plan.Planes.NumeroAparicionesAnuncio;
                            $scope.Advertiser.addDays();
                        } else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                }
            }

            // Carga lista de categorias, luego lista de paises 
            $scope.lists.fillList();
            $scope.Planes.getMyPlan();
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
