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
    @font-face {
      font-family: 'SauceBarbe';
      src: url("font/SauceBarbe.woff") format('woff');
    }
    .custom-font {
      font-family: SauceBarbe;
    }
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
      font-family: SauceBarbe;
      color: white;
      font-size: 1.8em;
      position: absolute;
      top: 160px;
      background-size: contain;
      width: 100%;
      height: 500px; /* Ajustez la hauteur selon vos besoins */
      text-align: center;
      line-height: 100px;
      z-index: 150;
    }
    .header {
      position: absolute;
      top: 10px;
      right: 20px;
      display: flex;
      align-items: center;
      z-index: 10;
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
      top: 60%;
      left: 50%;
      transform: translate(-50%, -50%);
      z-index: 200;
    }
    .home-button {
      padding: 10px 20px;
      margin-right: 10px;
      background-color: gray;
      color: white;
      border: none;
      cursor: pointer;
      border-radius: 5px;
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
      background: url('UI/UIButton.png') no-repeat center center;
      background-size: contain;
      color: white;
      border: none;
      cursor: pointer;
      font-size: 1.5em;
      border-radius: 5px;
      z-index: 160;
    }
    .popup {
      background-image: url('UI/DindonBackgroundSkinChange.png');
      background-size: 100% 100%;
      background-position: center;
      padding: 20px;
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
      background: url('UI/UIButton.png') no-repeat center center fixed;
      background-size: contain;
      color: white;
      text-align: center;
      border-radius: 5px;
      cursor: pointer;
      width: 200px;
    }
    .container {
      position: absolute;
      top: 40%;
      left: 50%;
      transform: translateX(-50%);
      display: flex;
      flex-direction: column;
      align-items: center;
      z-index: 200;
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
      margin-top: -75px;
      z-index: 200;
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
      width: 550px;
      height: 375px;
      background-image: url('UI/DindonBackgroundSkinChange.png');
      background-size: 100% 100%;
      background-position: center;
      top: 32%;
      left: 50%;
      transform: translate(-50%, -50%);
      z-index: -1;
    }
    .panel2 {
      position: absolute;
      width: 500px;
      height: 85px;
      background-color: rgba(0, 0, 0, 0.53);
      top: 71%;
      left: 50%;
      transform: translate(-50%, -50%);
      z-index: -1;
    }
    .play-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      margin-top: 400px; /* Augmente cette valeur pour déplacer le bouton plus bas */
      width: 100%;
      padding: 10px;
      box-sizing: border-box;
    }
    .image-container {
      position: relative;
      width: 150px; /* Ajustez selon vos images */
      height: 253px;
      top: -50px;
    }
    .image-overlay {
      position: absolute;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
    }
    .color-box.gray {
      position: relative;
      display: flex;
      justify-content: center;
      align-items: center;
    }
    .color-box.gray::before,
    .color-box.gray::after {
      content: "";
      position: absolute;
      width: 60%;
      height: 3px;
      background-color: red;
    }
    .color-box.gray::before {
      transform: rotate(45deg);
    }
    .color-box.gray::after {
      transform: rotate(-45deg);
    }
    @media (orientation: portrait) {
      body {
        flex-direction: column;
        align-items: center;
        padding: 10px;
      }
      .header {
        position: fixed; /* Fixe le header en haut à droite */
        top: 0; /* Placer tout en haut */
        right: 0; /* Placer tout à droite */
        display: flex;
        justify-content: flex-start; /* Aligner les éléments à gauche */
        align-items: center;
        padding: 10px;
        background-color: rgba(0, 0, 0, 0.5); /* Optionnel : un fond semi-transparent pour le header */
        z-index: 10;
        width: auto; /* Largeur automatique pour éviter d'étendre sur toute la page */
      }
      .header span {
        color: white;
        font-size: 1.2em;
        margin-right: 10px;
      }
      h1 {
        font-size: 2em;
        margin-top: 10px;
      }
      .container {
        position: absolute;
        top: 20%; /* Ajustez ce pourcentage pour déplacer la zone plus bas */
        left: 50%;
        transform: translateX(-50%);
        display: flex;
        flex-direction: column;
        align-items: center;
        z-index: 1;
        width: 80%;
        max-width: 400px;
      }
      #mainImage {
        width: 120px;
        height: 120px;
        margin-bottom: 15px;
      }
      .color-picker {
        flex-wrap: wrap;
        justify-content: center;
        margin-top: 5px;
        width: 100%;
      }
      .color-box {
        width: 25px;
        height: 25px;
        margin: 5px;
      }
      .panel1 {
        width: 100%;
        height: 120%;
        background-color: rgba(0, 0, 0, 0.53);
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        z-index: -1;
      }
      .panel2 {
        width: 80%;
        height: 80px;
        background-color: rgba(0, 0, 0, 0.53);
        top: 54%;
        z-index: -1;
        display: flex;
        align-items: center;
        justify-content: center; /* Espace entre le panneau et le bouton */
      }
      .play-container {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        margin-top: 75px; /* Augmente cette valeur pour déplacer le bouton plus bas */
        width: 100%;
        padding: 10px;
        box-sizing: border-box;
      }
      .play-button {
        padding: 15px 30px;
        background-color: #007BFF;
        color: white;
        border: none;
        cursor: pointer;
        font-size: 1.5em;
        z-index: 20;
        border-radius: 5px;
        margin-bottom: 20px; /* Espacement entre le bouton et le panneau */
      }
      .home-button, .logout-button {
        padding: 8px 16px;
        font-size: 1em;
      }
      .toggle-panel {
        padding: 8px 16px;
        font-size: 1em;
        width: 80%;
      }
      .game-container iframe {
        height: 80vh;
      }
      .popupContainer {
        width: 80%;
        max-width: 450px;
        z-index: 30;
      }
    }
    .color-box.gray {
      position: relative;
      display: flex;
      justify-content: center;
      align-items: center;
    }
    .color-box.gray::before,
    .color-box.gray::after {
      content: "";
      position: absolute;
      width: 100%;
      height: 3px;
      background-color: red;
    }
    .color-box.gray::before {
      transform: rotate(45deg);
    }
    .color-box.gray::after {
      transform: rotate(-45deg);
    }
  </style>
