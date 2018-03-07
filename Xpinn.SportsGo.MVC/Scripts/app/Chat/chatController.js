(function () {
    'use strict';

    app
        .controller('ChatController',
        ['$translate','$filter', "$sce", "$scope", "service", "$rootScope", "$timeout",
            function ($translate, $filter, $sce, $scope, service, $rootScope, $timeout) {

                var controller = 'Chat/';
                // Notifications
                $scope.$parent.$on('onGetMessage', function (e, chatReceived) {
                    if (chatReceived.CodigoPersonaOwner !== $scope.Chat.active.personId && chatReceived.CodigoPersonaOwner !== $scope.Chat.myInfo.Consecutivo) return;

                    var contentMsg = angular.element(document.querySelector('.chat-msgs'));
                    if (chatReceived.CodigoPersonaOwner !== $scope.Chat.myInfo.Consecutivo) {
                        contentMsg.append($scope.Chat.printMessage(true, chatReceived.UltimoMensaje.Mensaje));
                        $scope.Chat.scrollBottom();
                        $scope.$apply();
                    }
                    else if (!$filter('filter')($scope.Chat.messageList, { Consecutivo: chatReceived.UltimoMensaje.Consecutivo })[0]) {
                        contentMsg.append($scope.Chat.printMessage(false, chatReceived.UltimoMensaje.Mensaje));
                        $scope.Chat.scrollBottom();
                        $scope.$apply();
                    }

                    if (!$scope.Chat.viewContacts) {
                        $scope.Chat.getChats();
                    }

                });
                $rootScope.$on('GetMoreContacts', function (event, obj) {
                    $scope.Chat.getMoreContacts();
                });
                $scope.ConverToDate = function (date) {
                    return service.formatoFecha(date);
                }

                $scope.Chat = {
                    selectedChat: false,
                    viewContacts: false,
                    showButtonMoreMessages: false,
                    viewContacts: false,
                    isBusy: false,
                    endList: false,
                    rangeMessagesToTake: 50,
                    UrlImagePerfilDefault: '../Content/assets/img/demo/avatar.png',
                    myInfo: {},
                    activeChat: { CodigoChatEnvia: 0, CodigoChatRecibe: 0, Mensaje: '', SkipIndexBase: 0, TakeIndexBase: 0 },
                    active: { name: '', UrlImageProfile: '', toUserId: '', msg: '', conected: false, personId: 0 },
                    myLastMessage: {},
                    contacts: [],
                    messageList: [],
                    filter: { SkipIndexBase: -5, TakeIndexBase: 5 },
                    printMessage: function (isMyMessage, msg) {
                        var urlImage = '';
                        if (!isMyMessage) urlImage = $scope.Chat.myInfo.UrlImagenPerfil;
                        else urlImage = $scope.Chat.active.UrlImageProfile;
                        if (urlImage === undefined || urlImage === '') urlImage = $scope.Chat.UrlImagePerfilDefault;

                        var divMsg = $('<div class="msg"><figure class="msg-avatar"><img src="' + urlImage + '" /></figure><div class="msg-content"></div></div>');
                        divMsg.find('.msg-content').text(msg);

                        if (isMyMessage) divMsg.addClass('msg-andrew');
                        else divMsg.addClass('msg-visitor');

                        return divMsg;
                    },
                    send: function () {
                        if ($scope.Chat.active.msg === '') return;

                        $scope.Chat.activeChat.PersonasOwner = $scope.Chat.myInfo;
                        $scope.Chat.activeChat.Creacion = new Date();
                        $scope.Chat.activeChat.UltimoMensaje = { Mensaje: $scope.Chat.active.msg, FechaMensaje: new Date() }
                        //service.sendMessage($scope.Chat.activeChat);

                        var contentMsg = angular.element(document.querySelector('.chat-msgs'));
                        contentMsg.append($scope.Chat.printMessage(false, $scope.Chat.active.msg));
                        $scope.Chat.scrollBottom();
                        
                        if ($scope.Chat.activeChat.CodigoChatEnvia === undefined || $scope.Chat.activeChat.CodigoChatEnvia === 0)
                            $scope.Chat.saveChat($scope.Chat.active.msg);
                        else
                            $scope.Chat.saveMessage($scope.Chat.active.msg);

                        $scope.Chat.active.msg = "";
                        
                    },
                    getContacts: function () {
                        if ($scope.Chat.isBusy || $scope.Chat.endList) return;
                        $scope.Chat.isBusy = true;
                        $scope.Chat.filter.SkipIndexBase += 5;
                        service.post(controller + 'GetListContacs', $scope.Chat.filter, function (res) {
                            if (res.data.Success) {
                                if (res.data.list.length < 5) {
                                    $scope.Chat.endList = true;
                                }
                                $scope.Chat.contacts = $scope.Chat.contacts.concat(res.data.list);
                            }
                            $scope.Chat.isBusy = false;
                        })
                    },
                    getMoreContacts: function () {
                        if ($scope.Chat.viewContacts) {
                            $scope.Chat.getContacts();
                        } else {
                            $scope.Chat.getChats();
                        }
                    },
                    openChatWindow: function (contact) {
                        $scope.Chat.active.name = contact.PersonasContacto.NombreYApellido;
                        $scope.Chat.active.UrlImageProfile = contact.PersonasContacto.UrlImagenPerfil;
                        $scope.Chat.active.personId = contact.PersonasContacto.Consecutivo;
                        $scope.Chat.activeChat.PersonasNoOwner = contact.PersonasContacto;
                        $rootScope.$emit('onGetActiveChat', { personId: contact.PersonasContacto.Consecutivo });
                        $scope.Chat.getChatsFromContact();
                        service.getConnectionIdByUserId(contact.PersonasContacto.Consecutivo, function (connectionId) { // Get contact connection id
                            if (connectionId != '') $('#imgChatActive').removeClass("disconected").addClass("conected");
                            else $('#imgChatActive').removeClass("conected").addClass("disconected");

                        });
                        $scope.Chat.selectedChat = true;

                    },
                    getChatsFromContact: function () {
                        $('#chat-msgs').empty();
                        $scope.Chat.showButtonMoreMessages = false;
                        $scope.Chat.activeChat.TakeIndexBase = $scope.Chat.rangeMessagesToTake;
                        service.post(controller + 'GetChatFromContact', { CodigoPersonaNoOwner: $scope.Chat.active.personId }, function (res) {
                            if (res.data.Success) {
                                $scope.Chat.activeChat.Consecutivo = res.data.obj.Consecutivo;
                                $scope.Chat.activeChat.CodigoChatEnvia = res.data.obj.Consecutivo;
                                $scope.Chat.activeChat.CodigoChatRecibe = res.data.obj.CodigoChatRecibe;
                                $scope.Chat.activeChat.CodigoPersonaNoOwner = res.data.obj.CodigoPersonaNoOwner;
                                $scope.Chat.activeChat.CodigoPersonaOwner = res.data.obj.CodigoPersonaOwner;
                                $scope.Chat.activeChat.ZonaHorariaGMTBase = service.getTimeZone();
                                var timeZone = service.getTimeZone();
                                if (res.data.obj.Consecutivo != 0) {
                                    service.post(controller + 'GetChatMessages', $scope.Chat.activeChat, function (res) {
                                        if (res.data.Success) {
                                            if (res.data.list.length === $scope.Chat.rangeMessagesToTake) $scope.Chat.showButtonMoreMessages = true;
                                            if (res.data.list.length > 0) {
                                                $scope.Chat.messageList = res.data.list;
                                                $scope.Chat.showMessages(res.data.list);
                                            }
                                        }
                                        else {
                                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                        }
                                    })
                                }
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    getMoreChatsFromContact: function () {
                        $scope.Chat.activeChat.TakeIndexBase = $scope.Chat.rangeMessagesToTake;
                        $scope.Chat.activeChat.SkipIndexBase += $scope.Chat.rangeMessagesToTake;
                        service.post(controller + 'GetChatFromContact', { CodigoPersonaNoOwner: $scope.Chat.active.personId }, function (res) {
                            if (res.data.Success) {
                                if (res.data.obj.Consecutivo != 0) {
                                    service.post(controller + 'GetChatMessages', $scope.Chat.activeChat, function (res) {
                                        if (res.data.Success) {
                                            if (res.data.list.length === $scope.Chat.rangeMessagesToTake) $scope.Chat.showButtonMoreMessages = true;
                                            if (res.data.list.length > 0) $scope.Chat.showMoreMessages(res.data.list);
                                        }
                                        else {
                                            service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                        }
                                    })
                                }
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    saveChat: function (msg) {
                        if ($scope.Chat.activeChat.Consecutivo === undefined || $scope.Chat.activeChat.Consecutivo === 0) {
                            service.post(controller + 'SaveChat', { CodigoPersonaNoOwner: $scope.Chat.active.personId }, function (res) {
                                if (res.data.Success) {
                                    $scope.Chat.activeChat.CodigoChatEnvia = res.data.obj.ConsecutivoCreado;
                                    $scope.Chat.activeChat.CodigoChatRecibe = res.data.obj.ConsecutivoChatRecibe;
                                    $scope.Chat.saveMessage(msg);
                                }
                                else {
                                    service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                                }
                            })
                        }
                    },
                    saveMessage: function (msg) {
                        $scope.Chat.activeChat.Mensaje = msg;
                        service.post(controller + 'SaveMessage', $scope.Chat.activeChat, function (res) {
                            if (res.data.Success) {
                                $scope.Chat.myLastMessage = res.data.obj;
                                $scope.Chat.activeChat.UltimoMensaje.Consecutivo = res.data.obj.ConsecutivoCreado;
                                $scope.Chat.messageList.push($scope.Chat.activeChat.UltimoMensaje);
                                service.sendMessage($scope.Chat.activeChat);
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    showMessages: function (messages) {
                        for (var x in messages) {
                            var contentMsg = angular.element(document.querySelector('.chat-msgs'));
                            var isMyMessage = false;
                            if (messages[x].CodigoChatEnvia == $scope.Chat.activeChat.CodigoChatEnvia) isMyMessage = false;
                            else isMyMessage = true;
                            contentMsg.append($scope.Chat.printMessage(isMyMessage, messages[x].Mensaje));
                            $scope.Chat.scrollBottom();
                        }
                    },
                    showMoreMessages: function (messages) {
                        var firstMsg = $('.msg:first');
                        var curOffset = firstMsg.offset().top - $($('#ChatContainer')).scrollTop();
                        for (var i = messages.length - 1; i >= 0; i--) {
                            var isMyMessage = false;
                            if (messages[i].CodigoChatEnvia == $scope.Chat.activeChat.CodigoChatEnvia) isMyMessage = false;
                            else isMyMessage = true;
                            firstMsg.before($scope.Chat.printMessage(isMyMessage, messages[i].Mensaje));
                        }
                        $($('#ChatContainer')).scrollTop(firstMsg.offset().top - curOffset);
                    },
                    scrollBottom: function () {
                        $timeout(function () {
                            var scroller = document.getElementById("ChatContainer");
                            scroller.scrollTop = scroller.scrollHeight;
                        }, 0, false);
                    },
                    getChats: function () {
                        if ($scope.Chat.isBusy) return;
                        $scope.Chat.isBusy = true;
                        $scope.Chat.filter.SkipIndexBase += 5;
                        service.post(controller + 'GetAllChats', { SkipIndexBase: 0, TakeIndexBase: 99999, ZonaHorariaGMTBase: service.getTimeZone() }, function (res) {
                            if (res.data.Success) {
                                for (var x in res.data.list) {
                                    res.data.list[x].PersonasContacto = res.data.list[x].PersonasNoOwner;
                                }
                                $scope.Chat.contacts = res.data.list;
                                
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                            $scope.Chat.isBusy = false;
                        })
                    },
                    delete: function () {
                        service.post(controller + 'DeleteConversation', $scope.Chat.activeChat, function (res) {
                            if (res.data.Success) {
                                $('#chat-msgs').empty();
                                if (!$scope.Chat.viewContacts) {
                                    $scope.Chat.getChats();
                                }
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('success'));
                                $scope.Chat.selectedChat = false;
                            }
                            else {
                                service.showErrorMessage(res.data.Message, service.getTypeMessage('error'))
                            }
                        })
                    },
                    getUserLogin: function () {
                        service.get('Profile/getInfoPerson', null, function (res) {
                            $scope.Chat.myInfo = res.data.obj;
                        })
                    },
                    showListContacts: function (show) {
                        $scope.Chat.viewContacts = show;
                        $scope.Chat.contacts = [];
                        $scope.Chat.filter.SkipIndexBase = -5;
                        $scope.Chat.endList = false;
                        $scope.Chat.isBusy = false;
                        if ($scope.Chat.viewContacts) {
                            $scope.Chat.getContacts();
                        } else {
                            $scope.Chat.getChats();
                        }
                    }
                };

                // Load Functions
                $scope.Chat.getChats();
                $scope.Chat.getUserLogin();

            }]
        )
        .directive("scrollContacts", function ($window, $rootScope) {
            return function (scope, element, attrs) {
                angular.element($('.people')).bind("scroll", function () {
                    $rootScope.$emit("GetMoreContacts", {});
                });
            };
        }
        );
    
})();
