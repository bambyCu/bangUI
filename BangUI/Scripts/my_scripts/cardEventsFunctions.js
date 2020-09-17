
function makeElementByIdApplicableTo(id) {
    makeElementApplicableTo(document.getElementById(id));
}

function makeElementByIdPlayable(id) {
    makeElementPlayable(document.getElementById(id));
}

function makeElementApplicableTo(elem) {
    elem.addEventListener("drop", applyCardEvent);
    elem.addEventListener("dragover", allowPlayCardEvent);
}

function makeElementPlayable(elem) {
    elem.addEventListener("dragstart", beginPlayingCardEvent);
    elem.setAttribute('draggable', true);
}

function allowPlayCardEvent(ev) {
    ev.preventDefault();
}

function beginPlayingCardEvent(inputEvent) {
    inputEvent.dataTransfer.setData("text", inputEvent.target.id);
}

function applyCardEvent(inputEvent) {
    var data = inputEvent.dataTransfer.getData("text");
    console.log("started on ", data, "droped on", inputEvent.target.id);

    //ev.target.appendChild(document.getElementById(data));
}
