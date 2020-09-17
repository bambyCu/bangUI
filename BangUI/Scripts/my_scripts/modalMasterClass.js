class modalMaster {
    constructor(modalParentId, modalContentId, modalHeaderId) {
        this.modalId = modalParentId;
        this.table = modalContentId;
        this.header = modalHeaderId;
    }

    showModal() {
        document.getElementById(this.modalId).style.display = "block";
    }

    hideModal() {
        document.getElementById(this.modalId).style.display = "none";
    }

    set headerText(text) {
        document.getElementById(this.header).innerText = text;
    }

    set contentInner(cont) {
        let elem = document.getElementById(this.table);
        elem.innerHTML = cont;
    }

    set contentSet(cont) {
        let elem = document.getElementById(this.table);
        elem.innerHTML = '';
        elem.appendChild(cont);
    }
}