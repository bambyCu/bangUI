
const myHub = new Hub($.connection.mainHub);
const hand = new CardsOnTable("handDiv");
const table = new CardsOnTable("tableDiv"); 
const enemyList = new enemies("enemyNames", "enemyDiv");
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
/*
let addToLogged = function (str) {
    $('#loggedList').append('<button id=\"' + SALT + str + '\" type=\"button\" class=\"list-group-item myBtn\">' + str + '</button>');
    document.getElementById(SALT + str).addEventListener("click", actAppl);
}
*/

let setMainPage = function (i) {
    enemyList.mainEnemyIndex = i;
}

let showMain = function(){
    enemyList.showMain();
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



    function allowDrop(ev) {
        ev.preventDefault();
    }

function drag(ev) {
        DRAGED = ev.target.id;
        //ev.dataTransfer.setData("text", );
    }

    function playCard(ev) {
        ev.preventDefault();
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

$.connection.hub.start({ waitForPageLoad: false })
    .done(function () {
        myHub.users.forEach(name => addToLogged(name));
        myHub.userLogIn("give me thy name(alphanumeric shorter than 11 chars)");
        var imageSrcs = ["bang", "missed", "KitCarlson", "ElGringo", "Joudonnais", "WillyTheKid"];
        setupImages(imageSrcs);
    })

function setupImages(srcs) {
    let img = new imageRequester();
    let remaining = srcs.length;
    for (var i = 0; i < srcs.length; i++) {
        img.doneFuncion(function () {
            --remaining;
            if (remaining <= 0) {
                siteSetup();
            }
        }
        )
        img.load(srcs[i])
    }
}

function siteSetup() {

    enemyList.addEnemy("gringo", "SHERIF", myHub.cardImages["ElGringo"], 3, 0, [], 5);
    enemyList.addEnemy("kit", "RENEGATE", myHub.cardImages["KitCarlson"], 4, 0, [], 5);
    enemyList.addEnemy("jou", "BANDIT", myHub.cardImages["Joudonnais"], 4, 1, [], 5);
    enemyList.addEnemy("willy", "BANDIT", myHub.cardImages["WillyTheKid"], 4, 1, [], 5);
    hand.addImageElement("lala", myHub.cardImages["bang"]);
    enemyList.setUpEnemyPages();
    enemyList.setUpNameButtons();
    setImage("bang", "userImage");
}
// then to call it, you would use this



window.onload = function () {
    window.addEventListener('resize', () => setTimeout(() => { hand.setUpCards(); }, 200));
    window.addEventListener('resize', () => setTimeout(() => { enemyList.setButtonsToSize(); }, 200));
    
    /*document.getElementById("discardPile").ondragover = function (event) {
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
    })*/
    //document.getElementById("gameStarter").onclick = function () { startGame(); };     
}

    
