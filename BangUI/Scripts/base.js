(function () {
    let userName = "";
    let salt = "name:";
    let myHub = $.connection.mainHub;
    let activeButtonClass = "active"

    let thyName = function (str) {
        let person = prompt(str, "");
        $("#nameHold").text("name:" + person);
        if (person == null || person == "" || person.length > 10 ||
            person.replace(/[^a-z0-9]/gi, '').length !== person.length) {
            thyName(person + " is not correct name");
        } else {
            userName = person;
            
            myHub.server.logIn(person)
                .done(function (_val) {
                    if (!_val) { thyName(person + " is taken"); }
                })
                .fail(function (_val) { alert("something happened"); })
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
    };

    let addToLogged = function (str) {
        $('#loggedList').append('<button id=\"' + salt + str + '\" type=\"button\" class=\"list-group-item myBtn\">' + str + '</button>');
        document.getElementById(salt + str).addEventListener("click", actAppl);
    }

    let removeFromLogged = function (str) {
        document.getElementById( salt + str).remove();
    }

    let startGame = function () {
        let temp = document.getElementsByClassName(activeButtonClass);
        let _users = [];
        for (let i = 0; i < temp.length; i++) {
            _users.push(temp[i].textContent);
        }
        myHub.server.gameInvitation(_users)
            .done(function (_val) {
                if (_val === false) {
                    alert("game invitations not sent, you made mistake" + _val);
                }
                else {
                    console.log('game invitations have been sent');
                }
            });
    }

    myHub.client.logIn = function (user) {
        addToLogged(user);
    }

    myHub.client.disconnect = function (user) {
        removeFromLogged(user);
    }

    let inviteRefuse = function (_group){
        myHub.server.getInVal(_group, false).done(function () {
            console.log("invitation has not been accepted");
        });
    }

    let inviteAccept = function (_group){
        myHub.server.getInVal(_group, true).done(function () {
            console.log("invitation has  been accepted");
        });
    }

    myHub.client.invitation = function (_group, team) {
        $("#modalBody").text("does thou wish to play with " + team + " ?");
        $("#exampleModal").modal();
        document.getElementById("invBtnYes").addEventListener("click", function () { inviteAccept(_group);});
        document.getElementById("invBtnNo").addEventListener("click", function () { inviteRefuse(_group);});
    }

    myHub.client.invitationRefused = function (names) {
        $("#exampleModal").modal('hide');
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
            console.log("hub is alive bitch");
            thyName("give me thy name(alphanumeric shorter than 11 chars)")
            
        })
        .fail(function () { alert("this doesn't seem to work, bitch"); });

    document.getElementById("gameStarter").onclick = function () { startGame();}; 

   

})()