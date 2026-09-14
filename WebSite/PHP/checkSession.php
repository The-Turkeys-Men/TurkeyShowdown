<?php
session_start();
header('Content-Type: application/json; charset=utf-8');

// Vérifier si l'utilisateur est connecté
if (isset($_SESSION['user_id'])) {
  $response = [
    'loggedIn' => true,
    'userName' => $_SESSION['user_name'],
    'userPseudo' => $_SESSION['user_pseudo'],
    'userEmail' => $_SESSION['user_email'],
  ];

  // Inclure les données du joueur si elles existent dans la session
  if (isset($_SESSION['player_data'])) {
    $response['playerData'] = $_SESSION['player_data'];
  }

  echo json_encode($response);
} else {
  // Si l'utilisateur n'est pas connecté
  echo json_encode([
    'loggedIn' => false
  ]);
}
?>
