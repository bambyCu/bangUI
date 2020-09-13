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
        console.log(this.username);
        let div = document.createElement("div");
        div.id = this.username;
        div.appendChild(this.generateImage());
        div.appendChild(this.generateStatsTable());
        return div;

    }

    generateImage() {
        let div = document.createElement("div");

        let image = document.createElement("img");
        image.src = "data:image/png;base64," + this.image;
        div.id = this.username + "image";
        div.classList.add("heroImage");
        div.appendChild(image);
        return div;
    }

    generateStatsTable() {
        let list = document.createElement("table");
        list.appendChild(this.generateTrElement("NAME", this.username + "Name", this.username));
        list.appendChild(this.generateTrElement("DISTANCE", this.username + "Distance", this.distance));
        list.appendChild(this.generateTrElement("HEALTH", this.username + "Health", this.health));
        list.appendChild(this.generateTrElement("ROLE", this.username + "Role", this.role));
        list.appendChild(this.generateTrElement("CARDS_IN_HAND", this.username + "Hand", this.hand));
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
}