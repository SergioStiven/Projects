(function () {
    'use strict';

    var dependencies = [
            "ngSanitize",
            "com.2fdevs.videogular",
            "com.2fdevs.videogular.plugins.controls",
            "com.2fdevs.videogular.plugins.overlayplay",
            "com.2fdevs.videogular.plugins.poster",
            "com.2fdevs.videogular.plugins.buffering",
            "rzModule",
            "oi.select",
            "ngFileUpload",
            "ui.bootstrap.datetimepicker",
            "uiCropper"
    ];

    angular.forEach(dependencies, function (dependency) {
        app.requires.push(dependency);
    });

    app.controller('ProfileController',
        ["$translate", "$sce", "$scope", '$filter', "service", "Upload", "$timeout", "$rootScope",
            function ($translate, $sce, $scope, $filter, service, Upload, $timeout, $rootScope) {

                var controller = 'Profile/';

                $scope.ConverToDate = function (date) {
                    return service.formatoFecha(date);
                }

                $scope.changeLanguage = function () {
                    $translate.use($scope.infoUser.user.Lenguage.Consecutivo.toString());
                    $scope.lists.genders = [{ Consecutivo: 1, Descripcion: $translate.instant('LBL_MALE') }, { Consecutivo: 2, Descripcion: $translate.instant('LBL_FEMALE') }];
                    $scope.lists.lenguages = [{ Consecutivo: 1, Descripcion: $translate.instant('LBL_SPANISH') }, { Consecutivo: 2, Descripcion: $translate.instant('LBL_ENGLISH') }, { Consecutivo: 3, Descripcion: $translate.instant('LBL_PORTUGUESE') }];
                    $scope.infoUser.user.Lenguage = $filter('filter')($scope.lists.lenguages, { Consecutivo: $scope.infoUser.user.Lenguage.Consecutivo })[0];
                    if ($scope.infoUser.isCandidate) {
                        var currentGender = $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero;
                        $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero = 0;
                        $timeout(function () {
                            $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero = currentGender;
                        }, 100);
                    }
                };

                $scope.getDescriptionProfile = function (ProfileId) {
                    switch (ProfileId) {
                        case 1:
                            return $translate.instant('LBL_ATHLETES');
                        case 2:
                            return $translate.instant('LBL_GROUPS');
                        case 3:
                            return $translate.instant('LBL_REPRESENTATIVE');
                        default:
                            return $translate.instant('LBL_ATHLETES');
                    }
                }
                $scope.videogularTheme = service.urlBase + 'Scripts/app/lib/bower_components/videogular-themes-default/videogular.css';
                $scope.loadingPage = true;
                $scope.tabBiographyActive = false;
                $scope.OperationIsAuthorizedByPlan = false;
                $scope.showBiographyTab = function (show) {
                    $scope.tabBiographyActive = show;
                }
                $rootScope.$on('onSearchNews', function (event, obj) {
                    if (!$scope.tabBiographyActive) return;
                    $scope.PostsCandidate.getBySearch(obj.filter.IdentificadorParaBuscar);
                });

                $rootScope.$on('GetMorePost', function (event, obj) {
                    if (!$scope.tabBiographyActive) return;
                    $scope.PostsCandidate.get();
                });

                $rootScope.$emit("viewSearchBar", { search: true });

                $scope.lists = {
                    categories: [],
                    countries: [],
                    genders: [{ Consecutivo: 1, Descripcion: $translate.instant('LBL_MALE') }, { Consecutivo: 2, Descripcion: $translate.instant('LBL_FEMALE') }],
                    lenguages: [{ Consecutivo: 1, Descripcion: $translate.instant('LBL_SPANISH') }, { Consecutivo: 2, Descripcion: $translate.instant('LBL_ENGLISH') }, { Consecutivo: 3, Descripcion: $translate.instant('LBL_PORTUGUESE') }],
                    getCategories: function () {
                        service.get('Administration/GetListCategories', null, function (res) {
                            if (res.data.Success) {
                                $scope.lists.categories = res.data.list;
                            }
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        })
                    },
                    getCountries: function () {
                        service.post('Administration/GetListCountries', null, function (res) {
                            if (res.data.Success) {
                                $scope.lists.countries = res.data.list;
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

                $scope.Categories = {
                    list: [],
                    listAvailable: [],
                    currentIndex: 0,
                    selected: 0,
                    category: { Consecutivo: 0 },
                    newCategory: false,
                    maximumCategoriesByPlan: 0,
                    myListCategories: [],
                    getMaximumCategoriesByPlan: function () {
                        service.get('Settings/GetMyPlan', null, function (res) {
                            if (res.data.Success)
                                if (res.data.obj.Planes != null && res.data.obj.Planes != undefined)
                                    $scope.Categories.maximumCategoriesByPlan = res.data.obj.Planes.NumeroCategoriasPermisibles;
                                else
                                    $scope.Categories.maximumCategoriesByPlan = res.data.obj.NumeroCategoriasPermisibles;
                        })
                    },
                    get: function () {
                        service.get('Administration/GetListCategories', null, function (res) {
                            if (res.data.Success) {
                                $scope.Categories.list = res.data.list;
                                $scope.Categories.listAvailable = res.data.list;
                                $scope.infoUser.get();
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    showList: false,
                    havePositionInField: false,
                    categoryToSave: {},
                    showModalCategory: function (newCategory, category) {
                        $scope.Categories.newCategory = newCategory;
                        if (!newCategory) {
                            if ($scope.infoUser.user.TipoPerfil === 1) {
                                $scope.Categories.category = category;
                                $scope.Categories.validateIfHavePositionInField($scope.Categories.category.CodigoCategoria);
                                if ($scope.infoUser.user.CandidatoDeLaPersona.Consecutivo === 0) {
                                    $scope.Abilities.list = category.HabilidadesCandidatos;
                                    $scope.Abilities.showList = true;
                                }
                                else
                                    $scope.Abilities.get({ Consecutivo: category.CodigoCategoria, CodigoCategoriaCandidato: category.Consecutivo });
                            }
                            else {
                                return;
                            }
                        } else {
                            if ($scope.Categories.listAvailable.length === 0) return;
                            $scope.Abilities.list = [];
                            $scope.Categories.selectedOption($scope.Categories.listAvailable[0], 0);
                            //$scope.Abilities.showList = false;
                        }
                        $('#modalInfoCategories').modal('toggle');
                    },
                    selectedOption: function (obj, indice) {
                        $scope.Categories.selected = indice;
                        this.showList = false;
                        this.currentIndex = indice;
                        $scope.Categories.category = obj;
                        $scope.Categories.category.CodigoCategoria = obj.Consecutivo;
                        if ($scope.infoUser.user.TipoPerfil === 1) { // Candidate
                            $scope.Abilities.get(obj);
                        }
                        $scope.Categories.validateIfHavePositionInField($scope.Categories.category.CodigoCategoria);
                    },
                    addcategory: function () {
                        if ($scope.Categories.newCategory) {
                            if (($scope.Categories.myListCategories.length + 1) > $scope.Categories.maximumCategoriesByPlan) {
                                service.showErrorMessage('Ya has alcanzado el máximo de deportes soportado por tu plan', service.getTypeMessage('error'));
                                return;
                            }
                            var newCategoryTemp = { Consecutivo: 0, DescripcionIdiomaBuscado: '', UrlArchivo: '', HabilidadesCandidatos: [] };
                            newCategoryTemp.Consecutivo = $scope.Categories.category.Consecutivo;
                            newCategoryTemp.DescripcionIdiomaBuscado = $scope.Categories.category.DescripcionIdiomaBuscado;
                            newCategoryTemp.UrlArchivo = $scope.Categories.category.UrlArchivo;
                            // Candidate
                            if ($scope.infoUser.user.TipoPerfil === 1) {
                                var positionInField = $scope.Categories.category.PosicionCampo;
                                $scope.Categories.category = { CodigoCategoria: 0, Categorias: {}, HabilidadesCandidatos: [] };
                                $scope.Categories.category.CodigoCategoria = newCategoryTemp.Consecutivo;
                                $scope.Categories.category.Categorias = newCategoryTemp;
                                $scope.Categories.category.HabilidadesCandidatos = $scope.Abilities.list;
                                $scope.Categories.category.PosicionCampo = positionInField;
                                // Add category when candidate exist
                                if ($scope.infoUser.user.CandidatoDeLaPersona !== null && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != null
                                    && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != undefined && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != 0) {
                                    $scope.Categories.category.CodigoCandidato = $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo;
                                    service.post(controller + 'CreateCandidateSkills', $scope.Categories.category, function (res) {
                                        if (res.data.Success) {
                                            $scope.Categories.category.Consecutivo = res.data.obj.ConsecutivoCreado;
                                        }
                                        else {
                                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                        }
                                    });
                                }
                            }
                            // Group & Agent
                            else if ($scope.infoUser.user.TipoPerfil === 2 || $scope.infoUser.user.TipoPerfil === 3) {
                                $scope.Categories.category = { CodigoCategoria: 0, Categorias: {} };
                                $scope.Categories.category.CodigoCategoria = newCategoryTemp.Consecutivo;
                                $scope.Categories.category.Categorias = newCategoryTemp;
                                // Add category when group exist
                                if ($scope.infoUser.user.GrupoDeLaPersona !== null) {
                                    $scope.Categories.category.CodigoGrupo = $scope.infoUser.user.GrupoDeLaPersona.Consecutivo;
                                    service.post(controller + 'CreateGroupSkills', $scope.Categories.category, function (res) {
                                        if (res.data.Success) {
                                            $scope.Categories.category.Consecutivo = res.data.obj.ConsecutivoCreado;
                                        }
                                        else {
                                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                        }
                                    });
                                }
                                // Add category when agent exist
                                else if ($scope.infoUser.user.RepresentanteDeLaPersona !== null) {
                                    $scope.Categories.category.CodigoRepresentante = $scope.infoUser.user.RepresentanteDeLaPersona.Consecutivo;
                                    service.post(controller + 'CreateAgentSkills', $scope.Categories.category, function (res) {
                                        if (res.data.Success) {
                                            $scope.Categories.category.Consecutivo = res.data.obj.ConsecutivoCreado;
                                        }
                                        else {
                                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                        }
                                    });
                                }

                            }
                        }
                        // Update category when candidate exist
                        else if ($scope.infoUser.user.CandidatoDeLaPersona !== null && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != null
                            && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != undefined && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo != 0) {
                            $scope.Categories.category.HabilidadesCandidatos = $scope.Abilities.list;
                            service.post(controller + 'CreateCandidateSkills', $scope.Categories.category, function (res) {
                                if (res.data.Success) {
                                    if ($scope.Categories.category.Consecutivo === 0 || $scope.Categories.category.Consecutivo === null) {
                                        $scope.Categories.category.Consecutivo = res.data.obj.ConsecutivoCreado;
                                    }
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                }
                            });
                            $('#modalInfoCategories').modal('hide');
                            return;
                        }
                        $('#modalInfoCategories').modal('hide');
                        if ($scope.Categories.newCategory) {
                            $scope.Categories.myListCategories.push(this.category);
                            $scope.Categories.listAvailable.splice($scope.Categories.currentIndex, 1);
                        }
                    },
                    remove: function (obj, indice) {
                        // Delete category when candidate exist
                        if ($scope.infoUser.user.CandidatoDeLaPersona !== null && $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo !== 0) {
                            service.post(controller + 'DeleteCandidateSkills', { Consecutivo: obj.Consecutivo }, function (res) {
                                if (!res.data.Success) service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            });
                        }
                        // Delete category when group exist
                        else if ($scope.infoUser.user.GrupoDeLaPersona !== null && $scope.infoUser.user.GrupoDeLaPersona.Consecutivo !== 0) {
                            service.post(controller + 'DeleteGroupSkills', { Consecutivo: obj.Consecutivo }, function (res) {
                                if (!res.data.Success) service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            });
                        }
                        // Delete category when agent exist
                        else if ($scope.infoUser.user.RepresentanteDeLaPersona !== null && $scope.infoUser.user.RepresentanteDeLaPersona.Consecutivo !== 0) {
                            service.post(controller + 'DeleteAgentSkills', { Consecutivo: obj.Consecutivo }, function (res) {
                                if (!res.data.Success) service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            });
                        }

                        $scope.Categories.myListCategories.splice(indice, 1);
                        $scope.Categories.listAvailable.push(obj.Categorias);
                    },
                    validateIfHavePositionInField: function (categoryId) {
                        if (categoryId === 3 || categoryId === 5 || categoryId === 6 || categoryId === 7) {
                            $scope.Categories.havePositionInField = true;
                        }
                        else {
                            $scope.Categories.havePositionInField = false;
                        }
                    }
                };

                $scope.Abilities = {
                    list: [],
                    isBusy: false,
                    showList: false,
                    currentSelectedStars: 0,
                    get: function (categorieToSearch) {
                        this.isBusy = true;
                        $scope.Abilities.list = [],
                            $scope.Abilities.currentSelectedStars = 0;
                        service.post('Administration/GetListAbilitiesByCategoryAndLenguage', categorieToSearch, function (res) {
                            if (res.data.Success) {
                                var abilitiesTmp = [];
                                for (var i = 0; i < res.data.list.length; i++) {
                                    var abilityTemp = { NumeroEstrellas: 0, Habilidades: [], CodigoHabilidad: 0 };
                                    abilityTemp.Habilidades = res.data.list[i];
                                    abilityTemp.CodigoHabilidad = res.data.list[i].Consecutivo;
                                    abilitiesTmp.push(abilityTemp);
                                }
                                $scope.Abilities.list = abilitiesTmp;
                                if (!$scope.Categories.newCategory) {
                                    $scope.Abilities.getByCandidate(categorieToSearch);
                                }

                                $scope.Abilities.currentSelectedStars = $scope.Abilities.getTotalNumberStarsByAbility();
                                $scope.Abilities.isBusy = false;
                                $scope.Abilities.showList = true;
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    getByCandidate: function (categoryToSearch) {
                        $scope.Abilities.isBusy = true;
                        $scope.Abilities.currentSelectedStars = 0;
                        service.post(controller + 'GetListAbilitiesByCandidate', { CodigoCategoriaCandidato: categoryToSearch.CodigoCategoriaCandidato }, function (res) {
                            if (res.data.Success) {
                                for (var i = 0; i < res.data.list.length; i++) {
                                    for (var j = 0; j < $scope.Abilities.list.length; j++) {
                                        if (res.data.list[i].CodigoHabilidad === $scope.Abilities.list[j].CodigoHabilidad) {
                                            $scope.Abilities.list[j] = res.data.list[i];
                                            break;
                                        }
                                    }
                                }
                                $scope.Abilities.currentSelectedStars = $scope.Abilities.getTotalNumberStarsByAbility();
                                $scope.Abilities.isBusy = false;
                                $scope.Abilities.showList = true;
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    updateNumberStars: function (ability, numberStars) {
                        var currentNumberStarsAbility = ability.NumeroEstrellas;
                        ability.NumeroEstrellas = numberStars;
                        var totalStars = $scope.Abilities.getTotalNumberStarsByAbility();
                        if (totalStars > 50) {
                            ability.NumeroEstrellas = currentNumberStarsAbility;
                        }
                        $scope.Abilities.currentSelectedStars = totalStars;
                    },
                    getTotalNumberStarsByAbility: function () {
                        var totalStars = 0;
                        for (var i = 0; i < $scope.Abilities.list.length; i++) {
                            totalStars += $scope.Abilities.list[i].NumeroEstrellas;
                        }
                        return totalStars;
                    }
                };

                $scope.infoUser = {
                    user: { Consecutivo: 0, CandidatoDeLaPersona: { CodigoGenero: 0, Consecutivo: 0 }, Lenguage: {}, Gender: {}, Usuarios: { Email: '' } },
                    defaultPlan: { CodigoTipoPerfil: 0 },
                    emailBefore: '',
                    archivosPerfil: service.urlImageProfileDefault,
                    archivosBanner: service.urlImageBannerDefault,
                    archivosPerfilTemp: {
                        data: null,
                        name: '',
                        isDefault: true
                    },
                    archivosBannerTemp: {
                        data: null,
                        name: '',
                        isDefault: true
                    },
                    isBusy: false,
                    isCandidate: false,
                    isGroup: false,
                    isAgent: false,
                    get: function () {
                        service.get(controller + 'GetInfoPersonLoggedIn', null, function (res) {
                            res.data.obj.Candidatos = [];
                            $scope.infoUser.user = res.data.obj;
                            $translate.use($scope.infoUser.user.CodigoIdioma.toString());
                            if (res.data.Success) {
                                $scope.infoUser.emailBefore = $scope.infoUser.user.Usuarios.Email;
                                $('#btnUpdateImageBanner').show();
                                $scope.PostsCandidate.validateOperationByPlan();
                                $scope.infoUser.fillUserPropertiesByProfile();
                            } else {
                                $scope.infoUser.user.Nombres = '';
                                $scope.infoUser.user.Apellidos = '';
                                if ($scope.infoUser.user.TipoPerfil === 1) {
                                    $scope.infoUser.clearCandidateInformation();
                                    $scope.infoUser.isCandidate = true;
                                    $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero = 1;
                                }
                                else if ($scope.infoUser.user.TipoPerfil === 2) $scope.infoUser.isGroup = true;
                                else if ($scope.infoUser.user.TipoPerfil === 3) $scope.infoUser.isAgent = true;
                                $('#btnUpdateImageBanner').show();
                                service.showErrorMessage($translate.instant('NOTI_REGISTRATION_INCOMPLETE'), service.getTypeMessage('warning'));
                            }
                            $scope.infoUser.user.Lenguage = $filter('filter')($scope.lists.lenguages, { Consecutivo: $scope.infoUser.user.IdiomaDeLaPersona })[0];
                            $scope.changeLanguage();
                            $scope.loadingPage = false;
                            $scope.infoUser.setImageBanner();
                            $scope.PostsCandidate.get();
                        })
                    },
                    save: function () {
                        if (!$scope.formPersonalInfo.$valid) {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                            return;
                        }
                        if (!$scope.infoUser.validateForm()) return;

                        $scope.infoUser.isBusy = true;
                        $scope.infoUser.addCategoriesToSave();
                        $scope.infoUser.user.CodigoPais = $scope.infoUser.user.Paises.Consecutivo;
                        $scope.infoUser.user.CodigoIdioma = $scope.infoUser.user.Lenguage.Consecutivo;

                        $scope.infoUser.cleanObjectToSave();
                        var saveImage = $scope.infoUser.user.Consecutivo === 0 ? true : false;
                        service.post(controller + 'UpdatePerson', $scope.infoUser.user, function (res) {
                            if (res.data.Success) {
                                if ($scope.infoUser.user.Usuarios.Consecutivo != null && $scope.infoUser.user.Usuarios.Consecutivo > 0) {
                                    service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                                } else {
                                    $scope.infoUser.user.Usuarios.Consecutivo = res.data.obj.ConsecutivoUsuarioCreado;
                                    if (saveImage) {
                                        if (!$scope.infoUser.archivosPerfilTemp.isDefault) {
                                            $scope.infoUser.isBusy = false;
                                            $scope.infoUser.updateImage($scope.infoUser.archivosPerfilTemp.data, $scope.infoUser.archivosPerfilTemp.name);
                                        }
                                        if (!$scope.infoUser.archivosBannerTemp.isDefault) {
                                            $scope.infoUser.isBusy = false;
                                            $scope.infoUser.updateImageBanner($scope.infoUser.archivosBannerTemp.data, $scope.infoUser.archivosBannerTemp.name);  
                                        }
                                    }
                                    $('#alertConfirmationRegistration').modal('toggle');
                                }
                            }
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            $scope.infoUser.isBusy = false;
                        });
                    },
                    sliderHeight: {
                        options: {
                            floor: 120,
                            ceil: 220,
                            showSelectionBar: true,
                            vertical: true
                        }
                    },
                    sliderWeight: {
                        options: {
                            floor: 40,
                            ceil: 120,
                            showSelectionBar: true
                        }
                    },
                    fillUserPropertiesByProfile: function () {
                        if ($scope.infoUser.user.TipoPerfil === 1) { // Candidate
                            $scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento = service.formatoFecha($scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento);
                            $scope.infoUser.user.Gender = $filter('filter')($scope.lists.genders, { Consecutivo: $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero })[0];
                            $scope.Categories.myListCategories = $scope.infoUser.user.CandidatoDeLaPersona.CategoriasCandidatos;
                            $scope.Categories.listAvailable = $scope.infoUser.filteredCategories($scope.infoUser.user.CandidatoDeLaPersona.CategoriasCandidatos);
                            $scope.Tutor.validateDateForShowButton()
                            if ($scope.infoUser.user.Consecutivo !== -1 && $scope.infoUser.user.Consecutivo !== -2) $scope.infoUser.isCandidate = true;
                        } else if ($scope.infoUser.user.TipoPerfil === 2) { // Group
                            $scope.Categories.myListCategories = $scope.infoUser.user.GrupoDeLaPersona.CategoriasGrupos;
                            $scope.Categories.listAvailable = $scope.infoUser.filteredCategories($scope.infoUser.user.GrupoDeLaPersona.CategoriasGrupos);
                            $scope.infoUser.user.Apellidos = '';
                            if ($scope.infoUser.user.Consecutivo !== -1 && $scope.infoUser.user.Consecutivo !== -2) $scope.infoUser.isGroup = true;
                        } else if ($scope.infoUser.user.TipoPerfil === 3) { // Agent
                            $scope.Categories.myListCategories = $scope.infoUser.user.RepresentanteDeLaPersona.CategoriasRepresentantes;
                            $scope.Categories.listAvailable = $scope.infoUser.filteredCategories($scope.infoUser.user.RepresentanteDeLaPersona.CategoriasRepresentantes);
                            if ($scope.infoUser.user.Consecutivo !== -1 && $scope.infoUser.user.Consecutivo !== -2) $scope.infoUser.isAgent = true;
                        }
                    },
                    filteredCategories: function (toFilter) {
                        var idsCategories = toFilter.map(function (a) { return a.CodigoCategoria; });
                        return $scope.Categories.list.filter(function (category) { return idsCategories.indexOf(category.Consecutivo) === -1 });
                    },
                    uploadImageTemp: function (dataImage, name) {
                        $scope.infoUser.archivosPerfilTemp.data = dataImage;
                        $scope.infoUser.archivosPerfilTemp.name = name;
                        $scope.infoUser.archivosPerfilTemp.isDefault = false;
                        $scope.infoUser.user.UrlImagenPerfil = dataImage;
                        $rootScope.$emit("changeImageProfile", { image: dataImage });
                        $('#modalImageProfile').modal('hide');
                    },
                    uploadImageBannerTemp: function (dataImage, name) {
                        $scope.infoUser.archivosBannerTemp.data = dataImage;
                        $scope.infoUser.archivosBannerTemp.name = name;
                        $scope.infoUser.archivosBannerTemp.isDefault = false;
                        $scope.infoUser.user.UrlImagenBanner = dataImage;
                        $scope.infoUser.setImageBanner();
                        $rootScope.$emit("changeImageBanner", { image: dataImage });
                        $('#modalImageBanner').modal('hide');
                    },
                    updateImage: function (dataImage, nameImage) {
                        if ($scope.infoUser.isBusy) return;
                        $scope.infoUser.isBusy = true;
                        if ($scope.infoUser.user.Usuarios.Consecutivo === 0) {
                            $scope.infoUser.uploadImageTemp(dataImage, nameImage);
                            $scope.infoUser.isBusy = false;
                            return;
                        }
                        $scope.infoUser.isBusy = true;
                        Upload.upload({
                            url: service.urlBase + controller + 'uploadImageProfile',
                            data: {
                                file: Upload.dataUrltoBlob(dataImage, nameImage)
                            },
                        }).then(function (res) {
                            $scope.infoUser.isBusy = false;
                            $('#modalImageProfile').modal('hide');
                            if (res.data.Success) {
                                if ($scope.infoUser.user.UrlImagenPerfil === '')
                                    $scope.infoUser.archivosPerfil = dataImage;
                                else
                                    $scope.infoUser.user.UrlImagenPerfil = dataImage;
                                    
                                $rootScope.$emit("changeImageProfile", { image: dataImage });
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    updateImageBanner: function (dataImage, nameImage) {
                        if ($scope.infoUser.isBusy) return;
                        $scope.infoUser.isBusy = true;
                        if ($scope.infoUser.user.Usuarios.Consecutivo === 0) {
                            $scope.infoUser.uploadImageBannerTemp(dataImage, nameImage);
                            $scope.infoUser.isBusy = false;
                            return;
                        }
                        $scope.infoUser.isBusy = true;
                        Upload.upload({
                            url: service.urlBase + controller + 'uploadImageBanner',
                            data: {
                                file: Upload.dataUrltoBlob(dataImage, nameImage),
                                Person: JSON.stringify({ Consecutivo: $scope.infoUser.user.Consecutivo })
                            },
                        }).then(function (res) {
                            if (res.data.Success) {
                                if ($scope.infoUser.user.UrlImagenBanner === '')
                                    $scope.infoUser.archivosBanner = dataImage;
                                else {
                                    $scope.infoUser.user.UrlImagenBanner = dataImage;
                                    $scope.infoUser.setImageBanner();
                                }

                                $scope.infoUser.isBusy = false;
                                $rootScope.$emit("changeImageBanner", { image: dataImage });
                                $('#modalImageBanner').modal('hide');
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    setImageBanner: function () {
                        if ($scope.infoUser.user.UrlImagenBanner != '')
                            $('#divBanner').css("background-image", "url(" + $scope.infoUser.user.UrlImagenBanner + ")");
                        else
                            $('#divBanner').css("background-image", "url(" + $scope.infoUser.archivosBanner + ")");
                    },
                    addCategoriesToSave: function () {
                        switch ($scope.infoUser.user.TipoPerfil) {
                            case 1: // Candidate
                                $scope.infoUser.user.CandidatoDeLaPersona.CategoriasCandidatos = $scope.Categories.myListCategories;
                                break;
                            case 2: // Group
                                $scope.infoUser.user.GrupoDeLaPersona.CategoriasGrupos = $scope.Categories.myListCategories;
                                break;
                            case 3: // Agent
                                $scope.infoUser.user.RepresentanteDeLaPersona.CategoriasRepresentantes = $scope.Categories.myListCategories;
                                break;
                            default:
                                return;
                        }
                    },
                    validateForm: function () {
                        if ($scope.Categories.myListCategories.length === 0) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                            return false;
                        }
                        else if (!$scope.infoUser.validateEmail()) {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                            return false;
                        }
                        else if ($scope.infoUser.user.Paises.Consecutivo === 0) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_COUNTRIES'), service.getTypeMessage('error'));
                            return false;
                        }
                        if ($scope.infoUser.user.TipoPerfil === 1) {
                            var d = new Date();
                            if ($scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento > d) {
                                service.showErrorMessage($translate.instant('NOTI_INVALID_BIRTHDAY'), service.getTypeMessage('error'));
                                return false;
                            }
                            var year = d.getFullYear();
                            var month = d.getMonth();
                            var day = d.getDate();
                            var minDate = new Date(year - 18, month, day)
                            if ($scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento > minDate && !$scope.infoUser.validateFormTutor()) {
                                service.showErrorMessage($translate.instant('NOTI_VALIDATION_TUTOR'), service.getTypeMessage('error'));
                                return false;
                            }
                        }
                        return true;

                    },
                    validateFormTutor: function () {
                        if ($scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Nombres === ''
                            || $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Apellidos === ''
                            || $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Email === ''
                            || $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.TelefonoMovil === ''
                            || $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.TelefonoFijo === ''
                            || $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Skype === '') {
                            return false;
                        }
                        return true;
                    },
                    cleanObjectToSave: function () {
                        $scope.infoUser.user.Grupos = [];
                        $scope.infoUser.user.Representantes = [];
                        $scope.infoUser.user.Candidatos = [];
                        $scope.infoUser.user.Anunciantes = [];

                    },
                    clearCandidateInformation: function () {
                        $scope.infoUser.user.CandidatoDeLaPersona = {};
                        $scope.infoUser.user.CandidatoDeLaPersona.Estatura = 120;
                        $scope.infoUser.user.CandidatoDeLaPersona.Peso = 40;
                        $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo = 0;
                        $scope.infoUser.user.CandidatoDeLaPersona.CodigoGenero = 0;
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables = {};
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Nombres = '';
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Apellidos = '';
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Email === '';
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.TelefonoMovil = '';
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.TelefonoFijo = '';
                        $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Skype === '';
                    },
                    validateIfEmailExist: function () {
                        if ($scope.infoUser.emailBefore === $scope.infoUser.user.Usuarios.Email) {
                            $scope.infoUser.save();
                        } else {
                            service.post('Authenticate/ValidateIfEmailExist', { Email: $scope.infoUser.user.Usuarios.Email }, function (res) {
                                if (!res.data.Success) {
                                    service.showErrorMessage($translate.instant('NOTI_VALIDATION_MAIL_REGISTERED'), service.getTypeMessage('error'));
                                    return;
                                }
                                $scope.infoUser.save();
                            })
                        }

                    },
                    validateEmail: function () {
                        if ($scope.infoUser.user.Usuarios.Email === '') return true;
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test($scope.infoUser.user.Usuarios.Email);
                    }
                };

                $scope.Contacts = {
                    isBusy: false,
                    filter: { SkipIndexBase: 0, TakeIndexBase: 10000 },
                    list: [],
                    get: function () {
                        service.post(controller + 'GetMyContacts', $scope.Contacts.filter, function (res) {
                            if (res.data.Success) {
                                $scope.Contacts.list = res.data.list;
                            }
                            $scope.Contacts.isBusy = false;
                        });
                    },
                    save: function () {
                        if ($scope.Contacts.isBusy) return;
                        if ($scope.infoUser.user.YaEstaAgregadaContactos) {
                            $scope.Contacts.delete();
                            return;
                        }
                        $scope.Contacts.isBusy = true;
                        service.post(controller + 'CreateContact', null, function (res) {
                            if (res.data.Success) {
                                service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                                $scope.infoUser.user.ConsecutivoContacto = res.data.obj.ConsecutivoCreado;
                                $scope.infoUser.user.YaEstaAgregadaContactos = true;
                            }
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));

                            $scope.Contacts.isBusy = false;
                        });
                    },
                    delete: function () {
                        $scope.Contacts.isBusy = true;
                        service.post(controller + 'DeleteContact', { Consecutivo: $scope.infoUser.user.ConsecutivoContacto }, function (res) {
                            if (res.data.Success) {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                                $scope.infoUser.user.ConsecutivoContacto = 0;
                                $scope.infoUser.user.YaEstaAgregadaContactos = false;
                            }
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));

                            $scope.Contacts.isBusy = false;
                        });
                    },
                    deleteContact: function (contact, indice) {
                        $scope.Contacts.list.splice(indice, 1);
                        service.post(controller + 'DeleteContact', { Consecutivo: contact.Consecutivo }, function (res) {
                            if (res.data.Success) {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                            }
                            else
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));

                            $scope.Contacts.isBusy = false;
                        });
                    },
                    viewProfile: function (contact) {
                        service.post('Search/SaveSearchIdInSession', { Consecutivo: contact.PersonasContacto.Consecutivo }, function (res) {
                            service.showErrorMessage($translate.instant('NOTI_CANNOT_SEE_PERSON'), service.getTypeMessage('success'));
                        });
                    }
                }

                $scope.Tutor = {
                    validateDateForShowButton: function () {
                        if (!$scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento)
                            return;
                        var d = new Date();
                        if ($scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento > d) {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_BIRTHDAY'), service.getTypeMessage('error'));
                            return;
                        }
                        var year = d.getFullYear();
                        var month = d.getMonth();
                        var day = d.getDate();
                        var minDate = new Date(year - 18, month, day);
                        if ($scope.infoUser.user.CandidatoDeLaPersona.FechaNacimiento > minDate) {
                            //service.showErrorMessage('Debes ingresar los datos del tutor', service.getTypeMessage('warning'));
                            $scope.Tutor.showButton = true;
                        }
                        else {
                            $scope.Tutor.showButton = false;
                        }
                    },
                    showButton: false,
                    toSaveTemp: {},
                    save: function () {
                        if (!$scope.Tutor.validateEmail()) {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_EMAIL'), service.getTypeMessage('error'));
                            return;
                        }
                        $('#modalInfoTutor').modal('hide');
                        if ($scope.infoUser.user.Consecutivo !== undefined && $scope.infoUser.user.Consecutivo !== 0) {
                            $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.CodigoCandidato = $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo;
                            service.post(controller + 'CreateTutorCandidate', $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables, function (res) {
                                if (res.data.Success)
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                                else
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            });
                        }
                        else {
                            $scope.Tutor.toSaveTemp = $scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables;
                        }

                    },
                    validateEmail: function () {
                        if ($scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Email === '') return true;
                        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                        return re.test($scope.infoUser.user.CandidatoDeLaPersona.CandidatosResponsables.Email);
                    }
                }

                $scope.PostsCandidate = {
                    filter: { SkipIndexBase: -2, TakeIndexBase: 2 },
                    list: [],
                    post: { CodigoArchivo: 0, CodigoCandidato: 0, Titulo: '', Descripcion: '', CategoriasEventos: [], Pais: { Consecutivo: 0 } },
                    isBusy: false,
                    endList: false,
                    toDelete: {},
                    validateOperationByPlan: function () {
                        service.get(controller + 'validateOperationByPlan', null, function (res) {
                            $scope.OperationIsAuthorizedByPlan = res.data.Success;
                        })
                    },
                    uploadFile: function () {
                        if (!$scope.PostsCandidate.validate()) return;
                        if ($scope.infoUser.user.TipoPerfil === 2) {
                            if ($scope.PostsCandidate.post.Descripcion === '') {
                                service.showErrorMessage($translate.instant('NOTI_VALIDATION_DESCRIPTION'), service.getTypeMessage('error'));
                                return;
                            }
                            if (categories.length === 0) {
                                service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                                return;
                            }
                        } else if (!$scope.OperationIsAuthorizedByPlan) {
                            service.showErrorMessage($translate.instant('NOTI_OPTION_NOT_AVAILABLE'), service.getTypeMessage('error'));
                            return;
                        }
                        $scope.PostsCandidate.isBusy = true;
                        Upload.upload({
                            url: service.urlBase + controller + 'UploadVideoToControlDuration',
                            data: { file: $scope.picFilePost }
                        }).then(function (res) {
                            if (res.data.Success) {
                                $scope.PostsCandidate.post.CodigoArchivo = res.data.obj.ConsecutivoCreado;
                                $scope.PostsCandidate.createPost();
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }

                            $scope.PostsCandidate.isBusy = false;

                        }, function (res) {
                            if (res.status > 0)
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                            $scope.PostsCandidate.isBusy = false;
                        }, function (evt) {
                            $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                        });
                    },
                    updateFile: function (post, picFilePost) {
                        if ($scope.PostsCandidate.isBusy) return;
                        if ($scope.infoUser.user.TipoPerfil === 2) {
                            if ($scope.PostsCandidate.post.CategoriasEventos.length === 0) {
                                service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                                return;
                            }
                        }
                        $scope.PostsCandidate.isBusy = true;
                        if (!picFilePost) {
                            $scope.PostsCandidate.post = post;
                            $scope.PostsCandidate.createPost();
                            return;
                        }
                        Upload.upload({
                            url: service.urlBase + controller + 'UpdateVideoCandidate',
                            data: { file: picFilePost, ConsecutivoArchivo: post.CodigoArchivo, CodigoCandidatoVideo: post.Consecutivo }
                        }).then(function (res) {
                            if (res.data.Success) {
                                $scope.PostsCandidate.post = post;
                                $scope.PostsCandidate.post.CodigoArchivo = post.CodigoArchivo;
                                $scope.PostsCandidate.createPost();
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }

                        }), function (res) {
                            if (res.status > 0)
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }, function (evt) {
                            $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                        };
                    },
                    updateEventFile: function (post, picFilePost) {
                        if ($scope.PostsCandidate.isBusy) return;
                        if (post.CategoriasEventos.length === 0) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_SPORTS'), service.getTypeMessage('error'));
                            return;
                        }
                        for (var i = 0; i < post.CategoriasEventos.length; i++) {
                            var categoryEventId = post.CategoriasEventos[i];
                            post.CategoriasEventos[i] = { CodigoCategoria: categoryEventId, CodigoEvento: post.Consecutivo };
                        }
                        $scope.PostsCandidate.isBusy = true;
                        if (!picFilePost) {
                            $scope.PostsCandidate.post = post;
                            $scope.PostsCandidate.createPost();
                            return;
                        }
                        Upload.upload({
                            url: service.urlBase + 'Events/UpdateEventFile',
                            data: {
                                file: picFilePost,
                                eventToUpdate: JSON.stringify(post)
                            }
                        }).then(function (res) {
                            if (res.data.Success) {
                                $scope.PostsCandidate.post = post;
                                $scope.PostsCandidate.post.CodigoArchivo = res.data.obj.ConsecutivoArchivoCreado;
                                $scope.PostsCandidate.createPost();
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                $scope.PostsCandidate.isBusy = false;
                            }

                        }), function (res) {
                            if (res.status > 0)
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                        }, function (evt) {
                            $scope.picFilePost.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
                        };
                    },
                    createPost: function () {
                        if ($scope.infoUser.user.TipoPerfil === 1) {
                            if ($scope.infoUser.user.CandidatoDeLaPersona.Consecutivo == -1 || $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo == -2) return;
                            $scope.PostsCandidate.post.CodigoCandidato = $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo;
                            service.post(controller + 'CreatePostsCandidate', $scope.PostsCandidate.post, function (res) {
                                if (res.data.Success) {
                                    $scope.PostsCandidate.clean();
                                    $scope.PostsCandidate.get();
                                    service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                                } else if (!res.data.AuthorizedByPlan) {
                                    service.showErrorMessage($translate.instant('NOTI_OPTION_NOT_AVAILABLE'), service.getTypeMessage('error'));
                                } else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                }
                                $scope.PostsCandidate.isBusy = false;
                            })
                        }
                        else if ($scope.infoUser.user.TipoPerfil === 2) {
                            $scope.PostsCandidate.post.CodigoGrupo = $scope.infoUser.user.GrupoDeLaPersona.Consecutivo;
                            $scope.PostsCandidate.post.CodigoIdioma = service.getLenguageFromNavigator();
                            $scope.PostsCandidate.post.FechaInicio = $filter('date')($scope.PostsCandidate.post.FechaInicio, 'yyyy-MM-dd HH:mm:ss');
                            $scope.PostsCandidate.post.FechaTerminacion = $filter('date')($scope.PostsCandidate.post.FechaTerminacion, 'yyyy-MM-dd HH:mm:ss');
                            service.post(controller + 'CreatePostsGroup', $scope.PostsCandidate.post, function (res) {
                                if (res.data.Success) {
                                    $scope.PostsCandidate.clean();
                                    service.showErrorMessage($translate.instant('NOTI_SAVE_SUCCESS'), service.getTypeMessage('success'));
                                    $scope.PostsCandidate.isBusy = false;
                                    $scope.PostsCandidate.filter.SkipIndexBase = -2;
                                    $scope.PostsCandidate.filter.TakeIndexBase = 2;
                                    $scope.PostsCandidate.get();
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'));
                                    $scope.PostsCandidate.isBusy = false;
                                }
                            })
                        }
                    },
                    deletePost: function (obj) {
                        if ($scope.infoUser.user.TipoPerfil === 1) {
                            service.post(controller + 'DeletePostsCandidate', { Consecutivo: obj.Consecutivo, CodigoArchivo: obj.CodigoArchivo }, function (res) {
                                if (res.data.Success) {
                                    $scope.PostsCandidate.filter.SkipIndexBase = -2;
                                    $scope.PostsCandidate.filter.TakeIndexBase = 2;
                                    $scope.PostsCandidate.list = [];
                                    $scope.PostsCandidate.get();
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                }
                            })
                        }
                        else if ($scope.infoUser.user.TipoPerfil === 2) {
                            service.post('Events/DeletetEvent', { Consecutivo: obj.Consecutivo }, function (res) {
                                if (res.data.Success) {
                                    $scope.PostsCandidate.filter.SkipIndexBase = -2;
                                    $scope.PostsCandidate.filter.TakeIndexBase = 2;
                                    $scope.PostsCandidate.list = [];
                                    $scope.PostsCandidate.get();
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('success'))
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                }
                            })
                        }

                    },
                    get: function () {
                        if ($scope.infoUser.user.Consecutivo === null || $scope.infoUser.user.Consecutivo === undefined) {
                            service.showErrorMessage($translate.instant('NOTI_REGISTRATION_INCOMPLETE'), service.getTypeMessage('error'));
                            return;
                        }
                        if ($scope.PostsCandidate.isBusyListPost || $scope.PostsCandidate.endList) return;
                        $scope.PostsCandidate.isBusyListPost = true;
                        $scope.PostsCandidate.filter.SkipIndexBase += 2;
                        $scope.PostsCandidate.filter.ZonaHorariaGMTBase = service.getTimeZone();
                        if ($scope.infoUser.user.TipoPerfil === 1) {
                            $scope.PostsCandidate.getPostCandidate();
                        }
                        else if ($scope.infoUser.user.TipoPerfil === 2) {
                            $scope.PostsCandidate.getPostGroup();
                        } else return;

                    },
                    getPostCandidate: function () {
                        $scope.PostsCandidate.filter.CodigoCandidato = $scope.infoUser.user.CandidatoDeLaPersona.Consecutivo;
                        service.post(controller + 'GetListPostsByCandidate', $scope.PostsCandidate.filter, function (res) {
                            if (res.data.Success) {
                                if (res.data.list.length < 2) {
                                    $scope.PostsCandidate.endList = true;
                                }
                                $scope.PostsCandidate.list = $scope.PostsCandidate.list.concat(res.data.list);
                            }
                            $scope.PostsCandidate.isBusyListPost = false;
                        })
                    },
                    getPostGroup: function () {
                        $scope.PostsCandidate.filter.ConsecutivoPerfil = $scope.infoUser.user.GrupoDeLaPersona.Consecutivo;
                        $scope.PostsCandidate.filter.ConsecutivoPersona = $scope.infoUser.user.Consecutivo;
                        service.post(controller + 'GetListPostsByGroup', $scope.PostsCandidate.filter, function (res) {
                            if (res.data.Success) {
                                if (res.data.list.length < 2) {
                                    $scope.PostsCandidate.endList = true;
                                    return;
                                }
                                for (var i = 0; i < res.data.list.length; i++) {
                                    res.data.list[i].FechaInicio = service.formatoFecha(res.data.list[i].FechaInicio);
                                    res.data.list[i].FechaTerminacion = service.formatoFecha(res.data.list[i].FechaTerminacion);
                                    res.data.list[i].CategoriasEventos = res.data.list[i].CategoriasEventos.map(function (a) { return a.CodigoCategoria; });
                                }
                                $scope.PostsCandidate.list = $scope.PostsCandidate.list.concat(res.data.list);
                            }
                            $scope.PostsCandidate.isBusyListPost = false;
                        })
                    },
                    clean: function () {
                        $scope.picFilePost = undefined;
                        $scope.PostsCandidate.post.CodigoArchivo = 0;
                        $scope.PostsCandidate.post.CodigoCandidato = 0;
                        $scope.PostsCandidate.post.Titulo = '';
                        $scope.PostsCandidate.post.Ubicacion = '';
                        $scope.PostsCandidate.post.Descripcion = '';
                        $scope.PostsCandidate.post.CategoriasEventos = [];
                        $scope.PostsCandidate.filter = { SkipIndexBase: -2, TakeIndexBase: 2 };
                        $scope.PostsCandidate.endList = false;
                        $scope.PostsCandidate.list = [];
                    },
                    getBySearch: function (name) {
                        $scope.PostsCandidate.filter.SkipIndexBase = -2;
                        $scope.PostsCandidate.filter.TakeIndexBase = 2;
                        $scope.PostsCandidate.filter.IdentificadorParaBuscar = name;
                        $scope.PostsCandidate.list = [];
                        $scope.PostsCandidate.isBusyListPost = false;
                        $scope.PostsCandidate.endList = false;
                        $scope.PostsCandidate.get();
                    },
                    validateTypeFile: function (file) {
                        if (!file) return;
                        if (!file.type.includes('video')) {
                            $scope.picFilePost = !$scope.picFilePost;
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_VIDEO'), service.getTypeMessage('error'));
                        } else if (file.size > 104857600) {
                            $scope.picFilePost = !$scope.picFilePost;
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_SIZE_VIDEO'), service.getTypeMessage('error'));
                        }
                    },
                    validate: function () {
                        if ($scope.PostsCandidate.post.Titulo.trim() === '') {
                            service.showErrorMessage($translate.instant('NOTI_INVALID_FORM'), service.getTypeMessage('error'));
                            return false;
                        } else if (!$scope.picFilePost) {
                            service.showErrorMessage($translate.instant('NOTI_VALIDATION_VIDEO'), service.getTypeMessage('error'));
                            return false;
                        }
                        return true;
                    }
                };

                $scope.selectedVideo = function (obj) {
                    $scope.urlVideoSelected = obj;
                }

                $scope.refreshSlider = function () {
                    $timeout(function () {
                        $scope.$broadcast('rzSliderForceRender');
                    });
                }

                // Load data
                $scope.Categories.get();
                $scope.Categories.getMaximumCategoriesByPlan();
                $scope.lists.getCategories();
                $scope.Contacts.get();
            }]
    );

    app
        .directive('newsList', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Profile/Posts'
            }
        }])
        .directive('generalInformation', ['service', function (service) {
            return {
                restrict: 'E',
                compile: function (tElem, tAttrs) {
                    return {
                        pre: function (scope, iElem, iAttrs) {
                            scope.lists.getCountries();
                        }
                    }
                },
                templateUrl: service.urlBase + 'Profile/GeneralInformation'
            }
        }])
        .directive('infoAgent', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Agent/InfoAgent'
            }
        }])
        .directive('informationByProfile', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Profile/InformationByProfile'
            }
        }])
        .directive('infoCandidate', ['service', '$timeout', function (service, $timeout) {
            return {
                restrict: 'E',
                compile: function () {
                    return {
                        post: function (scope, iElem, iAttrs) {
                            scope.refreshSlider = function () {
                                $timeout(function () {
                                    scope.$broadcast('rzSliderForceRender');
                                }, 3000);
                            }
                            scope.refreshSlider();
                        }
                    }
                },
                templateUrl: service.urlBase + 'Candidate/InfoCandidate'
            }
        }])
        .directive('infoGroup', ['service', function (service) {
            return {
                restrict: 'E',
                compile: function (tElem, tAttrs) {
                    return {
                        pre: function (scope, iElem, iAttrs) {

                            /* Bindable functions
                                    -----------------------------------------------*/
                            scope.endDateBeforeRender = endDateBeforeRender;
                            scope.endDateOnSetTime = endDateOnSetTime;
                            scope.startDateBeforeRender = startDateBeforeRender;
                            scope.startDateOnSetTime = startDateOnSetTime;

                            function startDateOnSetTime() {
                                scope.$broadcast('start-date-changed');
                            };

                            function endDateOnSetTime() {
                                scope.$broadcast('end-date-changed');
                            };

                            function startDateBeforeRender($dates) {
                                if (scope.dateRangeEnd) {
                                    var activeDate = moment(scope.dateRangeEnd);

                                    $dates.filter(function (date) {
                                        return date.localDateValue() >= activeDate.valueOf()
                                    }).forEach(function (date) {
                                        date.selectable = false;
                                    })
                                }
                            };

                            function endDateBeforeRender($view, $dates) {
                                if (scope.dateRangeStart) {
                                    var activeDate = moment(scope.dateRangeStart).subtract(1, $view).add(1, 'minute');

                                    $dates.filter(function (date) {
                                        return date.localDateValue() <= activeDate.valueOf()
                                    }).forEach(function (date) {
                                        date.selectable = false;
                                    })
                                }
                            };
                        }
                    }
                },
                templateUrl: service.urlBase + 'Groups/InfoGroup'
            }
        }])
        .directive('soccerCourtCandidate', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Candidate/SoccerCourtCandidate'
            }
        }])
        .directive('basketballCourtCandidate', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Candidate/BasketballCourtCandidate'
            }
        }])
        .directive('baseballCourtCandidate', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Candidate/BaseballCourtCandidate'
            }
        }])
        .directive('volleyballCourtCandidate', ['service', function (service) {
            return {
                restrict: 'E',
                templateUrl: service.urlBase + 'Candidate/VolleyballCourtCandidate'
            }
            }])
        .directive("scroll", function ($window, $rootScope) {
            return function (scope, element, attrs) {
                angular.element($window).bind("scroll", function () {
                    if ($(window).scrollTop() + $(window).height() > $(document).height() - 500) {
                        $rootScope.$emit("GetMorePost", {});
                    }
                    scope.$apply();
                })
            }
        });
    
})();