const stat = document.getElementById('status');
const player = document.getElementById('player');
let currStat = 'red';
let currPlayer = 'red';
let cookie = '';

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

function updateGame() {

  fetch('updateGame').then((res) => {
    let rowNum = 1;
    let colNum = 1;

    for (const row of res.board) {
      for (const col of row) {
        let space = document.getElementById(
          String(rowNum) + 'x' + String(colNum)
        );
        space.classList.remove('red', 'black');

        if (col == 1) {
          space.classList.add('black');
        } else if (col == 2) {
          space.classList.add('red');
        }
        colNum += 1;
      }
      rowNum += 1;
    }

    player.innerHTML = 'Playing as ' + res.player;
    stat.innerHTML = 'Current Status: ' + res.status;
    currStat = res.status;
    currPlayer = res.player;
  });
}

function sendMove(column) {
  console.log('placing piece at ' + String(column));
  data = {
    move: column,
  };
  fetch('move', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(column),
  })
    .then((response) => {
      response.json();
    })
    .then((data) => {
      updateGame();
    })
    .catch((error) => {
      console.error('Error:', error);
    });
}

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
      document.cookie = data.cookie;
    });
});


//poll server every 5 seconds
setInterval(updateGame(), 5000);
