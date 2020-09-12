let hand = []
let a = 0
let move = 1


function preload() {
	hand[0] = loadImage("~/Models/Images/Heroes/bartcassidy.png");
}

function setup() {
	createCanvas(400, 400);
	background(200);
}

function draw() {
	a += move;
	if (a >= 255 || a <= 0)
		move = -move;

	background(a);
	ball(mouseX, mouseY, 100, 20);
}

function ball(x, y, size, dep) {

	if (5 >= size)
		return
	fill(255 / dep);

	ellipse(x, y, size, size);
	ball(x, y, size - dep, dep);
}