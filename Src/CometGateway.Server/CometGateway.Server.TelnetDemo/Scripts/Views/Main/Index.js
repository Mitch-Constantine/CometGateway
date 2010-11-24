/// <reference path="../../jquery-1.4.1-vsdoc.js" />
/// <reference path="../../cometdwrapper.js" />

var PageController = function (cometWrapper) {

    var dialogSwitcher = function () {

        return {

            loadDialogs: function () {
                $(".dialog").each(function (index, e) {
                    $(e).dialog({
                        autoOpen: false,
                        title: $(e).find(".header").text(),
                        width: "25em"
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
        $("#txtTextTyped").focus();
        $("#textReceived").text("")
    }

    function _handle_disconnected(messageData) {
        dialogSwitcher.setDialog("connectDialog");
    }

    function _handle_textReceived(messageData) {
        $("#textReceived")
            .append(messageData.text)
            .scrollTo("max");
    }

    function _handle_errorOccurred(messageData) {
        dialogSwitcher.setDialog("errorOccurred");
        $("#errorMsg").text(messageData.errorMessage);
        $("#btnClose").click(
            function () {
                dialogSwitcher.setDialog("connectDialog");
            });
    }

    function _getDefaultButton(divSelector) {
        return $(divSelector).find("button.default");
    }

    function _handleEnter(divSelector) {
        var inputsOnDiv = $(divSelector).find(":text");
        inputsOnDiv.keydown(function(e) {
            var ENTER = 13;
            if (e.which == ENTER) {
                _getDefaultButton(divSelector).trigger('click');
            }
        });
    }

    return {
        pageLoaded: function () {
            cometWrapper.onConnectionCompleted = _onConnectionCompleted;
            cometWrapper.onMessageReceived = _handleMessage;
            var cometdPath = $("#applicationPath").val() + "/comet.axd";
            cometWrapper.init($.cometd, cometdPath, _getChannelId());

            dialogSwitcher.loadDialogs();

            _handleEnter(".contentWrapper");
            this.parseServerPort();
        },

        testSetup: function () {
            dialogSwitcher.loadDialogs();
            this.parseServerPort();
        },

        parseServerPort: function () {
            var server = $("#hdnServer").val();
            var port = $("#hdnPort").val();

            if (!server || !port) {
                return;
            }

            $("#txtServer").val(server);
            $("#txtPort").val(port);
            this.onClickConnect();
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
            var text = $("#txtTextTyped").val() + "\n";
            cometWrapper.sendMessage(
                {
                    type: "textEntered",
                    text: text
                });
            $("#txtTextTyped")
                    .val("")
                    .focus();
            $("#textReceived").append(text);
        },
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
        $("#btnCancelConnect").click(function(e) {window.location.reload();});
    });
}