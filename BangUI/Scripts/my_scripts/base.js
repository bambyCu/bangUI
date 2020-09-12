
let myHub = new Hub($.connection.mainHub);
let hand = new CardsOnTable("myDiv");
const SALT = "theUser";


    
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



let setImage = function (cardType, elementId) {
    if (myHub.cardImages[cardType] === undefined) {
        myHub.requestImage(cardType);
        setTimeout(() => { document.getElementById(elementId).src = "data:image/png;base64," + myHub.cardImages[cardType]; }, 200);
        return;
    }
    document.getElementById(elementId).src = "data:image/png;base64," + myHub.cardImages[cardType];
}

let removeFromLogged = function (str) {
    document.getElementById(SALT + str).remove();
}

let addPhoto = function (parentId, newId, picture) {
    var newImage = document.createElement("img");
    newImage.id = newId;
    
    document.getElementById(parentId).appendChild(newImage);
    setImage(picture, newId);

}

// DONE ----------------------------------------------------------------------------------------------------------------------------------------

function sizeFy(size) {
    if (document.getElementById("d1").clientWidth == size) {
        return;
    }
    if (document.getElementById("d1").clientWidth < size) {
        document.getElementById("d1").style.width = (document.getElementById("d1").clientWidth + 1) + "px";
    }
    else if (document.getElementById("d1").clientWidth > size) {
        document.getElementById("d1").style.width = (document.getElementById("d1").clientWidth - 1) + "px";
    }
    console.log("hello");
    sizeFy(size);


}

function chg() {
    document.getElementById("d1").innerHTML = "Great Job!";
    document.getElementById("d1").style.width = "600px";
    document.getElementById("d1").style.height = "600px";
    document.getElementById("d1").style.background = "green";
}

function chg2() {
    document.getElementById("d1").innerHTML = "Hover Over Me!";
    document.getElementById("d1").style.width = "230px";
    document.getElementById("d1").style.height = "160px";
    document.getElementById("d1").style.background = "red";
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

window.onload = function () {
    window.addEventListener('resize', () => setTimeout(() => { hand.setUpCards(); }, 200));
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
        myHub.users.forEach(name => addToLogged(name));
        myHub.userLogIn("give me thy name(alphanumeric shorter than 11 chars)");
        setImage("bang", "heroPhoto");
        console.log(myHub.cardImages["bang"]);
        setTimeout(() => { hand.addImageElement("lala", myHub.cardImages["bang"]); }, 200);
        ;
    })
    .fail(function () { alert("this doesn't seem to work"); });