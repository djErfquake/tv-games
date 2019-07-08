/*

To run, use node --inspect index.js


*/


const express = require('express');
const app = express();
const path = require('path');
const server = require('http').createServer(app);
const port = process.env.PORT || 1818;

const WebSocket = require('ws');
//const wss = new WebSocket.Server({port: port});
const wss = new WebSocket.Server({server: server, maxReceivedFrameSize: 131072, maxReceivedMessageSize: 10 * 1024 * 1024,});




//////////////////////
// START SERVER
//////////////////////

app.use(express.static(path.join(__dirname, 'app')));

// serve these pages
app.get('/bpt', (req, res) => {
  res.status(200).sendFile(path.resolve(__dirname, '', 'app', 'bpt/bpt.html'));
});

app.get('/jo', (req, res) => {
  res.status(200).sendFile(path.resolve(__dirname, '', 'app', 'jo/jo.html'));
});

// start listening for requests
server.listen(port, () => { console.log(`Listening on port ${port}`); });


//////////////////////
// SETTINGS
//////////////////////

let gameHandlers = [];

// colors
let availableColors = [ // https://flatuicolors.com/palette/de
  { main: "#eb3b5a", secondary: "#fc5c65" },
  { main: "#fa8231", secondary: "#fd9644" },
  { main: "#f7b731", secondary: "#fed330" },
  { main: "#20bf6b", secondary: "#26de81" },
  { main: "#0fb9b1", secondary: "#2bcbba" },
  { main: "#2d98da", secondary: "#45aaf2" },
  { main: "#3867d6", secondary: "#4b7bec" },
  { main: "#8854d0", secondary: "#a55eea" }
];
function getRandomAvailableColor() { return availableColors.splice(Math.floor(Math.random()*availableColors.length), 1)[0]; }
function addToAvailableColors(color) { availableColors.push(color); }

// sockets
let bigScreenSocket = undefined;
let firstPlayerSocket = undefined;
let players = []; // array player's sockets, and other player data








//////////////////////
// START WEBSOCKET GAME SERVER
//////////////////////
wss.on('connection', ws => {

  console.log(`socket connected`);
  //ws.alive = true; ws.on('pong', () => { this.alive = true; });

  ws.on('message', messageData => {

    let message = JSON.parse(messageData);
    let gameType = message.gameType;
    //console.log('message', message);
    
    if (message.messageType == 'JOIN_ROOM') {
      joinServer(ws, message);
    } else {
      gameHandlers[gameType](ws, message);
    }

    //console.log("recieved message", message);
  });

  ws.on('close', () => {

    console.log(`socket closed.`);
    for (let i = 0; i < players.length; i++) {
      if (ws.name == players[i].name) {
        console.log(`${players[i].name} disconnected. ${players.length} players left.`);
        if (bigScreenSocket != undefined) {
          let playerLeftObject = { messageType: "PLAYER_LEFT", player: players[i].name };
          bigScreenSocket.send(JSON.stringify(playerLeftObject));
        }
        addToAvailableColors(players[i].color);
        players.splice(i, 1);
        break;
      }
    }

    // set new first player
    if (ws == firstPlayerSocket) {
      firstPlayerSocket = undefined;
      if (players.length > 0) { 
        firstPlayerSocket = players[0];
        let firstPlayerObject = { messageType: "SET_AS_FIRST_PLAYER" };
        firstPlayerSocket.send(JSON.stringify(firstPlayerObject));
        console.log(`now ${firstPlayerSocket.name} is the first player`);
      }
    }
    else if (ws == bigScreenSocket) {
      bigScreenSocket = undefined;
      console.log(`big screen disconnected`);
    }

  });
});


function joinServer(socket, messageData)
{
  if (messageData.name == "BIG_SCREEN") {

    bigScreenSocket = socket;
    console.log(`big screen joined.`);
    
    // update for all players in before big screen started
    for (let i = 0; i < players.length; i++) {
      let playerObject = { name: players[i].name, color: players[i].color };
      let joinObject = { messageType: "PLAYER_JOINED", player: playerObject };
      bigScreenSocket.send(JSON.stringify(joinObject));
    }

  } else {

    console.log(`player ${messageData.name} joined.`);
    
    // create player
    socket.name = messageData.name;
    socket.color = getRandomAvailableColor();
    socket.submitted = false;
    
    // tell big screen
    let joinObject = { messageType: "PLAYER_JOINED", player: { name: socket.name, color: socket.color } };
    if (bigScreenSocket != undefined) { bigScreenSocket.send(JSON.stringify(joinObject)); }
    
    // assign color to player
    let colorObject = { messageType: "COLOR_ASSIGNED", color: socket.color};
    socket.send(JSON.stringify(colorObject));
    
    // assign to be the first player if applicable
    if (firstPlayerSocket == undefined) { 
      firstPlayerSocket = socket; isFirstPlayer = true;
      let firstPlayerObject = { messageType: "SET_AS_FIRST_PLAYER" };
      socket.send(JSON.stringify(firstPlayerObject));
    }

    players.push(socket);
  }
}


