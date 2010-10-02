/// <reference path="../../jquery-1.4.1-vsdoc.js" />

function log(message) {
    if (console != null)
        console.log(message);
}

function handleIncomingMessage(message) {
    $("#messages").append(message.data.message);
}

var telnet = function () {
    var _telnetSubscription;
    var _metaSubscriptions = [];
    var _disconnecting;

    return {
        init: function () {

            // Subscribe to the meta channels
            _metaSubscribe();

            // Configure the connection
            $.cometd.configure({ url: 'comet.axd' });

            // And handshake - with authentication, as described at
            // http://cometd.org/documentation/howtos/authentication
            $.cometd.handshake();
        }

        , leave: function () {

            _unsubscribe();
            $.cometd.disconnect();

            _metaUnsubscribe();
            _disconnecting = true;
        }

    }

    function _unsubscribe() {
        if (_telnetSubscription) $.cometd.unsubscribe(_telnetSubscription);
        _telnetSubscription = null;
    }

    function _subscribe() {
        _unsubscribe();
        _telnetSubscription = $.cometd.subscribe('/telnet', this, handleIncomingMessage);
    }

    function _metaUnsubscribe() {
        for (var subNumber in _metaSubscriptions) {
            $.cometd.removeListener(_metaSubscriptions[subNumber]);
        }
        _metaSubscriptions = [];
    }

    function _metaSubscribe() {
        _metaUnsubscribe();
        _metaSubscriptions.push($.cometd.addListener('/meta/handshake', this, _metaHandshake));
        _metaSubscriptions.push($.cometd.addListener('/meta/connect', this, _metaConnect));
        _metaSubscriptions.push($.cometd.addListener('/meta/unsuccessful', this, _metaUnsuccessful));
    }

    function _metaHandshake(message) {
        _connected = false;
        _telnetSubscription = null;
        log("Handshake complete. Successful? " + message.successful);
    }

    function _connectionEstablished() {
        log('Connection to Server Opened');
        $.cometd.batch(function () {
            _subscribe();
        });
    }

    function _connectionBroken() {
        log('Connection to Server Broken');
    }

    function _connectionClosed() {
        log('Connection to Server Closed');
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
        log("Request on channel " + message.channel + " failed: " + (message.error == undefined ? "No message" : message.error));
    }

} ();
$(document).ready(
    function () {
        console.log("Starting connection");
        telnet.init();
        $(window).unload(
            function () {
                telnet.leave();
            });

            // Publish any messages the user enters
            $('#send').click(function () {
                $.cometd.publish('/telnet', { sender: name, message: $('#message').val() });
                $('#message').focus().val('');
                return false;
            });
        });

