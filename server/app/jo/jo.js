$(function () {

  // https://fxaeberhard.github.io/handdrawn.css/
  // https://wiredjs.com/



  const ip = "192.168.0.69";
  const port = 1818;
  const url = `ws://${ip}:${port}`;
  const ws = new WebSocket(url);

  let playerName = "";
  let playerColor = undefined;

  let roomScreen = $('.room-form');
  let waitScreen = $('.waiting-room'); waitScreen.hide();
  let writeScreen = $('.write-screen'); writeScreen.hide();

  let startButton = $('.start-button'); startButton.hide();
  let buttons = [
    $('.room-button'),
    $('.start-button'),
    $('.write-done-button')
  ];

  let secretWordText = $('.secret-word-text');
  

  ws.onerror = error => {
    console.error(`WebSocket error: ${error}`);
  }

  ws.onopen = () => {
    console.log(`Connection opened!`);
  }

  window.onbeforeunload = () => {
    ws.onclose = function() {};
    ws.close();
  }

  ws.onmessage = (e) => {
    let message = JSON.parse(e.data);
    console.log("received:", message);
    switch (message.messageType) {
      case 'SET_AS_FIRST_PLAYER':
        setAsFirstPlayer();
        break;
      case 'COLOR_ASSIGNED':
        setColor(message);
        break;
      case 'START_ROUND':
        startRound(message);
        break;
    }
  }


  // ROOM JOINING
  $('.room-button').on('click', () => {

    playerName = $('.name-input').val();
    if (playerName != "") {
      let joinRoomObject = {gameType: 'JO', messageType: 'JOIN_ROOM', name: playerName};
      ws.send(JSON.stringify(joinRoomObject));
      console.log("sending", joinRoomObject);
      roomScreen.hide();
      waitScreen.show();
    }
  });


  function setColor(message) {

    console.log("setting color", message);

    playerColor = message.color;
    for (let i = 0; i < buttons.length; i++) {
      buttons[i].css('background-color', playerColor.main);
    }
  }

  function setAsFirstPlayer() {
    console.log(`setting as first player`);
    startButton.show();
  }

  startButton.on('click', () => {
    let startGameObject = {gameType: 'JO', messageType: 'ALL_PLAYERS_READY'};
    ws.send(JSON.stringify(startGameObject));
    
  });


  function startRound(message) {
    
    startButton.hide();
    
    if (message.word) {
      waitScreen.hide();
      secretWordText.html(`The secret word is ${message.word}`);
      writeScreen.show();
    }
  }



  // WRITING
  $('.write-done-button').on('click', () => {
    let writeText = $('.write-input').val();
    if (writeText != "") {
      writeScreen.hide();
      waitScreen.show();
      let wordSubmittedObject = {gameType: 'JO', messageType: 'WORD_SUBMITTED', name: playerName, word: writeText };
      ws.send(JSON.stringify(wordSubmittedObject));
    }
  });

  


});
  