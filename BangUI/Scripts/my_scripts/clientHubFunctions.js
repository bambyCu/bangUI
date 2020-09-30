

$.connection.mainHub.client.invitation = function (team) {
    team.forEach(x => console.log(x));
    console.log("I have been invited with ", team)
    myHub.inviteModal.showModal();
    myHub.onlineUsersModal.hideModal();
}

$.connection.mainHub.client.addMessage = function (text) {
    document.getElementById("messages").innerHTML += text + "<br />";
}

$.connection.mainHub.client.logIn = function (text) {
    document.getElementById("messages").innerHTML += "\n" + " login "+  text;
}


$.connection.mainHub.client.addHandCards = function (listImageId) {
    //listImage [(image, string)]
    $('#playCards').empty();
    for (let i = 0; i < listImageId.length; i++) {
        myHub.client.addHandCard(listImageId[i].Item1, listImageId[i].Item2);
    }
}
let cici = 0;


$.connection.mainHub.client.beginGameInfo = function (listImageId) {
    //listImage [(image, string)]
    console.log(listImageId);
    cici = listImageId;
    myHub.inviteModal.hideModal();
    myHub.onlineUsersModal.hideModal();
    myHub.logInModal.hideModal();
    
    listImageId.forEach(x => {
        enemyList.addEnemy(x.Name, x.RoleType, myHub.cardImages[x.HeroType], x.Health, x.Distance, [], x.HandSize);
        console.log(x.HeroType);
    })
    enemyList.setUpEnemyPages();
    enemyList.setUpNameButtons();
    
}

$.connection.mainHub.client.setMeUp = function (myInfo) {
    myInfo.Hand.forEach(x => hand.addImageElement(x.Id, myHub.cardImages[x.CardType]));
    document.getElementById("userImage").src = "data:image/png;base64," + myHub.cardImages[myInfo.HeroType];
    document.getElementById("heroHealth").innerText = myInfo.Health;
    document.getElementById("heroRole").innerText = myInfo.RoleType;
    document.getElementById("heroName").innerText = myInfo.Name;
}

$.connection.mainHub.client.reloadHand = function (cardIdToImageName) {
    hand.removeAll();
    cardIdToImageName.forEach(x => hand.addImageElement(x.Id, myHub.cardImages[x.CardType]));
}

$.connection.mainHub.client.reloadTable = function (cardIdToImageName) {
    table.removeAll();
    cardIdToImageName.forEach(x => () => {
        if (!document.getElementById(x.Id))
            hand.removeCard(x.Id);
    });
    cardIdToImageName.forEach(x => table.addImageElement(x.Id, myHub.cardImages[x.CardType]));
}

$.connection.mainHub.client.setDeckSize = function (size) {
    document.getElementById("deckAmount").innerText = size;
}

$.connection.mainHub.client.setPileInfo = function (size, imageName) {
    document.getElementById("pileAmount").innerText = size;
    document.getElementById("pile").src = "data:image/png;base64," + myHub.cardImages[imageName];
}







