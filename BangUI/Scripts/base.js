(function () {
    let USERNAME = "";
    let SALT = "name:";
    let myHub = $.connection.mainHub;
    let ACTIVEBUTTONCLASS = "active"
    let GAMEID = "";
    let DRAGED = "";
    let ENEMYSALT = "enem"

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

    let addToMessageList = function (message) {
        document.getElementById("messageList").innerHTML += ("<br>" + message);
    }

    function allowDrop(ev) {
        ev.preventDefault();
    }

    function drag(ev) {
        console.log("I drag now " + ev.target.id);
        DRAGED = ev.target.id;
        //ev.dataTransfer.setData("text", );
    }

    function playCard(ev) {
        ev.preventDefault();
        //console.log("I drop now " + DRAGED + " on " + ev.currentTarget.id);
        myHub.server.applyGameIdCardTo(GAMEID, DRAGED, (ev.currentTarget.id).split("-")[1])
            .done(function (val) {
                if (val) {
                    //document.getElementById(DRAGED).remove();
                }
                else {
                    alert("incorrect use of card, or it is not yourturn");
                }
            })
            .fail(function () { alert("it seems there is server problem") });
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
        document.getElementById("invBtnYes").addEventListener("click", function () { inviteAccept(group); });
        document.getElementById("invBtnNo").addEventListener("click", function () { inviteRefuse(group); });
    }

    myHub.client.attacked = function (group, team) {
        $("#attackModalBody").text("does thou wish to play with " + team + " ?");
        $("#attackModal").modal();
        document.getElementById("invBtnYes").addEventListener("click", function () {
            myHub.server.block();
        });
        document.getElementById("invBtnNo").addEventListener("click", function () {
                
        });
    }

    myHub.client.invitationRefused = function (names) {
        $("#exampleModal").modal('hide');
    }

    myHub.client.addToMessageList = function (message) {
        addToMessageList(message)
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

    myHub.client.addHandCards = function (listImageId) {
        //listImage [(image, string)]
        $('#playCards').empty();
        for (let i = 0; i < listImageId.length; i++) {
            myHub.client.addHandCard(listImageId[i].Item1, listImageId[i].Item2);
        }
    }

    myHub.client.addEnemy = function (name, health, role) {
        let nameId = ENEMYSALT + "-" + name;
        $('#enemies').append(
            "<div class=\"list-group h-100 col-2 overflow-hidden\">"
            + "<div class=\"list-group-item d-flex justify-content-center overflow-hidden\">NAME:" + name
            + "</div> <div class=\"list-group-item d-flex justify-content-center overflow-hidden\">HEALTH:" + health
            + "</div > <div class=\"list-group-item d-flex justify-content-center overflow-hidden\">ROLE:" + role + "</div>"
            + '<div id="' + nameId + '" class="' + ENEMYSALT + ' border dark w-100 h-100" style="z - index: 3; top: 0; left: 0; position: absolute; opacity: .4;">'
            + "</div >");
        document.getElementById(nameId).ondragover = function (event) {
            allowDrop(event);
        };
        document.getElementById(nameId).ondrop = function (event) {
            playCard(event);
        };
        document.getElementById(nameId).addEventListener("mouseover", function () {
            myHub.server.getInfoUser(GAMEID, name)
                .done(function () { })
                .fail(function () { alert("sorry but connection is not great")});
        });
    }

    myHub.client.addEnemies = function (enemyList) {
        //enemyList [(name, health, role)]
        $('#enemies').empty();
        for (let i = 0; i < enemyList.length; i++) {
            myHub.client.addEnemy(enemyList[i].Item1, enemyList[i].Item2, enemyList[i].Item3);
        }
    }

    myHub.client.setGameId = function (str) {
        GAMEID = str
    }

    myHub.client.setCurrPlayer = function (name) {
        let temp = document.getElementsByClassName(ENEMYSALT);
        for (let i = 0; i < temp.length; i++) {
            temp[i].classList.remove("bg-danger")
        }

        console.log("nae is : " + ENEMYSALT + "-" + name);
        document.getElementById(ENEMYSALT + "-" + name).className += " " + "bg-danger"; 
    }

    myHub.client.message = function (str) {
        alert(str);
    }

    myHub.client.setGameView = function ( cardByteArray64, name, heroName, health, blueCards, handAmount,){
        setImage(cardByteArray64, "enemyPhoto");
        document.getElementById("enemyNameL").innerHTML = "NAME: " + name;
        document.getElementById("enemyHeroNameL").innerHTML = "HERO_NAME: " + heroName;
        document.getElementById("enemyHealthL").innerHTML = "HEALTH: " + health;
        document.getElementById("enemyCardsInHandL").innerHTML = "CARDS_IN_HAND: " + handAmount;
        $('#enemyBlueCards').empty();
        for (let i = 0; i < blueCards.length; i++) {
            let elemId = ENEMYSALT + "-" + blueCards[i].Item2;
            $('#enemyBlueCards').append('<img id=\"' + elemId + '\"  class=\" col-2 mh-100 rounded\"></img>');

            setImage(blueCards[i].Item1, elemId);
            
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
            document.getElementById("discardPile").ondragover = function (event) {
                allowDrop(event);
            };
            document.getElementById("discardPile").ondrop = function (event) {
                event.preventDefault();
                myHub.server.discard(DRAGED)
                    .done(function () {
                        //document.getElementById(DRAGED).remove();
                    })
                    .fail(function () { });
            };
            document.getElementById("btnEndTurn").addEventListener("click", function () {
                myHub.server.endTurn();
            })
        })
        .fail(function () { alert("this doesn't seem to work, bitch"); });

    document.getElementById("gameStarter").onclick = function () { startGame();}; 

   

})()