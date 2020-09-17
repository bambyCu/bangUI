class Hub{
    constructor(hub){
        this.hub = hub;
        this.cardImages = {}
    }


    get users() {
        var papa = [];
        this.hub.server.getUsers()
            .done(function (data) {
                console.log("dfsdfdsfs")
                data.forEach(x => papa.push(x));
            })
        console.log(papa);
        return papa;
    }

    makeButton(id, text) {
        let butt = document.createElement("button");
        butt.id = id;
        butt.innerText = text;
        butt.addEventListener("click", function (ev) {
            if (ev.target.classList.contains("active"))
                ev.target.classList.remove("active")
            else
                ev.target.classList.add("active")
        })
        return butt;
    }

    seeLoged() {
        console.log("lapis ",this.users);
        myModal.showModal();
        let names = document.createElement("div");
        names.id = "loggend-names";
        let that = this;
        console.log("dddddx", this.users, this.users.length)
        this.users.forEach(x => console.log(x));
        this.users.forEach(x => {  names.appendChild(that.makeButton(x, x))});
        myModal.contentSet = names;
    }


    userLogIn() {
        myModal.headerText = "you need to login";
        myModal.contentInner =
            '<form id="farm" name="myForm" onsubmit="return false;" method="post">' + 
                'Name: <input type="text" name="fname">' +
                    '<input type="submit" value="Submit">'+
            '</form>'

        let inLog = document.getElementById("farm");
        let that = this;
        myModal.showModal();
        inLog.addEventListener("submit",
            function () {
                let login = document.forms["myForm"]["fname"].value;

                that.hub.server.logIn(login)
                    .done(
                        //return values are "correct", "incorrect", "taken"adf
                        function (val) {
                            if (val != "correct") {
                                console.log("name", login, "has not been selected");
                                myModal.headerText = (login + " is " + val);
                                return false;
                            }
                            console.log("name", login, "has been selected");
                            myModal.hideModal();
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

