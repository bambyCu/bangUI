

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

