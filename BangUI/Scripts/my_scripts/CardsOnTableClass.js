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
        newImage.style.position = "absolute";
        /*newImage.addEventListener("mouseover", function (event) {
            console.log(event.target);
            event.target.style.bottom = event.target.height + "px";
        }, false);
        newImage.addEventListener("mouseout", function (event) {
            console.log(event.target);
            event.target.style.bottom = 0 + "px";
        }, false);*/
        document.getElementById(id).src = "data:image/png;base64," + byteArray;
        this.setUpCards(id);
    }

    removeCard(id) {
        document.getElementById(id).remove();
        const index = this.cards.indexOf(id);
        if (index > -1) {
            this.cards.splice(index, 1);
        }
        this.setUpCards();
    }

    showCard(cardId) {
        document.getElementById(cardId).style.bottom = this.cardHeight + "px";
        //this.cards.forEach(x => document.getElementById(x).style.marginBottom = (x == cardId) ? "100%" : "");
    }

    setUpCards() {
        this.tableSpecs = document.getElementById(this.table).getBoundingClientRect();
        let cardMargin = 100 / (this.cards.length + 1);
        let cardSize = this.cardWidth / this.tableSpecs.width * 100 / 2;
        if (cardMargin < cardSize)
            cardMargin = 100 / (this.cards.length);
        for (let i = 0; i < this.cards.length; i++) {
            if (i == 0 && cardMargin < cardSize) {
                document.getElementById(this.cards[i]).style.marginLeft = 0;
                continue;
            }
            document.getElementById(this.cards[i]).style.marginLeft = (cardMargin * (i + 1)) - cardSize + "%";
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