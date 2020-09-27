//this file is needed because client functions need to be set up before start of hub

$.connection.hub.start({ waitForPageLoad: false })
    .done(function () {
        myHub.userLogIn("give me thy name(alphanumeric shorter than 11 chars)");
        var imageSrcs = ["BartCassidy", "BlackJack", "CalamityJanet", "ElGringo", "Jessejones", "Joudonnais", "KitCarlson", "LuckyDuke", "PaulRegret", "PedroRamirez", "RoseDoolan", "SidKetchum", "SlabTheKiller", "SuzyLafayette",
            "VultureSam", "WillyTheKid", "bang", "missed", "backOfCard", "Bang", "Barel", "Beer", "Carabine", "CatBalou", "Diligenza", "Duel", "Dynamite","Emporio","Gatling", "Indians", "Mirino", "Missed", "Mustang", "Panic", "Prison", "emington", "Saloon", "Schofield","Volcanic","WellsFargo","Winchester"];
        setupImages(imageSrcs);
    })
