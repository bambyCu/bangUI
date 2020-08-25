
let cardImages = {};
let myHub = $.connection.mainHub;
const SALT = "theUser";

let userLogIn = function (str) {
    let userName = prompt(str, "");
    //return values are "correct", "incorrect", "taken"
    myHub.server.logIn(userName)
        .done(
            function (val) {
                if (val != "correct") {
                    USERNAME = userLogIn;
                    userLogIn(userName + " is " + val);
                }

        })
        .fail(function(val) { alert("something happened"); })
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

let requestImage = function (typeImage) {
    myHub.server.typeImageByteStream(typeImage).
        done(function (returned) { cardImages[typeImage] = returned; });
}

let setImage = function (cardType, elementId) {
    if (cardImages[cardType] === undefined) {
        requestImage(cardType);
        setTimeout(() => { document.getElementById(elementId).src = "data:image/png;base64," + cardImages[cardType]; }, 200);
        return;
    }
    document.getElementById(elementId).src = "data:image/png;base64," + cardImages[cardType];
}

let removeFromLogged = function (str) {
    document.getElementById(SALT + str).remove();
}

// DONE ----------------------------------------------------------------------------------------------------------------------------------------



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

myHub.client.logIn = function (user) {
    addToLogged(user);
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

myHub.client.invitation = function (team) {
    $("#modalBody").text("does thou wish to play with " + team + " ?");
    $("#modalInvite").modal();
    document.getElementById("invBtnYes").addEventListener("click", function () { inviteAccept(); });
    document.getElementById("invBtnNo").addEventListener("click", function () { inviteRefuse(); });
}

    let addToMessageList = function (message) {
        messageDiv = document.getElementById("messageList");
        messageDiv.innerHTML += ("<br>" + message);
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


    myHub.client.disconnect = function (user) {
        removeFromLogged(user);
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



    window.onload = function () {
        document.getElementById("discardPile").ondragover = function (event) {
            allowDrop(event);
        };
        document.getElementById("discardPile").ondrop = function (event) {
            event.preventDefault();
            myHub.server.discard(DRAGED)
                .done(function () {
                    document.getElementById(DRAGED).remove();
                })
                .fail(function () { });
        };
        document.getElementById("btnEndTurn").addEventListener("click", function () {
            myHub.server.endTurn();
        })
        document.getElementById("gameStarter").onclick = function () { startGame(); }; 
        
        
        
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
        userLogIn("give me thy name(alphanumeric shorter than 11 chars)");
        setImage("bang", "heroPhoto");
    })
    .fail(function () { alert("this doesn't seem to work"); });