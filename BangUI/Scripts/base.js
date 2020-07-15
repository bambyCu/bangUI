(function () {
    let userName = "";
    let salt = "name:";
    let myHub = $.connection.mainHub;

    let thyName = function (str) {
        let person = prompt(str, "");
        if (person == null || person == "" || person.length > 10 ||
            person.replace(/[^a-z0-9]/gi, '').length !== person.length) {
            thyName(person + " is not correct name");
        } else {
            userName = person;
        }
    }

    let actAppl = function () {
        let _elem = document.getElementById($(this).attr('id'));
        if (_elem.classList.contains("active")) {
            _elem.classList.remove("active")
        }
        else {
            _elem.classList.add("active");
        }
        console.log();
        console.log("sadasdasd");
    };

    let addToLogged = function (str) {
        $('#loggedList').append('<button id=\"' + salt + str + '\" type=\"button\" class=\"list-group-item myBtn\"> ' + str + '</button>');
        document.getElementById(salt + str).addEventListener("click", actAppl);
    }

    let removeFromLogged = function (str) {
        document.getElementById('#' + salt + str).remove();
    }
    

    myHub.client.announce = function (message) {
        writeTestText(message);
    }

    myHub.client.logIn = function (user) {
        writeTestText(user);
        document.cookie = "username=" + user;
    }

    $.connection.hub.start()
        .done(function () {
            console.log("hub is alive bitch");
            thyName("give me thy name(alphanumeric shorter than 11 chars)")
            addToLogged(userName)
            addToLogged("pelikan")

            myHub.server.getUsers()
                .done(function (data) { $("#dd").text(data + "afsdfasdfsdfasdf") })
                .fail(function (e) { alert("there seems to be a problem" + e)});
        })
        .fail(function () { alert("this doesn't works, bitch"); });

    

   

})()