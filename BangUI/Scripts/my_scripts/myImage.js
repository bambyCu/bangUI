class imageRequester {
    doneFuncion(f) {
        this.done = f;
    }

    load(image) {
        this.image = image;
        myHub.requestImage(image);
        this.waiter();
    }

    waiter() {
        
        if (myHub.cardImages[this.image] != undefined) {
            this.done();
            return;
        }
        if (myHub.cardImages[this.image] === undefined) {
            setTimeout(() => (this.waiter()), 50);
        }
        
    }

}