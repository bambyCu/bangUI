class enemy {
    constructor(username, role, image, health, distance, cardsOnTable, hand) {
        this.username = username;
        this.role = role;
        this.image = image;
        this.health = health;
        this.distance = distance;
        this.cardsOnTable = cardsOnTable;
        this.hand = hand;
    }

    generateDiv() {
        let div = document.createElement("div");
        div.id = this.username;
        div.appendChild(this.generateImage());
        div.appendChild(this.generateStatsTable());
        div.appendChild(this.generateCardsTable());
        this.table = new CardsOnTable(this.idCardsOnTable, (x) => makeElementApplicableTo(x), "-card-" + this.username);
        this.cardsOnTable.forEach(x => this.table.addImageElement(x.id, x.image));
        return div;
    }

    generateCardsTable() {
        let cards = document.createElement("div");
        cards.id = this.idCardsOnTable;
        cards.classList.add("tableCards");
        return cards;
    }

    generateImage() {
        let div = document.createElement("div");
        let image = document.createElement("img");
        image.src = "data:image/png;base64," + this.image;
        image.id = this.idRoleImage;
        div.classList.add("centeredCard");
        makeElementApplicableTo(div);
        div.appendChild(image);
        return div;
    }

    generateStatsTable() {
        let list = document.createElement("table");
        list.appendChild(this.generateTrElement("NAME:", this.idName, this.username));
        list.appendChild(this.generateTrElement("DISTANCE:", this.idDistance, this.distance));
        list.appendChild(this.generateTrElement("HEALTH:", this.idHealth, this.health));
        list.appendChild(this.generateTrElement("ROLE:", this.idRole, this.role));
        list.appendChild(this.generateTrElement("CARDS_IN_HAND:", this.idCardsInHand + "Hand", this.hand));
        return list;
    }

    generateTrElement(textR1C1, idR2C1, textR2C2) {
        let trElem = document.createElement("tr");
        trElem.appendChild(this.generateTdElement("", textR1C1));
        trElem.appendChild(this.generateTdElement(idR2C1, textR2C2));
        return trElem;
    }

    generateTdElement(id, text) {
        let tdElem = document.createElement("td");
        tdElem.id = id;
        tdElem.innerText = text;
        return tdElem;
    }

    get idName() {
        return this.username + "-name";
    }

    get idDistance() {
        return this.username + "-distance";
    }

    get idHealth() {
        return this.username + "-health";
    }

    get idRole() {
        return this.username + "-role";
    }

    get idCardsInHand() {
        return this.username + "-hand";
    }

    get idCardsOnTable() {
        return this.username + "-cards"
    }

    get idRoleImage() {
        return this.username + "-roleImage"
    }
}