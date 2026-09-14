<?php
session_start();

// Vérifier si l'utilisateur est connecté
$isLoggedIn = isset($_SESSION['user_id']) && isset($_SESSION['user_name']);
$userName = $isLoggedIn ? $_SESSION['user_pseudo'] : '';
?>

<!doctype html>
<html lang="fr">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Accueil</title>
  <link rel="stylesheet" href="css/style.css">
  <style>
    body {
      margin: 0;
      padding: 0;
      background: url('ledindon_fond_anime_anim.gif') no-repeat center center fixed;
      background-size: cover;
      font-family: Arial, sans-serif;
      height: 100vh;
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
    }
    h1 {
      color: black;
      font-size: 2.5em;
      position: absolute;
      top: 50px;
    }
    .header {
      position: absolute;
      top: 10px;
      right: 20px;
      display: flex;
      align-items: center;
      z-index: 1000;
    }
    .header span {
      color: white;
      font-size: 1.2em;
      margin-right: 10px;
    }
    .popupContainer {
      display: none; /* Caché par défaut */
      flex-direction: column;
      align-items: center;
      position: fixed;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
    }
    .logout-button {
      padding: 10px 20px;
      background-color: red;
      color: white;
      border: none;
      cursor: pointer;
      border-radius: 5px;
    }
    .play-button {
      padding: 15px 30px;
      background-color: #007BFF;
      color: white;
      border: none;
      cursor: pointer;
      font-size: 1.5em;
      border-radius: 5px;
    }
    .popup {
      background: gray;
      padding: 20px;
      box-shadow: 0 0 10px rgba(0, 0, 0, 0.5);
      border-radius: 10px;
      width: 360px;
      text-align: center;
    }
    .popup button {
      display: block;
      margin: 10px auto;
      padding: 10px;
      width: 80%;
      border: none;
      border-radius: 5px;
      cursor: pointer;
    }
    .game-container {
      display: none;
      width: 100%;
      height: 100vh;
      background-color: black;
      position: fixed;
      top: 0;
      left: 0;
    }
    .toggle-panel {
      margin-top: 10px;
      padding: 10px;
      background: #444;
      color: white;
      text-align: center;
      border-radius: 5px;
      cursor: pointer;
      width: 200px;
    }
    .container {
      position: absolute;
      top: 20%;
      left: 50%;
      transform: translateX(-50%);
      display: flex;
      flex-direction: column;
      align-items: center;
      z-index: 1;
    }
    #mainImage {
      width: 150px;
      height: 150px;
      margin-right: 20px;
    }
    .color-picker {
      display: flex;
      flex-direction: row;
      flex-wrap: wrap;
      justify-content: center;
      margin-top: 10px;
    }
    .color-box {
      width: 30px;
      height: 30px;
      margin: 5px;
      border: 2px solid black;
      cursor: pointer;
    }
    .panel1 {
      position: absolute;
      width: 500px;
      height: 250px;
      background-color: rgba(0, 0, 0, 0.53);
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      z-index: -1;
    }
    .panel2 {
      position: absolute;
      width: 500px;
      height: 100px;
      background-color: rgba(0, 0, 0, 0.53);
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      z-index: -1;
    }
  </style>
</head>
<body>

<div class="header" id="header">
  <?php if ($isLoggedIn): ?>
    <span id="playerName"><?= htmlspecialchars($userName) ?></span>
    <form action="PHP/logout.php" method="POST">
      <button type="submit" class="logout-button">Déconnexion</button>
    </form>
  <?php endif; ?>
</div>

<h1>TurkeyShowdown</h1>

<div class="container" style="display: <?= $isLoggedIn ? 'flex' : 'none'; ?>">
  <div class="panel1" id="panel1"></div>
  <img id="mainImage" src="dindon.png" alt="Image principale">
  <div class="color-picker">
    <div class="color-box" style="background-color: red;" onclick="changeColor('red')"></div>
    <div class="color-box" style="background-color: blue;" onclick="changeColor('blue')"></div>
    <div class="color-box" style="background-color: green;" onclick="changeColor('green')"></div>
    <div class="color-box" style="background-color: yellow;" onclick="changeColor('yellow')"></div>
    <div class="color-box" style="background-color: orange;" onclick="changeColor('orange')"></div>
    <div class="color-box" style="background-color: purple;" onclick="changeColor('purple')"></div>
    <div class="color-box" style="background-color: pink;" onclick="changeColor('pink')"></div>
    <div class="color-box" style="background-color: white;" onclick="changeColor('white')"></div>
    <div class="color-box" style="background-color: black;" onclick="changeColor('black')"></div>
    <div class="color-box" style="background-color: gray;" onclick="resetColor()"></div>
  </div>
</div>

