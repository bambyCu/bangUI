class enemies {
    constructor(buttonDiv, pageDiv) {
        this.enemyList = []
        this.buttonsId = buttonDiv;
        this.pagesId = pageDiv; 
        this.mainEnemyIndex = 0; 
    }

    addEnemy(username, role, image, health, distance, cardsOnTable, hand) {
        this.enemyList.push(new enemy(username, role, image, health, distance, cardsOnTable, hand));
    }

    setUpEnemyPages() {
        let enemyPage = document.getElementById(this.pagesId);
        for (let i = 0; i < this.enemyList.length; i++) {
            let temp = this.enemyList[i].generateDiv();
            temp.style.display = "none";
            enemyPage.appendChild(temp);
        }
    }

    setUpNameButtons() {
        let butts = document.getElementById(this.buttonsId);
        for (let i = 0; i < this.enemyList.length; i++) {
            let butt = this.createButton(this.getButtonId(i), this.enemyList[i].username);
            butt.addEventListener("click", function () { setMainPage(i), showMain() }, false);
            butts.appendChild(butt);
        }
    }

    setButtonsToSize() {
        for (let i = 0; i < this.enemyList.length; i++) {
            document.getElementById(this.getButtonId(i)).style.height = document.getElementById(this.buttonsId).offsetHeight / this.enemyList.length + "px";
        }
    }
    
    createButton(id, text) {
        document.getElementById(this.buttonDiv);
        let butt = document.createElement("p");
        butt.id = id;
        butt.classList.add("enemyButton");
        butt.innerText = text;
        let amount = this.enemyList.length;
        butt.style.height = document.getElementById(this.buttonsId).offsetHeight / this.enemyList.length + "px";
        document.getElementById(this.buttonsId).appendChild(butt);
        return butt;
    }

    showMain() {
        this.enemyList.forEach(x => document.getElementById(x.username).style.display = "none");
        document.getElementById(this.enemyList[this.mainEnemyIndex].username).style.display = "block";
    }

    getButtonId(i) {
        return this.enemyList[i].username + "NameTag";
    }
}


