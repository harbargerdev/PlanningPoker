"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/votehub").build();

// disable finalize button
document.getElementById("finishButton").disabled = true;

connection.on("VotingStatus", function (gameId, status) {
    var currentGame = document.getElementById("gameIdTextBox").value;
    if (gameId === currentGame && status === "complete") {
        document.getElementById("finishButton").disabled = false;
    }
});

connection.start().then(function () {
    setTimeout(() => { document.getElementById("finishButton").disabled = false; }, 300000);
    var cardNumber = document.getElementById('cardNumberTxtBox').value;
    if (cardNumber !== "") {
        var currentGame = document.getElementById('gameIdTextBox').value;
        connection.invoke("SendMessage", currentGame, cardNumber).catch(function (err) {
            return console.error(err.toString);
        });
    }
});