<div>
  <div class="panel2" id="panel2" style="display: <?= $isLoggedIn ? 'flex' : 'none'; ?>"></div>
  <button class="play-button" onclick="startGame()">Play</button>
</div>

<div class="popupContainer">
  <div class="popup" id="loginPopup">
    <h2 id="formTitle">Connexion</h2>
    <div id="formContainer"></div>
  </div>

  <!-- Bouton pour changer le formulaire, placé EN DEHORS de la popup -->
  <div class="toggle-panel" id="togglePanel" onclick="toggleForm()">
    <p class="toggle-form">Créer un compte</p>
  </div>
</div>

<div class="game-container" id="gameContainer">
  <iframe src="MyGame/index.html" width="100%" height="100%" frameborder="0"></iframe>
</div>

<script>
  const isLoggedIn = <?php echo $isLoggedIn ? 'true' : 'false'; ?>;
  let currentColor = 'default';

  function startGame() {
    if (isLoggedIn) {
      // Appeler la fonction PHP pour mettre à jour la couleur du joueur
      changePlayerColor(currentColor);

      // Démarrer le jeu
      document.body.style.overflow = 'hidden';
      document.querySelector('h1').style.display = 'none';
      document.querySelector('.play-button').style.display = 'none';
      document.querySelector('.popup').style.display = 'none';
      document.querySelector('.container').style.display = 'none';
      document.getElementById('gameContainer').style.display = 'block';
    } else {
      togglePopup();
    }
  }

  function changePlayerColor(color) {
    const xhr = new XMLHttpRequest();
    xhr.open('POST', 'PHP/DataCall/changePlayerColor.php', true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onload = function() {
      if (xhr.status === 200) {
        console.log('Couleur du joueur mise à jour');
      } else {
        console.error('Erreur lors de la mise à jour de la couleur');
      }
    };
    xhr.send('color=' + encodeURIComponent(color));
  }

  function togglePopup() {
    const popup = document.getElementById('loginPopup');
    const togglePanel = document.getElementById('togglePanel');
    const popupContainer = document.querySelector('.popupContainer');

    if (popupContainer.style.display === 'block') {
      popupContainer.style.display = 'none';
      togglePanel.style.display = 'none';
    } else {
      popupContainer.style.display = 'flex';
      togglePanel.style.display = 'block';
      loadForm('HTML/Form/FormUtilisateurConnexion.html');
    }
  }

  function toggleForm() {
    const formTitle = document.getElementById('formTitle');
    const toggleText = document.querySelector('.toggle-form');

    if (formTitle.textContent === 'Connexion') {
      formTitle.textContent = 'Créer un compte';
      toggleText.textContent = 'Se connecter';
      loadForm('HTML/Form/FormUtilisateurCreation.html');
    } else {
      formTitle.textContent = 'Connexion';
      toggleText.textContent = 'Créer un compte';
      loadForm('HTML/Form/FormUtilisateurConnexion.html');
    }
  }

  const loginError = <?php echo isset($_SESSION['login_error']) ? json_encode($_SESSION['login_error']) : 'null'; ?>;

  if (loginError) {
    document.getElementById('loginPopup').style.display = 'block';
    document.getElementById('togglePanel').style.display = 'block';
    loadForm('HTML/Form/FormUtilisateurConnexion.html', loginError);
    <?php unset($_SESSION['login_error']); ?>
  }

  function loadForm(url, errorMessage = null) {
    fetch(url)
      .then(response => response.text())
      .then(html => {
        document.getElementById('formContainer').innerHTML = html;
        if (errorMessage) {
          const errorContainer = document.getElementById('error-container');
          if (errorContainer) {
            errorContainer.textContent = errorMessage;
          }
        }
      })
      .catch(error => console.error('Erreur lors du chargement du formulaire:', error));
  }

  function changeColor(color) {
    const image = document.getElementById('mainImage');

    const colorFilters = {
      red: 'hue-rotate(0deg) saturate(1.5)',
      blue: 'hue-rotate(200deg) saturate(1.5)',
      green: 'hue-rotate(100deg) saturate(1.5)',
      yellow: 'hue-rotate(60deg) saturate(1.5)',
      orange: 'hue-rotate(30deg) saturate(1.5)',
      purple: 'hue-rotate(270deg) saturate(1.5)',
      pink: 'hue-rotate(320deg) saturate(1.5)',
      white: 'brightness(1.5)',
      black: 'brightness(0.3)',
    };

    if (colorFilters[color]) {
      image.style.filter = colorFilters[color];
      currentColor = color;
    } else {
      image.style.filter = 'none';
      currentColor = 'default';
    }
  }

  function resetColor() {
    const image = document.getElementById('mainImage');
    image.style.filter = 'none';
    currentColor = 'default';
  }

</script>

</body>
</html>
