class modalMaster {
    constructor(modalWraperId, modalContentId, modalHeaderId) {
        this.modalId = modalWraperId;
        this.contendId = modalContentId;
        this.headerId = modalHeaderId;
    }

    showModal() {
        document.getElementById(this.modalId).style.display = "block";
    }

    hideModal() {
        document.getElementById(this.modalId).style.display = "none";
    }

    generateModal() {
        this.wrapper = document.createElement("div");
        this.wrapper.id = this.modalId;
        this.wrapper.classList.add("modal");
        this.header = document.createElement("div");
        this.header.id = this.headerId;
        this.header.classList.add("modal-header");
        this.content = document.createElement("div");
        this.content.id = this.contendId;
        this.content.classList.add("modal-content");
        this.wrapper.appendChild(this.header);
        this.wrapper.appendChild(this.content);
        return this.wrapper;
    }

    set headerText(text) {
        this.header.innerText = text;
    }

    set contentInner(cont) {
        this.content.innerHTML = cont;
    }

    set contentSet(cont) {
        this.content.innerHTML = '';
        this.content.appendChild(cont);
    }
}