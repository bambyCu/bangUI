{
    let myHub = $.connection.mainHub
    myHub.client.invitation = function (team) {
        $("#modalBody").text("does thou wish to play with " + team + " ?");
        $("#modalInvite").modal();
        document.getElementById("invBtnYes").addEventListener("click", function () { inviteAccept(); });
        document.getElementById("invBtnNo").addEventListener("click", function () { inviteRefuse(); });
    }
    /*
    myHub.client.logIn = function (user) {
        addToLogged(user);
    }

    myHub.client.disconnect = function (user) {
        removeFromLogged(user);
    }
    */

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
}
