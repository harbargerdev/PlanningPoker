"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/votehub").build();

connection.on("VotingStatus", function (gameId, playerId) {
    var currentGame = document.getElementById('gameIdTextBox').value;
    if (gameId === currentGame) {
        var player = document.getElementById(playerId);
        player.style.display = "none";
    }
});

connection.start().then(function () {
    var cardNumber = document.getElementById('cardNumberTxtBox').value;
    if (cardNumber !== "") {
        var currentGame = document.getElementById('gameIdTextBox').value;
        connection.invoke("SendMessage", currentGame, cardNumber).catch(function (err) {
            return console.error(err.toString);
        });
    }
});