class Hub{
    constructor(hub){
        this.hub = hub;
        this.cardImages = {}
    }

    
    userLogIn(str) {
        let userName = prompt(str, "");
        let temp = this.userLogIn;
        //return values are "correct", "incorrect", "taken"adf
        this.hub.server.logIn(userName)
            .done(
                function (val) {
                    if (val != "correct") {
                        temp(userName + " is " + val);
                    }
                })
            .fail(function (val) { alert("something happened"); })
    }

    get users() {
        let temp = [];
        this.hub.server.getUsers()
            .done(function (data) {
                temp = data;
            })
            .fail(function (e) {
                alert("there seems to be a problem" + e);
                temp = [];
            });
        return temp;
    }


    requestImage(typeImage) {
        let tt = this.cardImages;
        this.hub.server.typeImageByteStream(typeImage).
            done(function (returned) {
                tt[typeImage] = returned;
            });
    }

    startGame() {
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

    inviteRefuse(group) {
        myHub.server.getInVal(group, false).done(function () {
            console.log("invitation has not been accepted");
        });
    }

    inviteAccept(group) {
        myHub.server.getInVal(group, true).done(function () {
            console.log("invitation has  been accepted");
        });
    }

}

