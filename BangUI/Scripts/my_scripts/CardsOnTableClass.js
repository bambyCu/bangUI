class CardsOnTable {
    constructor(idOfTableElement, massFunction, salt) {
        this.tableId = idOfTableElement;
        this.inFunction = massFunction;
        this.cards = [];
        this.salt = salt;
    }

    

    addImageElement(id, byteArray) {
        id += this.salt;
        let newImage = document.createElement("img");
        this.cards.push(id);
        document.getElementById(this.tableId).appendChild(newImage);
        newImage.id = id;
        newImage.style.width = this.cardWidth + "px";
        newImage.style.height = this.cardHeight + "px";
        newImage.style.position = "absolute";
        this.inFunction(newImage);
        document.getElementById(id).src = "data:image/png;base64," + byteArray;
        this.setUpCards();
    }

    addEventListenerToCards(ev, funct) {
        this.cards.forEach(x => this.addEventListenerToCard(x, ev, funct));
    }

    addEventListenerToCard(id, ev, funct) {
        document.getElementById(id).addEventListener(ev, funct);
    }

    removeCard(id) {
        document.getElementById(id).remove();
        const index = this.cards.indexOf(id);
        if (index > -1) {
            this.cards.splice(index, 1);
        }
        this.setUpCards();
    }

    removeAll() {
        for (let i = this.cards.length-1; i >= 0; i--) {
            document.getElementById(this.cards[i]).remove();
        }
        this.cards = [];
    }

    setUpCards() {
        let tableElem = document.getElementById(this.tableId);
        if (tableElem === undefined) {
            return;
        }
        this.tableSpecs = tableElem.getBoundingClientRect();
        let cardMargin = 100 / (this.cards.length + 1);
        let cardSize = this.cardWidth / this.tableSpecs.width * 100 / 2;
        if (cardMargin < cardSize)
            cardMargin = 100 / (this.cards.length);
        for (let i = 0; i < this.cards.length; i++) {
            if (i == 0 && cardMargin < cardSize) {
                document.getElementById(this.cards[i]).style.marginLeft = 0;
                continue;
            }
            let currCard = document.getElementById(this.cards[i])
            currCard.style.width = this.cardWidth + "px";
            currCard.style.height = this.cardHeight + "px";
            currCard.style.marginLeft = (cardMargin * (i + 1)) - cardSize + "%";
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
        return (document.getElementById(this.tableId).clientHeight * 25 / 39);
    }

    get cardHeight() {
        return document.getElementById(this.tableId).clientHeight;
    }
}