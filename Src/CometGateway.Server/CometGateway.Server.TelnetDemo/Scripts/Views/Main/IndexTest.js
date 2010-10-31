/// <reference path="../../jquery-1.4.1-vsdoc.js" />
/// <reference path="../../qunit.js" />
/// <reference path="../../cometdwrapper.js" />
/// <reference path="Index.js" />

test("testPageLoaded", function () {

    stubCometWrapper = function () {
        return {
            channelName: null,
            cometd: null,
            cometHandlerPath: null,
            onConnectionCompleted: function () { },
            init: function (cometd, cometHandlerPath, channelName) {
                this.cometd = cometd;
                this.cometHandlerPath = cometHandlerPath,
                this.channelName = channelName;
            }
        }
    } ();

    var testPageController = new PageController(stubCometWrapper);
    testPageController.pageLoaded();
    ok(stubCometWrapper.cometd == $.cometd, "Invalid cometd parameter");
    ok(/^\/telnet-[a-zA-Z0-9]{32}$/.test(stubCometWrapper.channelName), "Invalid channel name");
    equal(stubCometWrapper.cometHandlerPath, "/CometGateway.Server.TelnetDemo/comet.axd");

    stubCometWrapper.onConnectionCompleted();
    ok($("#connectDialog").dialog("isOpen"), "Dialog open after comet connect");
});

test("dialogSwitcher", function () {
    var testPageController = new PageController(stubCometWrapper);
    testPageController.pageLoaded(); 
    equal($("#connectDialog").dialog("option", "title"), "Connect to server");
});

test("testConnect", function () {

    stubCometWrapper = function () {
        return {
            message: { data: { type: null, server: null, port: null} },
            onMessageReceived: null,
            sendMessage: function (message) {
                this.message = message;
            }
        }
    } ();

    $("#txtServer").val("aardwolf.com");
    $("#txtPort").val("4000");
    var testPageController = new PageController(stubCometWrapper);
    testPageController.testSetup();
    testPageController.onClickConnect();

    equal(stubCometWrapper.message.type, "connect");
    equal(stubCometWrapper.message.server, "aardwolf.com");
    equal(stubCometWrapper.message.port, "4000");
    ok(!($("#connectDialog").dialog("isOpen")), "Connect dialog closed after comet connect");
    ok($("#cancelDialog").dialog("isOpen"), "Cancel dialog open after comet connect");
});

test("testConnectSucceeded", function () {

    stubCometWrapper = function () {
        return {
            message: { data: { type: null, server: null, port: null} },
            onMessageReceived: null,
            sendMessage: function (message) {
                this.message = message;
            }
        }
    } ();

    $("#txtServer").val("aardwolf.com");
    $("#txtPort").val("4000");
    var testPageController = new PageController(stubCometWrapper);
    testPageController.testSetup()
    testPageController.onClickConnect();
    stubCometWrapper.onMessageReceived({ data: { type: "connectionSucceeded"} });

    ok(!$("#connectDialog").dialog("isOpen"), "Connect dialog closed after connection succeeded");
    ok(!$("#cancelDialog").dialog("isOpen"), "Cancel dialog open after connection succeeded");
});

test("testErrorOccurred", function () {

    stubCometWrapper = function () {
        return {
            message: { data: { type: null, server: null, port: null} },
            onMessageReceived: null,
            sendMessage: function (message) {
                this.message = message;
            }
        }
    } ();

    var testPageController = new PageController(stubCometWrapper);
    testPageController.testSetup()
    testPageController.onClickConnect();
    stubCometWrapper.onMessageReceived({ data: { type: "errorOccurred", errorMessage: "It's wrong"} });

    ok(!$("#connectDialog").dialog("isOpen"), "Connect dialog closed after error");
    ok(!$("#cancelDialog").dialog("isOpen"), "Cancel dialog closed after error");
    ok($("#errorOccurred").dialog("isOpen"), "Error dialog open after error");
    equal($("#errorMsg").text(), "It's wrong");

    $("#btnClose").click();

    ok($("#connectDialog").dialog("isOpen"), "Connect dialog reopened after error");
});

test("testDisconnected", function () {

    stubCometWrapper = function () {
        return {
            message: { data: { type: null, server: null, port: null} },
            onMessageReceived: null,
            sendMessage: function (message) {
                this.message = message;
            }
        }
    } ();

    $("#txtServer").val("aardwolf.com");
    $("#txtPort").val("4000");
    var testPageController = new PageController(stubCometWrapper);
    testPageController.testSetup()
    testPageController.onClickConnect();
    stubCometWrapper.onMessageReceived({ data: { type: "connectionSucceeded"} });
    stubCometWrapper.onMessageReceived({ data: { type: "disconnected"} });

    ok($("#connectDialog").dialog("isOpen"), "Connect dialog reopen after disconnect");
});

test("ignoreIfClientIdPresent", function () {

    stubCometWrapper = function () {
        return {
            message: { data: { type: null, server: null, port: null} },
            onMessageReceived: null,
            sendMessage: function (message) {
                this.message = message;
            }
        }
    } ();

    $("#txtServer").val("aardwolf.com");
    $("#txtPort").val("4000");
    var testPageController = new PageController(stubCometWrapper);
    testPageController.testSetup()
    testPageController.onClickConnect();
    stubCometWrapper.onMessageReceived({ clientId: "ABC", data: { type: "XXXX"} });
});

QUnit.done = function () {
    $("body").contents().not("#testResults").hide();
}