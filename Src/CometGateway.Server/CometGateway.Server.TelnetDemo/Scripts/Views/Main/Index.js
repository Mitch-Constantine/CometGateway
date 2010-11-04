/// <reference path="../../jquery-1.4.1-vsdoc.js" />
/// <reference path="../../cometdwrapper.js" />

var PageController = function (cometWrapper) {

    var dialogSwitcher = function () {

        return {

            loadDialogs: function () {
                $(".dialog").each(function (index, e) {
                    $(e).dialog({
                        autoOpen: false,
                        title: $(e).find(".header").text()
                    });
                });
            },

            setDialog: function (dialogId) {
                $(".dialog").each(
                function (index, e) {
                    if ($(e).dialog("isOpen")) {
                        $(e).dialog("close");
                    }
                });
                if (dialogId) {
                    $("#" + dialogId).dialog("open");
                }
            }
        }
    } ();

    function _onConnectionCompleted() {
        $("#connectDialog").dialog("open");
    }

    function _getChannelId() {
        return "/telnet-" + _createUUID();
    }

    function _createUUID() {
        // http://www.ietf.org/rfc/rfc4122.txt
        var s = [];
        var hexDigits = "0123456789ABCDEF";
        for (var i = 0; i < 32; i++) {
            s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
        }
        s[12] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
        s[16] = hexDigits.substr((s[16] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01

        var uuid = s.join("");
        return uuid;
    }

    function _handleMessage(message) {
        if (message.clientId)
            return;
        messageType = message.data.type;
        var handler = eval("_handle_" + messageType);
        if (!handler)
            return;
        handler.call(this, message.data);
    }

    function _handle_connectionSucceeded(messageData) {
        dialogSwitcher.setDialog(null);
    }

    function _handle_disconnected(messageData) {
        dialogSwitcher.setDialog("connectDialog");
    }

    function _handle_textReceived(messageData) {
        $("#textReceived").append(messageData.text);
    }

    function _handle_errorOccurred(messageData) {
        dialogSwitcher.setDialog("errorOccurred");
        $("#errorMsg").text(messageData.errorMessage);
        $("#btnClose").click(
            function () {
                dialogSwitcher.setDialog("connectDialog");
            });
    }

    return {
        pageLoaded: function () {
            cometWrapper.onConnectionCompleted = _onConnectionCompleted;
            cometWrapper.onMessageReceived = _handleMessage;
            var cometdPath = $("#applicationPath").val() + "/comet.axd";
            cometWrapper.init($.cometd, cometdPath, _getChannelId());

            dialogSwitcher.loadDialogs();
        },

        testSetup: function () {
            dialogSwitcher.loadDialogs();
        },

        onClickConnect: function () {
            dialogSwitcher.setDialog("cancelDialog");
            cometWrapper.sendMessage(
                {
                    type: "connect",
                    server: $("#txtServer").val(),
                    port: $("#txtPort").val()
                });
            var that = this;
            cometWrapper.onMessageReceived = function (message) { _handleMessage(message); };
        },

        onClickSend: function () {
            cometWrapper.sendMessage(
                {
                    type: "textEntered",
                    text: $("#txtTextTyped").val() + "\n"
                });
        }
    };
};


var pageController = new PageController(cometWrapper);

function inTestMode() {
    return typeof(testMode) != "undefined";
}

if (!inTestMode()) {
    $(document).ready(function () {
        pageController.pageLoaded();
        $("#btnConnect").click(function () { pageController.onClickConnect(); });
        $("#btnLineReady").click(function () { pageController.onClickSend(); });
    });
}