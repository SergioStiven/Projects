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
    .constant("typeAbility", {
        0: "SinTipoHabilidad",
        1: "Tecnica",
        2: "Tactica",
        3: "Fisica",
    })
    .controller('ListController',
        ["$translate", "$sce", "$scope", "service", "Upload", "$timeout", "typeAbility", function ($translate,$sce, $scope, service, Upload, $timeout, typeAbility) {

            var controller = 'Administration/';

            $scope.lenguages = {
                list: [{ Consecutivo: 1, Descripcion: 'Español' }, { Consecutivo: 2, Descripcion: 'Inglés' }, { Consecutivo: 3, Descripcion: 'Portugués' }],
                idLenguageNavigator: service.getLenguageFromNavigator(),
                lenguageText: function (id) {
                    return service.getLenguageById(id);
                }
            };

            $scope.lenguage = service.getLenguageFromNavigator();
            $scope.lenguageText = function (id) {
                return service.getLenguageById(id);
            }
            $scope.imageDefault = '../Content/assets/img/no-image.png';
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

            $scope.abilitiesInfo = { 
                list: [],
                abilityDetil: {},
                listAbilitiesTemp: [],
                abilitieTemp: { HabilidadesContenidos: [{ Descripcion: '' }, { Descripcion: '' }, { Descripcion: '' }], TipoHabilidad: 1 },
                currentCategory:null,
                add: false,
                addAbilitie: function () {
                    $scope.abilitiesInfo.add = !$scope.abilitiesInfo.add;
                },
                openBlankModal: function () {
                        
                    $('#modalAbilities').modal('toggle');
                },
                showAbility: function (ability) {
                    service.post(controller + 'GetContentAbility', ability, function (res) {
                        if (res.data.Success) {
                            $scope.abilitiesInfo.abilityDetil = ability;
                            $scope.abilitiesInfo.abilityDetil.HabilidadesContenidos = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                typesAbility: [
                    { CodigoTipoHabilidad: 1, name: 'Técnica' },
                    { CodigoTipoHabilidad: 2, name: 'Táctica' },
                    { CodigoTipoHabilidad: 3, name: 'Física' }
                ],
                getTypeAbility: function(id){
                    return typeAbility[id];
                },
                resetListAbilities: function(){
                    $scope.abilitiesInfo.list = [];
                    $scope.abilitiesInfo.currentCategory = null;
                    $scope.abilitiesInfo.add = false;
                    $scope.abilitiesInfo.abilitieTemp = { HabilidadesContenidos: [{ Descripcion: '' }, { Descripcion: '' }, { Descripcion: '' }], TipoHabilidad: 1 };
                },
                getByCategory: function (categorieToSearch) {
                    if (!categorieToSearch || categorieToSearch == null) return;
                    service.post(controller + 'GetListAbilitiesByCategoryAndLenguage', categorieToSearch, function (res) {
                        if (res.data.Success) {
                            $scope.abilitiesInfo.list = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                save: function () {
                    if ($scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos.length <= 0) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_ALL_DESCRIPTION'), service.getTypeMessage('error'));
                        return;
                    }
                    for (var i in $scope.lenguages.list) {
                        if ($.trim($scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos[i].Descripcion) === '') {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_ALL_DESCRIPTION'), service.getTypeMessage('error'));
                            return;
                        }else
                            $scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos[i].CodigoIdioma = $scope.lenguages.list[i].Consecutivo;
                    }
                    $scope.abilitiesInfo.abilitieTemp.CodigoCategoria = $scope.abilitiesInfo.currentCategory.Consecutivo;
                    service.post(controller + 'SaveAbilitie', $scope.abilitiesInfo.abilitieTemp, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                            $scope.abilitiesInfo.add = false;
                            $scope.abilitiesInfo.getByCategory($scope.abilitiesInfo.currentCategory);
                            $scope.abilitiesInfo.abilitieTemp = { HabilidadesContenidos: [] }
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                        
                },
                update: function (abilityForUpdate) {
                    service.post(controller + 'UpdateAbilitie', abilityForUpdate, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                delete: function () {
                    service.post(controller + 'DeleteAbility', $scope.ToDelete.obj, function (res) {
                        if (res.data.Success) {
                            $scope.abilitiesInfo.getByCategory($scope.abilitiesInfo.currentCategory);
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                        }
                        else {
                            service.showErrorMessage($translate.instant('NOTI_ERROR_DELETE_BY_USER'), service.getTypeMessage('error'));
                        }
                    })
                },
            };
                
            $scope.categoriesInfo = {
                isBusy: false,
                list: [],
                listDetail: [],
                indexCurrentCategory: 0,
                image: '',
                imageTemp: {
                    data: null,
                    name: '',
                    isDefault: true
                },
                categoryToUpdate: {},
                categoryTemp: { Habilidades: [], ArchivoContenido: null, CategoriasContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }, { Descripcion: '', CodigoIdioma: 3 }] },
                add: false,
                addCategory: function () {
                    $scope.categoriesInfo.add = !$scope.categoriesInfo.add;
                    $scope.abilitiesInfo.list = [];
                    $scope.categoriesInfo.imageTemp.isDefault = true;
                    $scope.imageDefault = '../Content/assets/img/no-image.png';
                },
                get: function () {
                    service.get(controller + 'GetListCategories', null, function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.list = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    });
                },
                save: function () {
                    if ($scope.categoriesInfo.isBusy) return;
                    for (var i in $scope.lenguages.list) {
                        if ($.trim($scope.categoriesInfo.categoryTemp.CategoriasContenidos[i].Descripcion) === '') {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_ALL_DESCRIPTION'), service.getTypeMessage('error'));
                            return;
                        }
                    }
                    $scope.categoriesInfo.categoryTemp.Habilidades = $scope.abilitiesInfo.list;
                    if ($scope.categoriesInfo.imageTemp.isDefault) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_IMAGE'), service.getTypeMessage('error'));
                        return;
                    }
                    $scope.categoriesInfo.isBusy = true;
                    Upload.upload({
                        url: service.urlBase + controller + 'SaveCategorie',
                        data: {
                            file: Upload.dataUrltoBlob($scope.categoriesInfo.imageTemp.data, $scope.categoriesInfo.imageTemp.name),
                            categoryForSave: JSON.stringify($scope.categoriesInfo.categoryTemp)
                        },
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.categoriesInfo.categoryTemp = { Habilidades: [], ArchivoContenido: null, CategoriasContenidos: [{ Descripcion: '', CodigoIdioma: 1 }, { Descripcion: '', CodigoIdioma: 2 }, { Descripcion: '', CodigoIdioma: 3 }] },
                            $scope.categoriesInfo.add = false;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                        $scope.categoriesInfo.isBusy = false;
                    })
                },
                update: function (categorieForUpdate) {
                    if ($scope.categoriesInfo.isBusy) return;
                    $scope.categoriesInfo.isBusy = true;
                    service.post(controller + 'UpdateCategorie', categorieForUpdate, function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.get();
                            service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('error'));
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                        $scope.categoriesInfo.isBusy = false;
                    })
                },
                delete: function () {
                    service.post(controller + 'DeleteCategorie', { Consecutivo: $scope.ToDelete.obj.Consecutivo, CodigoArchivo: $scope.ToDelete.obj.CodigoArchivo }, function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                        }
                        else {
                            service.showErrorMessage($translate.instant('NOTI_ERROR_DELETE_BY_USER'), service.getTypeMessage('error'));
                        }
                    })
                },
                getAbilities: function (categorieToSearch) {
                    $scope.abilitiesInfo.list = [];
                    $scope.abilitiesInfo.currentCategory = categorieToSearch;
                    $scope.abilitiesInfo.add = false;
                    $scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos[0].Descripcion = '';
                    $scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos[1].Descripcion = '';
                    $scope.abilitiesInfo.abilitieTemp.HabilidadesContenidos[2].Descripcion = '';
                    $('#modalAbilities').modal('toggle');
                    $scope.abilitiesInfo.getByCategory(categorieToSearch);
                },
                showCategoryById: function (categoryForSearch) {
                    $scope.categoriesInfo.listDetail = [];
                    $('#modalCategorie').modal('toggle');
                    service.post(controller + 'GetCategoryById', categoryForSearch, function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.listDetail = res.data.obj.CategoriasContenidos;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                uploadImageTemp: function (dataImage, name) {
                    $scope.categoriesInfo.imageTemp.data = dataImage;
                    $scope.categoriesInfo.imageTemp.name = name;
                    $scope.categoriesInfo.imageTemp.isDefault = false;
                    $scope.imageDefault = dataImage;
                    $('#modalImageCategory').modal('hide');
                },
                updateImage: function (dataImage, nameImage) {
                    if ($scope.categoriesInfo.isBusy) return;
                    if ($scope.categoriesInfo.add) {
                        $scope.categoriesInfo.uploadImageTemp(dataImage, nameImage);
                        return;
                    }
                    $scope.categoriesInfo.isBusy = true;
                    Upload.upload({
                        url: service.urlBase + controller + 'uploadImageCategory',
                        data: {
                            file: Upload.dataUrltoBlob(dataImage, nameImage),
                            Categorias: JSON.stringify($scope.categoriesInfo.categoryToUpdate)
                        },
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.categoriesInfo.categoryToUpdate.UrlArchivo = dataImage;
                            $('#modalImageCategory').modal('hide');
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                        $scope.categoriesInfo.isBusy = false;
                    })
                },
                showModalImageCategory: function (categoryToModify) {
                    $scope.categoriesInfo.add = false;
                    $('#imageCategory').attr('src', '');
                    $scope.picFileCategory = undefined;
                    $scope.categoriesInfo.categoryToUpdate = categoryToModify;
                    $('#modalImageCategory').modal('toggle');
                }
            };

            $scope.countriesInfo = {
                list: [],
                detail: {},
                countryTemp: { CodigoIdioma: $scope.lenguages.idLenguageNavigator, PaisesContenidos: [ { Descripcion: '', CodigoIdioma: 0} ], CodigoMoneda: 0, Monedas: {} },
                add: false,
                indexCurrentCountry: 0,
                listCurrencies: [],
                imageTemp: {
                    data: null,
                    name: '',
                    isDefault: true
                },
                addCountry: function () {
                    $scope.countriesInfo.add = !$scope.countriesInfo.add;
                    $scope.countriesInfo.imageTemp.isDefault = true;
                    $scope.imageDefault = '../Content/assets/img/no-image.png';

                },
                get: function () {
                    service.post(controller + 'GetListCountries', { IdiomaBase: 1 }, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.list = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                save: function () {
                    if ($scope.countriesInfo.imageTemp.isDefault) {
                        service.showErrorMessage($translate.instant('NOTI_VALIDATION_IMAGE'), service.getTypeMessage('error'));
                        return;
                    }
                    for (var i in $scope.lenguages.list) {
                        if ($.trim($scope.countriesInfo.countryTemp.PaisesContenidos[i].Descripcion) === '') {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_ALL_DESCRIPTION'), service.getTypeMessage('error'));
                            return;
                        }
                        $scope.countriesInfo.countryTemp.PaisesContenidos[i].CodigoIdioma = $scope.lenguages.list[i].Consecutivo;
                    }
                    $scope.countriesInfo.countryTemp.CodigoMoneda = $scope.countriesInfo.countryTemp.Monedas.Consecutivo;
                    $scope.countriesInfo.countryTemp.CodigoIdioma = service.getLenguageFromNavigator();
                    Upload.upload({
                        url: service.urlBase + controller + 'SaveCountry',
                        data: {
                            file: Upload.dataUrltoBlob($scope.countriesInfo.imageTemp.data, $scope.countriesInfo.imageTemp.name),
                            countryForSave: JSON.stringify($scope.countriesInfo.countryTemp)
                        },
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            $scope.countriesInfo.countryTemp = { CodigoIdioma: $scope.lenguages.idLenguageNavigator, PaisesContenidos: [{ Descripcion: '', CodigoIdioma: 0 }], CodigoMoneda: 0, Monedas: {} };
                            $scope.countriesInfo.add = false;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                update: function () {
                    $scope.countriesInfo.detail.CodigoMoneda = $scope.countriesInfo.detail.Monedas.Consecutivo;
                    service.post(controller + 'UpdateCountry', $scope.countriesInfo.detail, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.get();
                            service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                            $('#modalCountry').modal('hide');
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                delete: function () {
                    service.post(controller + 'DeleteCountry', $scope.ToDelete.obj, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.get();
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                        }
                        else {
                            service.showErrorMessage($translate.instant('NOTI_ERROR_DELETE_BY_USER'), service.getTypeMessage('error'));
                        }
                    })
                },
                showCountryById: function (countryForSearch) {
                    $scope.countriesInfo.listDetail = [];
                    $('#modalCountry').modal('toggle');
                    service.post(controller + 'GetCountryById', countryForSearch, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.detail = res.data.obj;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                updateImage: function (dataImage, nameImage) {
                    if ($scope.countriesInfo.add) {
                        $scope.countriesInfo.imageTemp.data = dataImage;
                        $scope.countriesInfo.imageTemp.name = nameImage;
                        $scope.countriesInfo.imageTemp.isDefault = false;
                        $scope.imageDefault = dataImage;
                        $('#modalImageCountry').modal('hide');
                        return;
                    }
                    Upload.upload({
                        url: service.urlBase + controller + 'uploadImageCountry',
                        data: {
                            file: Upload.dataUrltoBlob(dataImage, nameImage),
                            Paises: JSON.stringify($scope.countriesInfo.countryTemp)
                        },
                    }).then(function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.countryTemp.UrlArchivo = dataImage;
                            $('#modalImageCountry').modal('hide');
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                showModalImageCountry: function (countryToModify, index) {
                    if ($scope.countriesInfo.add)
                        return;
                    $scope.countriesInfo.countryTemp = countryToModify;
                    $scope.countriesInfo.indexCurrentCountry = index;
                    $('#modalImageCountry').modal('toggle');
                },
                getCurrencies: function () {
                    service.get(controller + 'GetCurrencies', null, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.listCurrencies = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            };

            $scope.currenciesInfo = {
                list: [],
                currencyTemp: [],
                add: false,
                get: function () {
                    service.get(controller + 'GetCurrencies', null, function (res) {
                        if (res.data.Success) {
                            $scope.countriesInfo.listCurrencies = res.data.list;
                            $scope.currenciesInfo.list = res.data.list;
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                },
                update: function (currencyForUpdate) {
                    service.post(controller + 'SaveCurrency', currencyForUpdate, function (res) {
                        if (res.data.Success) {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                        }
                        else {
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                        }
                    })
                }
            };

            $scope.termsAndConditions = {
                list: [],
                get: function () {
                    service.get(controller + 'GetListTermsAndCondiions', null, function (res) {
                        if (res.data.Success)
                            $scope.termsAndConditions.list = res.data.list;
                        else
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                },
                update: function () {
                    service.post(controller + 'UpdateTermsAndCondiions', $scope.termsAndConditions.list, function (res) {
                        if (res.data.Success)
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                        else
                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                    })
                }
            };

            $scope.categoriesInfo.get();
            $scope.countriesInfo.get();
            $scope.currenciesInfo.get();
            $scope.termsAndConditions.get();
        }]
    )
    .directive('format', ['$filter', function ($filter) {
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                if (!ctrl) return;

                ctrl.$formatters.unshift(function (a) {
                    return $filter(attrs.format)(ctrl.$modelValue)
                });

                elem.bind('blur', function (event) {
                    var plainNumber = elem.val().replace(/[^\d|\-+|\.+]/g, '');
                    elem.val($filter(attrs.format)(plainNumber));
                });
            }
        };
    }]
    )
})();