// setInterval(function ping() {
//   wss.clients.forEach(function each(ws) {
//     if (ws.alive === false) {
//       if (ws === bigScreenSocket) { bigScreenSocket = undefined; console.log("Big screen left."); }
//       else { console.log(players[ws].name + " left."); delete players[ws]; }
//       return ws.terminate();
//     }
//     ws.alive = false;
//     ws.ping(() => {});
//   });
// }, 30000);




function bptHandler(socket, message) {
  switch (message.messageType) {
    case 'DRAWING_POINT':
      drawingPointAdded(socket, message.point);
      break;
  }
}
gameHandlers['BPT'] = bptHandler;



function drawingPointAdded(socket, point) {
  if (bigScreenSocket != undefined) {
    let drawingObject = { messageType: "DRAWING_POINT", player: socket.name, point: point };
    bigScreenSocket.send(JSON.stringify(drawingObject));
  }
}




/*
   ___           _     _____            
  |_  |         | |   |  _  |           
    | |_   _ ___| |_  | | | |_ __   ___ 
    | | | | / __| __| | | | | '_ \ / _ \
/\__/ / |_| \__ \ |_  \ \_/ / | | |  __/
\____/ \__,_|___/\__|  \___/|_| |_|\___|

 CLIENT                            SERVER                            BIG SCREEN
    +                                 +                                  +
    +-------------------------------->+                                  |
    |          GAME_STARTED           +--------------------------------->+
    |                                 |           GAME_STARTED           |
    |                                 |                                  |
    |                                 |                                  |
Main Game Loop                        |                                  |
+---+------------------------------------------------------------------------+
|   |                                 |                                  |   |
|   |                                 +<---------------------------------+   |
|   +<--------------------------------+             START_ROUND          |   |
|   |           START_ROUND           |                                  |   |
|   |                                 |                                  |   |
|   |                                 |                                  |   |
|   |                                 |                                  |   |
|   +-------------------------------->+                                  |   |
|   |          WORD_SUBMITTED         +--------------------------------->+   |
|   |                                 |          WORD_SUBMITTED          |   |
|   |                                 +<---------------------------------+   |
|   +<--------------------------------+          READY_TO_GUESS          |   |
|   |          READY_TO_GUESS         |                                  |   |
|   |                                 |                                  |   |
|   |                                 |                                  |   |
|   |                                 |                                  |   |
|   +-------------------------------->+                                  |   |
|   |              GUESS              +--------------------------------->+   |
|   |                                 |               GUESS              |   |
|   |                                 |                                  |   |
+----------------------------------------------------------------------------+
    |                                 |                                  |
    |                                 |                                  |
    |                                 |                                  |
    |                                 +<---------------------------------+
    +<--------------------------------+               DONE               |
    |               DONE              |                                  |
    |                                 |                                  |
    +                                 +                                  +


*/

function joHandler(socket, message) {
  switch (message.messageType) {
    case 'ALL_PLAYERS_READY':
      joAllPlayersReady();
      break;
    case 'START_ROUND':
      joRoundStarted(message);
      break;
    case 'WORD_SUBMITTED':
      joWordSubmitted(message);
      break;
    case 'READY_TO_GUESS':
      joReadyToGuess(message);
      break;
    case 'GUESS':
      joGuessSubmitted(message);
      break;
  }
}
gameHandlers['JO'] = joHandler;


function joAllPlayersReady() {
  if (bigScreenSocket != undefined && players.length >= 3) {
    let gameStartObject = { messageType: "START_ROUND" };
    bigScreenSocket.send(JSON.stringify(gameStartObject));
  }
}

function joRoundStarted(message) {

  let guessingPlayer = message.player;
  let guessingWord = message.word;

  for (let i = 0; i < players.length; i++) {

    if (players[i].name == guessingPlayer) {
      let jsonObject = { messageType: "START_ROUND" };
      players[i].send(JSON.stringify(jsonObject));
    } else {
      let jsonObject = { messageType: "START_ROUND", word: guessingWord };
      players[i].send(JSON.stringify(jsonObject));
    }
  }
}


function joWordSubmitted(message) {
  if (bigScreenSocket != undefined) {
    let wordSubmittedObject = { messageType: "WORD_SUBMITTED", name: message.name, word: message.word };
    bigScreenSocket.send(JSON.stringify(wordSubmittedObject));
  }
}

function joReadyToGuess(message) {
  let guessingPlayer = message.player;
  for (let i = 0; i < players.length; i++) {
    if (players[i].name == guessingPlayer) {
      let jsonObject = { messageType: "READY_TO_GUESS" };
      players[i].send(JSON.stringify(jsonObject));
      break;
    }
  }
}

function joGuessSubmitted(message) {
  if (bigScreenSocket != undefined) {
    let guessSubmittedObject = { messageType: "GUESS", word: message.word };
    bigScreenSocket.send(JSON.stringify(guessSubmittedObject));
  }
}