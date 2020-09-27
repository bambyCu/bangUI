
const myHub = new Hub($.connection.mainHub,
    new modalMaster("login-modal", "login-modal-content", "login-modal-header"),//needs to be set up in window.onload
    new modalMaster("users-modal", "users-modal-content", "users-modal-header"),//needs to be set up in window.onload
    new modalMaster("invite-modal", "invite-modal-content", "invite-modal-header")//needs to be set up in window.onload
);
const hand = new CardsOnTable("handDiv", (x) => {
    makeElementPlayable(x); makeElementApplicableTo(x);
});
const table = new CardsOnTable("tableDiv", (x) => makeElementApplicableTo(x)); 
const enemyList = new enemies("enemyNames", "enemyDiv");
const myModal = new modalMaster("bangModal", "modalContent", "modalHeader");





let setImage = function (cardType, elementId) {
    if (myHub.cardImages[cardType] === undefined) {
        myHub.requestImage(cardType);
        setTimeout(() => { document.getElementById(elementId).src = "data:image/png;base64," + myHub.cardImages[cardType]; }, 200);
        return;
    }
    document.getElementById(elementId).src = "data:image/png;base64," + myHub.cardImages[cardType];
}

// Setup functions ---------------------------------------------------------------------------------------------------------------------------------------


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

function createElement(elementType, id, text) {
    let butt = document.createElement(elementType);
    butt.id = id;
    butt.innerText = text;
    return butt;
}

function siteSetup() {
    //------------------------temporary testing values -------------------------------
    //enemyList.addEnemy("gringo", "SHERIF", myHub.cardImages["ElGringo"], 3, 0, [], 5);
    //enemyList.addEnemy("kit", "RENEGATE", myHub.cardImages["KitCarlson"], 4, 0, [], 5);
    //enemyList.addEnemy("jou", "BANDIT", myHub.cardImages["Joudonnais"], 4, 1, [], 5);
   // enemyList.addEnemy("willy", "BANDIT", myHub.cardImages["WillyTheKid"], 4, 1, [], 5);
    //hand.addImageElement("lala", myHub.cardImages["bang"]);
    //hand.addImageElement("secondLala", myHub.cardImages["missed"]);
    //-----------------------seting up elements of site-----------------------
    //enemyList.setUpEnemyPages(); // set up enemies
    //enemyList.setUpNameButtons(); // set up way to get to enemy panels
    makeElementByIdApplicableTo("pile"); // set up discard pile
    makeElementByIdApplicableTo("userImage"); // for self applicable cards
    
}

window.onload = function () {
    document.getElementsByTagName("BODY")[0].appendChild(myHub.logInModal.generateModal());
    document.getElementsByTagName("BODY")[0].appendChild(myHub.onlineUsersModal.generateModal());
    document.getElementsByTagName("BODY")[0].appendChild(myHub.inviteModal.generateModal());
    myHub.userLogIn();
    window.addEventListener('resize', () => setTimeout(() => { hand.setUpCards(); }, 200));
    window.addEventListener('resize', () => setTimeout(() => { enemyList.setButtonsToSize(); }, 200));
    window.addEventListener('resize', () => setTimeout(() => { enemyList.enemyList.forEach( x =>x.table.setUpCards()); }, 200));  
}

    
