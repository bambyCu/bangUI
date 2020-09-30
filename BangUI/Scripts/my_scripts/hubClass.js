class Hub{
    constructor(hub, logInModal, onlineUsersModal, inviteModal){
        this.hub = hub;
        this.cardImages = {}
        this.logInModal = logInModal;
        this.onlineUsersModal = onlineUsersModal;
        this.inviteModal = inviteModal;
    }

    //this.hub.server.getUsers()

    

    seeLoged() {
        this.onlineUsersModal.showModal();
        let names = document.createElement("div");
        names.id = "loggend-names";
        this.hub.server.getUsers()
            .done(function (data) {
                data.forEach(x => {
                    let tempButton = createElement("button", x, x);
                    tempButton.addEventListener("click", function (ev) {
                        if (ev.target.classList.contains("active"))
                            ev.target.classList.remove("active");
                        else
                            ev.target.classList.add("active");
                    });
                    names.appendChild(tempButton)
                });
            }
        );
        let startButton = createElement("button", "start-game", "start game", [])
        const that = this;
        startButton.addEventListener("click",
            () => {
                let names = [...document.getElementsByClassName("active")].map(x => x.id);
                that.hub.server.gameInvitation(names).done(function (input) {
                    if (!input) {
                        console.log("not permited action");
                    }
                    console.log("invitation request notices", input);
                })
            });
        startButton.style.position = "absolute";
        startButton.style.bottom = 0;
        startButton.style.left = 0;
        names.appendChild(startButton);
        this.onlineUsersModal.contentSet = names;
    }


    userLogIn() {

        this.logInModal.contentInner =
            '<form id="farm" name="myForm" onsubmit="return false;" method="post">' + 
                'Name: <input id="loginText" type="text" name="fname">' +
                    '<input type="submit" value="Submit">'+
            '</form>'
        this.logInModal.headerText = "you need to login";
        document.getElementById("loginText").focus();
        document.getElementById("loginText").select();
        let inLog = document.getElementById("farm");
        let that = this;
        this.logInModal.showModal();
        inLog.addEventListener("submit",
            function () {
                let login = document.forms["myForm"]["fname"].value;
                that.hub.server.logIn(login)
                    .done(
                        //return values are "correct", "incorrect", "taken"adf
                        function (val) {
                            if (val != "correct") {
                                that.logInModal.headerText = (login + " is " + val);
                                return false;
                            }
                            that.logInModal.hideModal();
                            that.seeLoged();
                            return false;
                        })
                return false;
                
            });
        
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

    cardApplied(cardId, dropId) {
        cardId = cardId.split("-")[0];
        let myName = document.getElementById("heroName").innerText;
        if (dropId == "userImage") {
            this.hub.server.appliedCardToUser(cardId, document.getElementById("heroName").innerText);
            //console.log("card used on me " + cardId + " " + myName);
            return;
        }
        if (dropId == "pile") {
            this.hub.server.discard(cardId);
            //console.log("discard", cardId);
            return;
        }
        let splitId = dropId.split("-");
        if (splitId.length == 3 && splitId[1] == "card") {
            //cardId, toCardId, victimUsername
            if (splitId[2] == "this")
                splitId[2] = myName;
            this.hub.server.appliedCardToCard(cardId, splitId[0], splitId[2]);
            //console.log("card " + cardId + " has been applied to card " + splitId[0] + " of " + splitId[2]);
        }
        if (splitId.length == 2 && splitId[1] == "roleImage") {

            this.hub.server.appliedCardToUser(cardId, splitId[0]);
            //console.log("card " +  cardId + " used on " + splitId[0]);
        }
        
    }

    gameStarter() {
        myHub.server.newGameSetUp()
            .done(function (val ) {
            });
    }

}

