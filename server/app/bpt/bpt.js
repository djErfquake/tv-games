$(function () {

  const ip = "192.168.0.69";
  const port = 1818;
  const url = `ws://${ip}:${port}`;
  const ws = new WebSocket(url);

  let playerColor = undefined;

  let roomScreen = $('.room-form');
  let waitScreen = $('.waiting-room'); waitScreen.hide();
  let writeScreen = $('.write-round'); writeScreen.hide();
  let drawScreen = $('.draw-round'); drawScreen.hide();

  let startButton = $('.start-button'); startButton.hide();
  let buttons = [
    $('.room-button'),
    $('.start-button'),
    $('.write-done-button'),
    $('.draw-done-button')
  ];

  let canvas = $('#draw-canvas')[0];
  let canvasContext = canvas.getContext("2d");
  let canvasSize = { width: canvasContext.canvas.width, height: canvasContext.canvas.height };
  let paint = false;
  let drawings = [];
  let drawingIndex = -1;
  redrawCanvas();
  

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
    }
  }


  // ROOM JOINING
  $('.room-button').on('click', () => {

    let name = $('.name-input').val();
    if (name != "") {
      let joinRoomObject = {gameType: 'BPT', messageType: 'JOIN_ROOM', name: name};
      ws.send(JSON.stringify(joinRoomObject));
      console.log("sending", joinRoomObject);
      roomScreen.hide();
      waitScreen.show();
    }
  });


  function setColor(message) {

    console.log("setting color", message);

    playerColor = message.color;
    canvasContext.strokeStyle = playerColor.main;
    for (let i = 0; i < buttons.length; i++) {
      buttons[i].css('background-color', playerColor.secondary);
    }
  }

  function setAsFirstPlayer() {
    console.log(`setting as first player`);
    startButton.show();
  }



  // WRITING
  $('.write-done-button').on('click', () => {
    let writeText = $('.write-input').val();
    if (writeText != "") {

    }
  });



  // DRAWING
  $('.draw-done-button').on('click', () => {
    if (drawings.length != 0) {

    }
  });


  $('#draw-canvas').on('pointerdown', (e) => {

    let mouseX = e.pageX - canvas.offsetLeft;
    let mouseY = e.pageY - canvas.offsetTop;

    drawings.push([]);
    drawingIndex++;

    paint = true;

    addCanvasDot(mouseX, mouseY, false);
  });

  $('#draw-canvas').on('pointerup', (e) => {
    paint = false;
  });

  $('#draw-canvas').on('pointereave', (e) => {
    paint = false;
  });

  $('#draw-canvas').on('pointerenter', (e) => {
    paint = false;
  });

  $('#draw-canvas').on('pointermove', (e) => {

    e.preventDefault();

    let mouseX = e.pageX - canvas.offsetLeft;
    let mouseY = e.pageY - canvas.offsetTop;

    if (paint) { addCanvasDot(mouseX, mouseY, true); }

  });


  function addCanvasDot(x, y, dragging) {
    let newPoint = {x: x, y: y, dragging: dragging};
    drawings[drawingIndex].push(newPoint);
    redrawCanvas();
    
    let drawingObject = {gameType: 'BPT', messageType: 'DRAWING_POINT', point: newPoint};
    ws.send(JSON.stringify(drawingObject));
  }

  function redrawCanvas() {
    canvasContext.clearRect(0, 0, canvasSize.width, canvasSize.height);

    canvasContext.fillStyle = "#fff";
    canvasContext.rect(0, 0, canvasSize.width, canvasSize.height);
    canvasContext.fill();

    canvasContext.lineJoin = "round";
    canvasContext.lineWidth = 5;

    for (let i = 0; i < drawings.length; i++) {
      for (let j = 0; j < drawings[i].length; j++) {
        canvasContext.beginPath();
        if (drawings[i][j].dragging && j > 0) {
          canvasContext.moveTo(drawings[i][j - 1].x, drawings[i][j - 1].y);
        } else {
          canvasContext.moveTo(drawings[i][j].x - 1, drawings[i][j].y);
        }
        canvasContext.lineTo(drawings[i][j].x, drawings[i][j].y);
        canvasContext.closePath();
        canvasContext.stroke();
      }
    }
  }


});
