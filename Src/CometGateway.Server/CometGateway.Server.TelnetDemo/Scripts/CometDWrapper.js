var cometWrapper = function () {
    var _cometdWrapperSubscription;
    var _metaSubscriptions = [];
    var _cometd;
    var _disconnecting;
    var _channelName;
    var _this;

    return {
        onConnectionCompleted: null,
        onMessageReceived: null,

        init: function (cometd, cometPath, channelName) {

            // Store the initialisation parameters    
            _cometd = cometd;
            _channelName = channelName;
            _this = this;

            // Subscribe to the meta channels
            _metaSubscribe();

            // Configure the connection
            _cometd.configure({ url: cometPath });

            // And handshake 
            _cometd.handshake();
        },

        leave: function () {

            _unsubscribe();
            _cometd.disconnect();
            _metaUnsubscribe();
            _disconnecting = true;
        },

        sendMessage: function (message) {
            _cometd.publish(_channelName, message);
        }
    }

    function _unsubscribe() {
        if (_cometdWrapperSubscription)
            _cometd.unsubscribe(_cometdWrapperSubscription);
        _cometdWrapperSubscription = null;
    }

    function _subscribe() {
        _unsubscribe();
        _cometdWrapperSubscription = _cometd.subscribe(_channelName, _this, _this.onMessageReceived);
    }

    function _subscribeCompleted(message) {
        _log("Subscribe completed");
        if (message.successful) {
            _raise(_this.onConnectionCompleted);
        }
    }

    function _metaUnsubscribe() {
        for (var subNumber in _metaSubscriptions) {
            _cometd.removeListener(_metaSubscriptions[subNumber]);
        }
        _metaSubscriptions = [];
    }

    function _metaSubscribe() {
        _metaUnsubscribe();
        _metaSubscriptions.push(_cometd.addListener('/meta/handshake', _this, _metaHandshake));
        _metaSubscriptions.push(_cometd.addListener('/meta/connect', _this, _metaConnect));
        _metaSubscriptions.push(_cometd.addListener('/meta/unsuccessful', _this, _metaUnsuccessful));
        _metaSubscriptions.push(_cometd.addListener('/meta/subscribe', _this, _subscribeCompleted));
    }

    function _metaHandshake(message) {
        _connected = false;
        _cometdWrapperSubscription = null;
        _log("Handshake complete. Successful? " + message.successful);
    }

    function _connectionEstablished() {
        _log('Connection to Server Opened');
        _cometd.batch(function () {
            _subscribe();
        });
    }

    function _connectionBroken() {
        _log("Connection to Server Broken");
    }

    function _connectionClosed() {
        _log('Connection to Server Closed');
    }

    var _connected = false;
    function _metaConnect(message) {
        if (_disconnecting) {
            _connected = false;
            _connectionClosed();
        }
        else {
            var wasConnected = _connected;
            _connected = message.successful === true;
            if (!wasConnected && _connected) {
                _connectionEstablished();
            }
            else if (wasConnected && !_connected) {
                _connectionBroken();
            }
        }
    }

    function _metaUnsuccessful(message) {
        _log("Request on channel " + message.channel + " failed: " + (message.error == undefined ? "No message" : message.error));
    }

    function _log(message) {
        if (console.log) {
            console.log(message);
        }
    }

    function _raise(event) {
        if (event != null) {
            event();
        }
    }
} ();