</head>
<body>

<div class="header" id="header">
  <?php if ($isLoggedIn): ?>
    <span id="playerName"><?= htmlspecialchars($userName) ?></span>
    <form action="homePage.php" method="GET">
      <button type="submit" class="home-button custom-font">Accueil</button>
    </form>
    <form action="PHP/logout.php" method="POST">
      <button type="submit" class="logout-button custom-font">Déconnexion</button>
    </form>
  <?php endif; ?>
</div>
<div class="custom-font" style="position: absolute; top: 160px; left: 47.5%; font-size: 28px; z-index: 150; color: white;"><strong>Ultimate</strong></div>
<h1>TurkeyShowdown</h1>

<img src="ledindon_TITRE.png" width="700" height="400" style="position: absolute; top: 25px; left: 32%;">

<div class="container" style="display: <?= $isLoggedIn ? 'flex' : 'none'; ?>">
  <div class="panel1" id="panel1"></div>

  <div class="image-container">
    <img class="image-overlay main-image" src="DindonImage/DindonDefault.png" alt="Image principale">
  </div>

  <div class="color-picker">
    <div class="color-box" style="background-color: blue;" onclick="changeColor('ultradindon')"></div>
    <div class="color-box" style="background-color: green;" onclick="changeColor('dindoom')"></div>
    <div class="color-box" style="background-color: yellow;" onclick="changeColor('bumbledindon')"></div>
    <div class="color-box" style="background-color: orange;" onclick="changeColor('gundindle')"></div>
    <div class="color-box" style="background-color: red;" onclick="changeColor('irondindon')"></div>
    <div class="color-box" style="background-color: pink;" onclick="changeColor('dindonmede')"></div>
    <div class="color-box" style="background-color: purple;" onclick="changeColor('evenlindon')"></div>
    <div class="color-box" style="background-color: white;" onclick="changeColor('knight')"></div>
    <div class="color-box gray" style="background-color: gray;" onclick="resetColor()"></div>
  </div>
</div>

<div class="play-container">
  <div class="panel2" id="panel2" style="display: none"></div>
  <button class="play-button custom-font" onclick="startGame()">Jouer</button>
</div>

<div class="popupContainer">
  <div class="popup" id="loginPopup">
    <h2 id="formTitle">Connexion</h2>
    <div id="formContainer"></div>
  </div>

  <!-- Bouton pour changer le formulaire, placé EN DEHORS de la popup -->
  <div class="toggle-panel" id="togglePanel" onclick="toggleForm()">
    <p class="toggle-form custom-font">Créer un compte</p>
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
      document.querySelector('.custom-font[style*="top: 160px"]').style.display = 'none'; // Masquer le texte "Ultimate"
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
    const images = document.querySelectorAll('.main-image');

    const imageSources = {
      irondindon: 'DindonImage/IronDindon.png',
      ultradindon: 'DindonImage/UltraDindon.png',
      dindoom: 'DindonImage/DinDoom.png',
      gundindle: 'DindonImage/Gundindle.png',
      bumbledindon: 'DindonImage/BumbleDindon.png',
      evenlindon: 'DindonImage/DindonEvenlindon.png',
      dindonmede: 'DindonImage/DindonMede.png',
      knight: 'DindonImage/DindonKnight.png',
      default: 'DindonImage/DindonDefault.png'
    };

    images.forEach(image => {
      if (imageSources[color]) {
        image.src = imageSources[color];
      } else {
        image.src = imageSources['default'];
      }
    });

    currentColor = color;
  }

  function resetColor() {
    const images = document.querySelectorAll('.main-image');
    images.forEach(image => {
      image.src = 'DindonImage/DindonDefault.png';
    });

    currentColor = 'default';
  }


</script>

</body>
</html>
