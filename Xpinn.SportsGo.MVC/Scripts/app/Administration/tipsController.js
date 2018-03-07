(function () {
    'use strict';
    var dependencies =
        [
            "oi.select",
            "ngFileUpload"
        ]

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app
    .controller('NewsController', ["$translate", "$sce", "$rootScope", "$scope", "service", "Upload", "$timeout",
        function ($translate, $sce, $rootScope, $scope, service, Upload, $timeout) {

            var controller = 'Administration/';

            $scope.convertToDate = function (date) {
                return service.formatoFecha(date);
            }

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

            $rootScope.$on('GetMoreTips', function (event, obj) {
                $scope.New.get();
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
                https: [{ value: 'http://', name: 'http' }, { value: 'https://', name: 'https' }],
                typesNews: [{ Consecutivo: 1, DescripcionIdiomaBuscado: 'Timeline' }, { Consecutivo: 2, DescripcionIdiomaBuscado: 'Notificacion' }],
                filter: { TakeIndexBase: 100, SkipIndexBase: 0 },
                getCountries: function () {
                    service.post(controller + 'GetListCountries', { CodigoIdiomaUsuarioBase: 1 }, function (res) {
                        if (res.data.Success) {
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].CodigoPais = res.data.list[i].Consecutivo;
                            }
                            $scope.lists.countries = res.data.list;
                            $scope.lists.getCategories();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                getCategories: function () {
                    service.get(controller + 'GetListCategories', null, function (res) {
                        if (res.data.Success) {
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].CodigoCategoria = res.data.list[i].Consecutivo;
                            }
                            $scope.lists.categories = res.data.list;
                            $scope.New.get();
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                fillList: function () {
                    $scope.lists.getCountries();
                }
            }

            $scope.lenguages = {
                list: [{ Consecutivo: 1, Descripcion: 'Español' }, { Consecutivo: 2, Descripcion: 'Inglés' }, { Consecutivo: 3, Descripcion: 'Portugués' }],
                idLenguageNavigator: service.getLenguageFromNavigator(),
                lenguageText: function (id) {
                    return service.getLenguageById(id);
                }
            };

            $scope.New = {
                list: [],
                addNew: false,
                isBusy: false,
                endList: false,
                postFileTemp: null,
                post: {
                    UrlPublicidad: '', http: 'https://',
                    NoticiasContenidos: [{ CodigoIdioma: 1 }, { CodigoIdioma: 2 }, { CodigoIdioma: 3 }], NoticiasPaises: [], CategoriasNoticias: [], CodigoArchivo: 0
                },
                filter: { TakeIndexBase: 3, SkipIndexBase: -3 },
                get: function () {
                    if ($scope.New.isBusy || $scope.New.endList) return;
                    $scope.New.isBusy = true;
                    $scope.New.filter.SkipIndexBase += 3;
                    $scope.New.filter.ZonaHorariaGMTBase = service.getTimeZone();
                    service.post(controller + 'GetListNews', $scope.New.filter, function (res) {
                        console.log('entró!!');
                        if (res.data.Success) {
                            if (res.data.list.length < 3) {
                                $scope.New.endList = true;
                            }
                            for (var i = 0; i < res.data.list.length; i++) {
                                res.data.list[i].picFilePostForEdit = null;
                                if (res.data.list[i].UrlPublicidad !== null) {
                                    if (res.data.list[i].UrlPublicidad.includes('https://')) {
                                        res.data.list[i].UrlPublicidad = res.data.list[i].UrlPublicidad.replace('https://', '');
                                        res.data.list[i].http = 'https://';
                                    }
                                    else {
                                        res.data.list[i].UrlPublicidad = res.data.list[i].UrlPublicidad.replace('http://', '');
                                        res.data.list[i].http = 'http://';
                                    }
                                } else {
                                    res.data.list[i].http = 'https://';
                                }
                                //res.data.list[i].listCategoriesAvailable = $scope.New.filteredCategories(res.data.list[i].CategoriasNoticias);
                                //res.data.list[i].listCountriesAvailable = $scope.New.filteredCountries(res.data.list[i].NoticiasPaises);
                            }
                            $scope.New.list = $scope.New.list.concat(res.data.list);
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        $scope.New.isBusy = false;
                    })
                },
                uploadFile: function () {
                    $scope.New.isBusy = true;
                    if (!$scope.New.validateForm($scope.New.post, $scope.picFilePost)) {
                        $scope.New.isBusy = false;
                        return;
                    }
                    if (!$scope.picFilePost) {
                        $scope.New.post.CodigoArchivo = null;
                        $scope.New.save();
                        return;
                    }
                    Upload.upload({
                        url: service.urlBase + 'Profile/UploadFile',
                        data: { file: $scope.picFilePost, ConsecutivoArchivo: 0 }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.New.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                            $scope.New.save();
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }

                        $scope.New.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                updateFile: function (post) {
                    $scope.New.isBusy = true;
                    if (!$scope.New.validateForm(post, $scope.New.postFileTemp)) {
                        $scope.New.isBusy = false;
                        return;
                    }
                    if ($scope.New.postFileTemp == null) {
                        $scope.New.update(post);
                        return;
                    }
                    if (post.CodigoArchivo == null) post.CodigoArchivo = 0;
                    Upload.upload({
                        url: service.urlBase + controller + 'UpdateFilePost',
                        data: {
                            file: $scope.New.postFileTemp,
                            Consecutivo: post.Consecutivo,
                            CodigoArchivo: post.CodigoArchivo
                        }
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.New.update(post);
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.New.isBusy = false;
                        }

                        $scope.New.isBusy = false;

                    }), function (res) {
                        if (res.status > 0)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    }, function (evt) {
                        $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                    };
                },
                save: function () {
                    if ($scope.New.post.UrlPublicidad.trim() !== '' && $scope.New.post.UrlPublicidad !== null) {
                        if (!$scope.New.post.UrlPublicidad.toLowerCase().match("^http")) {
                            $scope.New.post.UrlPublicidad = $scope.New.post.http + $scope.New.post.UrlPublicidad;
                        }
                    }
                    service.post(controller + 'CreatePost', $scope.New.post, function (res) {
                        if (res.data.Success) {
                            $scope.New.post = {
                                UrlPublicidad: '', http: 'https://', NoticiasPaises: [], CategoriasNoticias: [], CodigoArchivo: 0,
                                NoticiasContenidos: [{ CodigoIdioma: 1 }, { CodigoIdioma: 2 }, { CodigoIdioma: 3 }]
                            }
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.New.get();
                            $scope.New.isBusy = false;
                            $scope.picFilePost = null;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.New.isBusy = false;
                        }
                    })
                },
                update: function (post) {
                    console.log(post);
                    if (post.UrlPublicidad !== null) {
                        if (post.UrlPublicidad.trim() !== '') {
                            if (!post.UrlPublicidad.toLowerCase().match("^http")) {
                                post.UrlPublicidad = post.http + post.UrlPublicidad;
                            }
                        }
                    }
                    service.post(controller + 'CreatePost', post, function (res) {
                        if (res.data.Success) {
                            $scope.New.clear();
                            $scope.New.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.New.isBusy = false;
                        }
                    })
                },
                delete: function (newToDelete) {
                    service.post(controller + 'DeletePost', { Consecutivo: newToDelete.Consecutivo }, function (res) {
                        if (res.data.Success) {
                            $scope.New.clear();
                            $scope.New.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                filteredCategories: function (toFilter) {
                    var idsCategories = toFilter.map(function (a) { return a.CodigoCategoria; });
                    return $scope.lists.categories.filter(function (category) { return idsCategories.indexOf(category.Consecutivo) === -1 });
                },
                filteredCountries: function (toFilter) {
                    var idsCountries = toFilter.map(function (a) { return a.CodigoPais; });
                    return $scope.lists.countries.filter(function (country) { return idsCountries.indexOf(country.Consecutivo) === -1 });
                },
                validateForm: function (form, image) {
                    if (form.NoticiasContenidos[0].Descripcion === '' || form.NoticiasContenidos[1].Descripcion === ''
                        || form.NoticiasContenidos[2].Descripcion === '' || form.NoticiasContenidos[0].Titulo === ''
                        || form.NoticiasContenidos[1].Titulo === '' || form.NoticiasContenidos[2].Titulo === '') {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    }
                    else if (form.CategoriasNoticias.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                        return false;
                    } else if (form.NoticiasPaises.length === 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_COUNTRIES'), service.getTypeMessage('error'));
                        return false;
                    }
                    if (form.CodigoTipoNoticia === 2 && !image) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_IMAGE_NOTIFICATION'), service.getTypeMessage('error'));
                        return false;
                    }
                    return true;
                },
                showTitle: function () {
                    if ($scope.New.post.NoticiasContenidos[1].Titulo === '' || $scope.New.post.NoticiasContenidos[1].Titulo == undefined
                        || $scope.New.post.NoticiasContenidos[2].Titulo === '' || $scope.New.post.NoticiasContenidos[2].Titulo == undefined
                        || $scope.New.post.NoticiasContenidos[1].Descripcion === '' || $scope.New.post.NoticiasContenidos[1].Descripcion == undefined
                        || $scope.New.post.NoticiasContenidos[2].Descripcion === '' || $scope.New.post.NoticiasContenidos[2].Descripcion == undefined) {
                        $scope.addTitleAndDescription = true;
                    }
                    else
                        $scope.addTitleAndDescription = false;
                },
                changeFilePost: function (file) {
                    $scope.New.postFileTemp = file;
                },
                clear: function () {
                    $scope.New.isBusy = false;
                    $scope.New.endList = false;
                    $scope.New.filter = { TakeIndexBase: 3, SkipIndexBase: -3 };
                    $scope.New.list = [];
                }
            }

            $scope.Rss = {
                viewFormRss: false,
                viewButtonCreateRss: true,
                rssToSave: [{ Consecutivo: 0, http: 'https://', CodigoIdioma: 1, UrlFeed: '' }, { Consecutivo: 0, http: 'https://', CodigoIdioma: 2, UrlFeed: '' }, { Consecutivo: 0, http: 'https://', CodigoIdioma: 3, UrlFeed: '' }],
                get: function () {
                    service.get(controller + 'GetListRssFeed', null, function (res) {
                        if (res.data.Success) {
                            if (res.data.list.length > 0) {
                                res.data.list.sort(function (a, b) { return a.CodigoIdioma - b.CodigoIdioma; });
                                for (var i = 0; i < 3; i++) {
                                    if (res.data.list[i].CodigoIdioma !== (i + 1)) {
                                        res.data.list.splice(i, 0, { http: 'https://', CodigoIdioma: (i + 1), UrlFeed: '', Consecutivo: 0 });
                                    } else {
                                        if (res.data.list[i].UrlFeed.includes('https://')) {
                                            res.data.list[i].UrlFeed = res.data.list[i].UrlFeed.replace('https://', '');
                                            res.data.list[i].http = 'https://';
                                        }
                                        else {
                                            res.data.list[i].UrlFeed = res.data.list[i].UrlFeed.replace('http://', '');
                                            res.data.list[i].http = 'http://';
                                        }
                                    }
                                }
                                $scope.Rss.rssToSave = res.data.list;
                            }
                            $scope.Rss.viewButtonCreateRss = false;
                        }
                        else service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                save: function () {
                    if (!$scope.Rss.validateForm()) return;

                    var newRssToSave = JSON.parse(JSON.stringify($scope.Rss.rssToSave));
                    console.log(newRssToSave);
                    for (var i = 0; i < newRssToSave.length; i++) {
                        if (newRssToSave[i].UrlFeed !== '') {
                            if (!newRssToSave[i].UrlFeed.toLowerCase().match("^http")) {
                                newRssToSave[i].UrlFeed = newRssToSave[i].http + newRssToSave[i].UrlFeed;
                            }
                        }
                        else {
                            newRssToSave.splice(i, 1);
                            i--;
                        }
                    }
                    $scope.Rss.isBusy = true;
                    service.post(controller + 'CreateRssFeed', newRssToSave, function (res) {
                        if (res.data.Success) {
                            $scope.Rss.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                        $scope.Rss.isBusy = false;
                    })
                },
                delete: function (rssToDelete) {
                    service.post(controller + 'DeleteRssFeed', rssToDelete, function (res) {
                        if (res.data.Success) {
                            $scope.Rss.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }
                    })
                },
                validateForm: function () {
                    if ($scope.Rss.rssToSave[0].UrlFeed !== '' || $scope.Rss.rssToSave[1].UrlFeed !== '' || $scope.Rss.rssToSave[2].UrlFeed !== '') {
                        return true;
                    }
                    else {
                        service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                        return false;
                    }
                }
            }

            $scope.lists.fillList();
            $scope.Rss.get();
        }]
    )
    .directive("scroll", function ($window, $rootScope) {
        return function (scope, element, attrs) {
            angular.element($('#containerDark')).bind("scroll", function () {
                if ($('#containerDark').scrollTop() + $(window).height() > $('.content-responsive').height()) {
                    $rootScope.$emit("GetMoreTips", {});
                }
                scope.$apply();
            });
        };
    })
    .directive('newsList', newsList)
    .directive('rssForm', rssForm);
    newsList.$inject = ['service'];
    rssForm.$inject = ['service'];
        function newsList(service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Administration/ListOfNews'
            }
        };
        function rssForm(service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Administration/RssForm'
            }
        };
})();