﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/votehub").build();

// disable finalize button
document.getElementById("finishButton").disabled = true;

connection.on("VotingStatus", function (gameId, status) {
    var currentGame = document.getElementById("gameIdTextBox").nodeValue;
    if (gameId === currentGame && status === "complete") {
        document.getElementById("finishButton").disabled = false;
    }
});