//set URL here
//eg: http://localhost:5000
const url = 'https://connect4-api.azurewebsites.net';

const statElem = document.getElementById('status');
const player = document.getElementById('player');

let currStat = '';
let currPlayer = 'none';
let currID = 1;
let poll = -1;

//try to join room
fetch(`${url}/api/Game/Play/StartGame`, {
  method: 'POST',
  headers: {
    'Content-Type': 'text/plain',
  }
})
  .then((response) => 
    response.json()
  )
  .then((data) => {
    assignPlayer(data.playerType);
    updateGame(data);
    //poll server every 5 seconds
    poll = setInterval(requestGameUpdate(), 5000);
  })
  .catch((error) => {
    console.error('Something went wrong when joining the game: ', error);
  });

//assign player type
function assignPlayer(playerType) {

  currPlayer = playerType;
  player.className = '';
  player.classList.add(playerType);

  if (playerType === 'red') {
    player.innerHTML = `You are playing as Red.`;
  }
  if (playerType === 'black') {
    player.innerHTML = `You are playing as Black.`;
  }
  if (playerType === 'spectator') {
    player.innerHTML = `You are spectating this match.`;
  }
}

//let cookie = '';

/*
function updateRoomList() {

  fetch('/roomlist').then((res) => {
    if (!response.ok) {
      throw new Error(`Something went wrong!`);
    }

    let roomSelector = document.getElementById('roomlist');

    for (const i of res.roomlist) {
      //populate roomlist
	  if (!i.isFull) {
        let item = document.createElement('li');
        item.innerHTML = i.id + `<button onclick="joinRoom(${i.id})">Join</button>`;
        roomSelector.appendChild(item);
      } else {
        let item = document.createElement('li');
        item.innerHTML = i.id + `<button disabled>Room Full!</button>`;
        roomSelector.appendChild(item);
      }
    }
  });
}


function joinRoom(roomName) {
  window.location.href = `/${roomName}`;
}



function leaveRoom() {}

*/

//requests an updated game state from the server
function requestGameUpdate() {
  fetch(`${url}/api/Game/${currID}`)
    .then((response) => 
      response.json()
    )
    .then((data) => {
      currID = data.id;
      updateGame(data);
    })
    .catch((error) => {
      console.error('Something went wrong when requesting a game update: ', error);
    });
}

//Update the onscreen gameboard according to a new game state
function updateGame(game) {

  eval('board = ' + game.state);

  let rowNum = 1;
  for (const row of board) {
    let colNum = 1;
    for (const col of row) {
      let space = document.getElementById(
        String(rowNum) + 'x' + String(colNum)
      );
      space.classList.remove('red', 'black');

      if (col === 'black') {
        space.classList.add('black');
      } else if (col === 'red') {
        space.classList.add('red');
      }
      colNum += 1;
    }
    rowNum += 1;
  }

  
  statusUpdate(game);
}

//Change the status field whenever a status update occurs
function statusUpdate(game) {

  let stat = 'notStarted';

  if (game.isGameOver) {
    stat = 'gameEnded';
  }

  if (game.isPlayer1Turn) {
    stat = 'red';
  }

  if (game.isPlayer2Turn) {
    stat = 'black';
  }

  if (game.isPlayer1Winner) {
    stat = 'redWin';
  }
  
  if (game.isPlayer2Winner) {
    stat = 'blackWin';
  }

  currStat = stat;

  if (stat === 'notStarted') {
    statElem.innerHTML = `Waiting to start...`;
    statElem.className = '';
    statElem.classList.add('notStarted');
  } else if (stat === 'red') {
    statElem.innerHTML = `It's the Red player's turn.`;
    statElem.className = '';
    statElem.classList.add('redTurn');
  } else if (stat === 'black') {
    statElem.innerHTML = `It's the Black player's turn.`;
    statElem.className = '';
    statElem.classList.add('blackTurn');
  } else if (stat === 'redWin') {
    statElem.innerHTML = `The Red player wins!`;
    statElem.className = '';
    statElem.classList.add('redWin');
    clearInterval(poll);

    //reset game
    if (currPlayer === 'black') {
      fetch(`${url}/api/Game/${currID}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'text/plain',
        }
      })
        .then((response) => 
          response.json()
        )
        .then((data) => {
          updateGame(data);
        })
        .catch((error) => {
          console.error('Something went wrong when deleting a game: ', error);
        });
    }
  } else if (stat === 'blackWin') {
    statElem.innerHTML = `The Black player wins!`;
    statElem.className = '';
    statElem.classList.add('blackWin');
    clearInterval(poll);

    //reset game
    if (currPlayer === 'red') {
      fetch(`${url}/api/Game/${currID}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'text/plain',
        }
      })
        .then((response) => 
          response.json()
        )
        .then((data) => {
          updateGame(data);
        })
        .catch((error) => {
          console.error('Something went wrong when deleting a game: ', error);
        });
    }
  } else if (stat === 'gameEnded') {
    statElem.innerHTML = `The game ended.`;
    statElem.className = '';
    statElem.classList.add('disconnect');
    clearInterval(poll);
  } else if (stat === 'draw') {
    statElem.innerHTML = `The game ended in a draw!`;
    statElem.className = '';
    statElem.classList.add('draw');
    clearInterval(poll);
  }
}

//send a new move to the server
function sendMove(column) {

  dataColumn = parseInt(column);
  dataColumn -= 1;

  fetch(`${url}/api/Game/Play/${currPlayer}/Column/${column}?id=${currID}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    }
  })
    .then((response) => 
      response.json()
    )
    .then((data) => {
      updateGame(data);
    })
    .catch((error) => {
      console.error('Something went wrong when sending a move: ', error);
    });
}

//event listeners for board interaction
let spaces = document.querySelectorAll('td');
spaces.forEach(function (elem) {
  elem.addEventListener('mouseover', function () {
    if (currStat == currPlayer) {
      let headers = document.querySelectorAll('th');
      headers.forEach(function (header) {
        header.classList.remove('hovered');
      });
      let arrow = document.getElementById('col' + elem.id[2]);
      arrow.classList.add('hovered');
    }
  });

  elem.addEventListener('mouseout', function () {
    let arrow = document.getElementById('col' + elem.id[2]);
    arrow.classList.remove('hovered');
  });

  elem.addEventListener('click', function () {
    if (currStat == currPlayer) {
      sendMove(elem.id[2]);
    }
  });
});


/*
fetch('https://connect4-api.azurewebsites.net/api/Lobbies/PostLobby', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
  },
});

fetch('/getcookie').then((response) => {
  console.log('HTTP Status: ' + response.status);
  if (!response.ok) console.log('Request failed');
  else
    response.json().then((data) => {
      currPlayer = data.cookie;
    });
});

*/


