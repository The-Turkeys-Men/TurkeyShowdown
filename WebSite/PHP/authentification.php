<?php
include 'config.php';
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

// Connexion à la base de données
$conn = Database::getInstance();

if (!$conn) {
  die("Échec de la connexion : " . mysqli_connect_error());
}


if (!function_exists('sanitizeInput')) {
    function sanitizeInput($data): string
    {
        return htmlspecialchars(strip_tags($data));
    }
}


if (!function_exists('createUser')) {
  function createUser($conn)
  {
    $nom = sanitizeInput($_POST['nom']);
    $pseudo = sanitizeInput($_POST['pseudo']);
    $email = filter_var($_POST['email'], FILTER_VALIDATE_EMAIL);
    $password = password_hash($_POST['password'], PASSWORD_BCRYPT);

    // Vérifier si le pseudo ou l'email existe déjà
    $checkSql = "SELECT 1 FROM users WHERE pseudo = ? OR email = ?";
    $stmt = $conn->prepare($checkSql);
    $stmt->bind_param("ss", $pseudo, $email);
    $stmt->execute();
    $stmt->store_result();

    if ($stmt->num_rows > 0) {
      die("Erreur : Ce pseudo ou cet email existe déjà.");
    }
    $stmt->close();

    // Insérer un nouvel utilisateur
    $sql = "INSERT INTO users (nom, pseudo, email, mot_de_passe) VALUES (?, ?, ?, ?)";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("ssss", $nom, $pseudo, $email, $password);

    if (!$stmt->execute()) {
      die("Erreur lors de l'insertion : " . $conn->error);
    }

    $userId = $stmt->insert_id;
    $stmt->close();

    // Créer une entrée dans la table `player_data`
    $sqlPlayerData = "INSERT INTO player_data (user_id, high_score, score_table, nbr_victory, nbr_defeat, color)
                          VALUES (?, 0, '[]', 0, 0, 'default')";
    $stmtPlayer = $conn->prepare($sqlPlayerData);
    $stmtPlayer->bind_param("i", $userId);

    if (!$stmtPlayer->execute()) {
      die("Erreur lors de la création des données du joueur : " . $conn->error);
    }
    $stmtPlayer->close();

    connectUser($conn);
    exit;
  }
}

if (!function_exists('connectUser')) {
  function connectUser($conn)
  {
    session_start();

    $pseudo = sanitizeInput($_POST['pseudo']);
    $email = filter_var($_POST['email'], FILTER_VALIDATE_EMAIL);
    $password = $_POST['password'];

    if ($pseudo && $email && $password) {
      // Récupérer l'utilisateur
      $sql = "SELECT * FROM users WHERE email = ? AND pseudo = ?";
      $stmt = $conn->prepare($sql);
      $stmt->bind_param("ss", $email, $pseudo);
      $stmt->execute();
      $result = $stmt->get_result();

      if ($result->num_rows > 0) {
        $user = $result->fetch_assoc();
        if (password_verify($password, $user['mot_de_passe'])) {
          // Récupérer les données du joueur
          $sqlPlayerData = "SELECT * FROM player_data WHERE user_id = ?";
          $stmtPlayer = $conn->prepare($sqlPlayerData);
          $stmtPlayer->bind_param("i", $user['id']);
          $stmtPlayer->execute();
          $playerResult = $stmtPlayer->get_result();

          if ($playerData = $playerResult->fetch_assoc()) {
            $_SESSION['player_data'] = $playerData;
          }

          $stmtPlayer->close();

          $_SESSION['user_id'] = $user['id'];
          $_SESSION['user_name'] = $user['nom'];
          $_SESSION['user_pseudo'] = $user['pseudo'];
          $_SESSION['user_email'] = $user['email'];
          header('Location: /homePage.php');
          exit;
        }
      }

      $_SESSION['login_error'] = 'Pseudo, email ou mot de passe incorrect.';
      header('Location: /homePage.php');
      exit;
    }

    $_SESSION['login_error'] = 'Veuillez remplir tous les champs.';
    header('Location: /homePage.php');
    exit;
  }
}

if (isset($_POST['create'])) {
  createUser($conn);
} elseif (isset($_POST['connect'])) {
  connectUser($conn);
}
?>
