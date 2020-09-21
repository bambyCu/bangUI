//this file is needed because client functions need to be set up before start of hub

$.connection.hub.start({ waitForPageLoad: false })
    .done(function () {
        myHub.userLogIn("give me thy name(alphanumeric shorter than 11 chars)");
        var imageSrcs = ["bang", "missed", "KitCarlson", "ElGringo", "Joudonnais", "WillyTheKid", "backOfCard"];
        setupImages(imageSrcs);
    })
