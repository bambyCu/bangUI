(function () {
    let USERNAME = "";
    let SALT = "name:";
    let myHub = $.connection.mainHub;
    let ACTIVEBUTTONCLASS = "active"

    let thyName = function (str) {
        let person = prompt(str, "");
        $("#nameHold").text("name:" + person);
        if (person == null || person == "" || person.length > 10 ||
            person.replace(/[^a-z0-9]/gi, '').length !== person.length) {
            thyName(person + " is not correct name");
        } else {
            USERNAME = person;
            
            myHub.server.logIn(person)
                .done(function (val) {
                    if (!val) { thyName(person + " is taken"); }
                })
                .fail(function (val) { alert("something happened"); })
        }
    }

    let actAppl = function () {
        let elem = document.getElementById($(this).attr('id'));
        if (elem.classList.contains("active")) {
            elem.classList.remove("active")
        }
        else {
            elem.classList.add("active");
        }
    };

    let addToLogged = function (str) {
        $('#loggedList').append('<button id=\"' + SALT + str + '\" type=\"button\" class=\"list-group-item myBtn\">' + str + '</button>');
        document.getElementById(SALT + str).addEventListener("click", actAppl);
    }

    let removeFromLogged = function (str) {
        document.getElementById( SALT + str).remove();
    }

    let startGame = function () {
        let temp = document.getElementsByClassName(ACTIVEBUTTONCLASS);
        let users = [];
        for (let i = 0; i < temp.length; i++) {
            users.push(temp[i].textContent);
        }
        myHub.server.gameInvitation(users)
            .done(function (val) {
                if (val === false) {
                    alert("game invitations not sent, you made mistake" + val);
                }
                else {
                    console.log('game invitations have been sent');
                }
            });
    }

    let inviteRefuse = function (group) {
        myHub.server.getInVal(group, false).done(function () {
            console.log("invitation has not been accepted");
        });
    }

    let inviteAccept = function (group) {
        myHub.server.getInVal(group, true).done(function () {
            console.log("invitation has  been accepted");
        });
    }

    myHub.client.logIn = function (user) {
        addToLogged(user);
    }

    myHub.client.disconnect = function (user) {
        removeFromLogged(user);
    }

    myHub.client.invitation = function (group, team) {
        $("#modalBody").text("does thou wish to play with " + team + " ?");
        $("#exampleModal").modal();
        document.getElementById("invBtnYes").addEventListener("click", function () { inviteAccept(group);});
        document.getElementById("invBtnNo").addEventListener("click", function () { inviteRefuse(group);});
    }

    myHub.client.invitationRefused = function (names) {
        $("#exampleModal").modal('hide');
    }

    myHub.client.displayImage = function(byteArrayAsBase64 ){
        document.getElementById("heroPhoto").src = "data:image/png;base64," + byteArrayAsBase64;;
    }

    $.connection.hub.start()
        .done(function () {
            myHub.server.getUsers()
                .done(function (data) {
                    for (let i = 0; i < data.length; i++) {
                        addToLogged(data[i]);
                    }
                })
                .fail(function (e) { alert("there seems to be a problem" + e) });
            document.getElementById("heroPhoto").src = "~/Content/Images/heroes/blackjack.png";
            thyName("give me thy name(alphanumeric shorter than 11 chars)")
            
        })
        .fail(function () { alert("this doesn't seem to work, bitch"); });

    document.getElementById("gameStarter").onclick = function () { startGame();}; 

   

})()