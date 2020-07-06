(function () {
    var myHub = $.connection.mainHub;

    $.connection.hub.start()
        .done(function () {
            console.log("hub is alive bitch");
            myHub.server.getUsers()
                .done(function (data) { $("#userList").text(data + "afsdfasdfsdfasdf") })
                .fail(function (e) { alert("there seems to be a problem" + e)});
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

})()