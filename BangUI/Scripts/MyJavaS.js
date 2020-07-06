(function () {
    var myHub = $.connection.mainHub;

    $("#chat").on("submit", function () {
        myHub.server.announce(document.getElementById("chat").value);
    })

    $("#button-test").on("click", function () {
        myHub.server.date()
            .done(function (data) { writeTestText("site can get time from server" + data) })
            .fail(function (data) { writeTestText("problem" + data) });
    })
    $.connection.hub.start()
        .done(function () {
            console.log("hub is alive bitch");
            //myHub.server.announce("hector ");
            //myHub.server.date()
            //    .done(function (data) { writeTestText(data); })
            //    .fail(function (ex) { writeTestText(ex); })

        })
        .fail(function () { alert("this doesn't works, bitch"); });

    myHub.client.announce = function (message) {
        writeTestText(message)
    }

    var writeTestText = function (message) { $("#test_text").append(message + "<br/>"); }


    $("#inTex").submit(function (event) {
        myHub.server.announce($("#message").first().val());
        $("#message").val("");
    });
})()