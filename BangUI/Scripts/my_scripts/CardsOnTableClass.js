class CardsOnTable {
    constructor(idOfTableElement) {
        this.table = idOfTableElement;
        this.tableSpecs = document.getElementById(idOfTableElement).getBoundingClientRect();
        this.cards = [];
        this.salt = "theCard";
    }

    

    addImageElement(id, byteArray) {
        let newImage = document.createElement("img");
        this.cards.push(id);
        document.getElementById(this.table).appendChild(newImage);
        newImage.id = id;
        newImage.style.width = this.cardWidth + "px";
        newImage.style.height = this.cardHeight + "px";
        newImage.position = "absolute";
        document.getElementById(id).src = "data:image/png;base64," + byteArray;
        this.setUpCards(id);
    }

    showCard(cardId) {
        document.getElementById(cardId).style.bottom = this.cardHeight + "px";
        //this.cards.forEach(x => document.getElementById(x).style.marginBottom = (x == cardId) ? "100%" : "");
    }

    setUpCard(cardId) {
        document.getElementById(cardId).style.marginLeft = this.distanceBetweenCardsInPercent + "%";
    }

    setUpCards() {
        this.tableSpecs = document.getElementById(this.table).getBoundingClientRect();
        for (let i = 0; i < this.cards.length; i++) {
            if (this.distanceBetweenCardsInPercent < 0 && i == 0) {
                document.getElementById(this.cards[i]).style.marginLeft = 0;
                continue;
            }
            this.setUpCard(this.cards[i]);
        }
        
    }

    get distanceBetweenCardsInPercent() {
        let percentageTakenUpByImages = this.cardWidth * this.cards.length / this.tableSpecs.width * 100;
        let freeSpace = 100 - percentageTakenUpByImages;
        if (freeSpace < 0)  
            return  (freeSpace / (this.cards.length - 1)) - 0.05;
        return freeSpace / (this.cards.length + 1);
    }

    get cardWidth() {
        return (document.getElementById(this.table).clientHeight * 25 / 39);
    }

    get cardHeight() {
        return document.getElementById(this.table).clientHeight;
    }
}