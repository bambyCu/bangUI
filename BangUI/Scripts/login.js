﻿(function () {
    var myHub = $.connection.mainHub;

    $.connection.hub.start()
        .done(function () {
            console.log("hub is alive bitch");
        })
        .fail(function () { alert("this doesn't works, bitch"); });

    myHub.client.announce = function (message) {
        writeTestText(message);
    }

    myHub.client.logIn = function (user) {
        writeTestText(user);
        document.cookie = "username=" + user;
    }

    var writeTestText = function (message) { $("#test_text").append(message + "<br/>"); }

    $("#login").submit(function (event) {
        myHub.server.logIn($("#username").first().val())
            .done(function (data) {
                if (data === true) {
                    console.log("deed has been done");
                    window.location.href = "Base"
                }
                else {
                    $("#message").text($("#username").first().val() + " is used");
                    console.log("occupied");
                }
            })
            .fail(function (data) { alert(data); });
    });
})()