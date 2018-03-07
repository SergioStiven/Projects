(function () {
    'use strict';

    angular
        .module('app')
        .factory('service', service);

    service.$inject = ['$http', '$q', '$filter', '$rootScope', '$translate'];

    function service($http, $q, $filter, $rootScope, $translate) {

        /// privates
        //var _urlBase = 'http://localhost:2013/';
        //var _urlBase = 'http://app.1980tic.com/SportsGoWeb/';
        //var _urlBase = 'http://201.184.78.68/SportsGoWeb/';
        //var _urlBase = 'http://74.208.251.233/SportsGoWeb/';
        //var _urlBase = 'http://74.208.251.233/SportsGoWebPruebas/';
        //var _urlBase = 'http://app.sportsgo.pro/SportsGoWeb/';
        var _urlBase = 'http://app.sportsgo.pro/SportsGoWebPruebas/';

        //var _hubConnection = $.hubConnection('http://app.1980tic.com/WebApiService/signalr', { useDefaultPath: false });
        //var _hubConnection = $.hubConnection('http://201.184.78.68/WebApiService/signalr', { useDefaultPath: false });
        //var _hubConnection = $.hubConnection('http://74.208.251.233/WebApiService/signalr', { useDefaultPath: false });
        //var _hubConnection = $.hubConnection('http://74.208.251.233/WebApiServicePruebas/signalr', { useDefaultPath: false });
        //var _hubConnection = $.hubConnection('http://app.sportsgo.pro/WebApiService/signalr', { useDefaultPath: false });
        var _hubConnection = $.hubConnection('http://app.sportsgo.pro/WebApiServicePruebas/signalr', { useDefaultPath: false });

        var _chatHubProxy = undefined;
        var myIdConnection = null;
        //// Service methods implementation
        function _initializeSignalR(personId) {
            _chatHubProxy = _hubConnection.createHubProxy('chatHub');
            _hubConnection.qs = "myUserId=" + personId;

            // Notify Chat
            _chatHubProxy.on('receiveMessage', function (chatReceived) {
                $rootScope.$emit('onGetMessage', chatReceived); // This emit is called from controller
            });

            // connect
            _hubConnection.start()
                .done(function () {
                    //console.log('Conected with signalR success!.');
                })
                .fail(function () { console.error('Error connecting to signalR'); });
        }

        function _sendMessage(chatDestination) {
            _chatHubProxy.invoke('SendMessageToPerson', chatDestination);
        }

        function _getConnectionIdByUserId(userId, callback) {
            _chatHubProxy.invoke('get_connectionid', userId).done(callback);
        }

        function getTypeMessage(type) {
            var typeMessage = {
                error: { color: '#e91e63', actionText: $translate.instant('BTN_ACCEPT') },
                success: { color: '#4caf50', actionText: $translate.instant('BTN_ACCEPT') },
                warning: { color: '#eb8c00', actionText: $translate.instant('BTN_ACCEPT') },
                info: { color: '#00aac0', actionText: $translate.instant('BTN_ACCEPT') }
            }
            switch (type) {
                case 'error':
                    return typeMessage.error;
                case 'success':
                    return typeMessage.success;
                case 'warning':
                    return typeMessage.warning;
                case 'info':
                    return typeMessage.info;
                default:
                    return typeMessage.error;
            }
        }

        function get(urlFunction, data, callback) {

            $http.get(service.urlBase + urlFunction, data, {
                headers: { 'Content-Type': 'application/json' }
            })
                .then(
                function (res) {
                    if (res.data.Path !== '' && res.data.Path !== null) {
                        window.location.href = service.urlBase + res.data.Path;
                    }
                    else {
                        callback(res);
                    }
                }
                ),
                function (e) {
                    showErrorMessage('Error: ' + e, getTypeMessage('error'));
                };
        }

        function post(urlFunction, data, callback) {
            $http.post(service.urlBase + urlFunction, data, {
                headers: { 'Content-Type': 'application/json' }
            })
                .then(
                function (res) {
                    if (res.data.Path !== '' && res.data.Path !== null) {
                        window.location.href = service.urlBase + res.data.Path;
                    }
                    else {
                        callback(res);
                    }
                }
                ),
                function (e) {
                    showErrorMessage('Error: ' + e, getTypeMessage('error'));
                };
        }

        function showErrorMessage(message, type) {
            Snackbar.show({
                text: message,
                width: '475px',
                actionTextColor: type.color,
                actionText: type.actionText,
                onActionClick: function (element) {
                    $(element).css('opacity', 0);
                    //alert('Clicked Called!');
                }
            })
        }

        function showNotiChat(message, name, urlImage) {
            Snackbar.show({
                text: "<img src='" + urlImage + "' style='with:40px;height:50px;border-radius:50%;' />" +
                "<span style='padding:10px;'>" + name + ": </span>" +
                "<span>" + message + "</span>",
                width: '475px',
                duration: 10000,
                showAction: false
            })
        }

        function getLenguageFromNavigator() {
            var lenguage = 0;
            if (navigator.appName === 'Netscape')
                lenguage = navigator.language;
            else
                lenguage = navigator.browserLanguage;

            if (lenguage.indexOf('en') > -1) {
                return 2; // Inglés
            }
            else if (lenguage.indexOf('es') > -1) {
                return 1; // Español
            }
            else {
                return 2; // Si no es ni español ni inglés retorna español
            }
        }

        function getLenguageById(id) {
            if (id === 1)
                return "Español";
            else if (id === 2)
                return "Inglés";
            else
                return "Portugués";
        }

        function formatoFecha(date) {
            var re = /-?\d+/;
            var m = re.exec(date);
            var d = new Date(parseInt(m));
            return d;
        }

        function formatoFechaCorta(date, format) {
            var re = /-?\d+/;
            var m = re.exec(date);
            var d = new Date($filter('date')(parseInt(m), format));
            return d;
        }

        function toDateDotNet(date) {
            var d = $filter('date')(date);
            return d;
        }

        function getTimeZone() {
            var hrs = -(new Date().getTimezoneOffset() / 60);
            return hrs;
        }

        /// Public
        var service = {
            urlBase: _urlBase,
            urlImageProfileDefault: _urlBase + 'Content/assets/img/demo/avatar.png',
            urlImageBannerDefault: _urlBase + 'Content/assets/img/ball-2226172_1280.jpg',
            get: get,
            post: post,
            showErrorMessage: showErrorMessage,
            showNotiChat: showNotiChat,
            getTypeMessage: getTypeMessage,
            getLenguageFromNavigator: getLenguageFromNavigator,
            getLenguageById: getLenguageById,
            formatoFecha: formatoFecha,
            formatoFechaCorta: formatoFechaCorta,
            toDateDotNet: toDateDotNet,
            initializeSignalR: _initializeSignalR,
            sendMessage: _sendMessage,
            getConnectionIdByUserId: _getConnectionIdByUserId,
            getTimeZone: getTimeZone
        };

        return service;

    }
})();
