(function () {
    let USERNAME = "";
    let SALT = "name:";
    let myHub = $.connection.mainHub;
    let ACTIVEBUTTONCLASS = "active"
    let GAMEID = "";
    let DRAGED = "";

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

    function allowDrop(ev) {
        ev.preventDefault();
    }

    function drag(ev) {
        console.log("I drag now " + ev.target.id);

        DRAGED = ev.target.id;
        //ev.dataTransfer.setData("text", );
    }

    function drop(ev) {
        ev.preventDefault();

        console.log("I drop now " + DRAGED + " " + ev.target.id + " " + ev.currentTarget.id);

        document.getElementById(DRAGED).remove();
        //ev.target.appendChild(document.getElementById(data));
    }

    let setImage = function (byteArrayAsBase64, elementId) {
        document.getElementById(elementId).src = "data:image/png;base64," + byteArrayAsBase64;
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

    myHub.client.displayImage = function (cardByteArray64) {

        document.getElementById("heroPhoto").src = "data:image/png;base64," + cardByteArray64;
    }

    myHub.client.setHealth = function (health) {
        document.getElementById("characterHealth").innerHTML = "HEALTH:" + health;
    }

    myHub.client.setRole = function (role) {
        document.getElementById("characterRole").innerHTML = "ROLE:" + role;
    }


    myHub.client.addHandCard = function (cardByteArray64, cardId) {
        $('#playCards').append('<img id=\"' + cardId
            + '\"  class=\" col-2 mh-100 rounded mx-auto d-block\"'
            + 'draggable=\"true\"></img>');
        setImage(cardByteArray64, cardId);
        document.getElementById(cardId).ondragstart = function (event) {
            drag(event);
        };
    }

    myHub.client.addEnemy = function (name, health, role) {
        let nameId = "enemyDiv"+ name;
        $('#enemies').append(
            "<div class=\"list-group h-100 col-2 \">"
            + "<div class=\"list-group-item d-flex justify-content-center overflow-hidden\">NAME:" + name
            + "</div> <div class=\"list-group-item d-flex justify-content-center overflow-hidden\">HEALTH:" + health
            + "</div > <div class=\"list-group-item d-flex justify-content-center overflow-hidden\">ROLE:" + role + "</div>"
            + '<div id="' + nameId + '"class=" border border-dark w-100 h-100" style="z-index: 3;top:0;left:0;position: absolute;">'
            + "</div >");
        document.getElementById(nameId).ondragover = function (event) {
            allowDrop(event);
        };
        document.getElementById(nameId).ondrop = function (event) {
            drop(event);
        };

        document.getElementById(nameId).addEventListener("mouseover", function () {
            myHub.server.getInfoUser(GAMEID, name)
                .done(function () { console.log("dfsdf"); })
                .fail(function () { alert("sorry but connection is not great")});
        });
    }

    myHub.client.setGameId = function (str) {
        GAMEID = str
    }
    let elem = document.getElementById($(this).attr('id'));

    myHub.client.message = function (str) {
        alert(str);
    }

    myHub.client.setGameView = function (cardByteArray64, name, heroName, health, blueCards, handAmount,){
        setImage(cardByteArray64, "enemyPhoto");
        document.getElementById("enemyNameL").innerHTML = "NAME: " + name;
        document.getElementById("enemyHeroNameL").innerHTML = "HERO_NAME: " + heroName;
        document.getElementById("enemyHealthL").innerHTML = "HEALTH: " + health;
        document.getElementById("enemyCardsInHandL").innerHTML = "CARDS_IN_HAND: " + handAmount;
        $('#enemyBlueCards').empty();
        for (let i = 0; i < blueCards.length; i++) {
            $('#enemyBlueCards').append('<img id=\"blueCard' + i + '\"  class=\" col-2 mh-100 rounded\"></img>');
            setImage(blueCards[i], 'blueCard' + i);
        }
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
            //document.getElementById("heroPhoto").src = "~/Content/Images/heroes/blackjack.png";
            thyName("give me thy name(alphanumeric shorter than 11 chars)")
            

        })
        .fail(function () { alert("this doesn't seem to work, bitch"); });

    document.getElementById("gameStarter").onclick = function () { startGame();}; 

   

})()