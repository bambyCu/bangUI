

$.connection.mainHub.client.invitation = function (team) {
    team.forEach(x => console.log(x));
    console.log("I have been invited with ", team)
    myHub.inviteModal.showModal();
    myHub.onlineUsersModal.hideModal();
}

$.connection.mainHub.client.addMessage = function (text) {
    document.getElementById("messages").innerHTML += "message" + text;
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


