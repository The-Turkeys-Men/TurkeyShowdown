<?php
include '../playerDataHandler.php';  // Assure-toi que le fichier playerData.php est inclus

session_start();

if (isset($_SESSION['user_id']) && isset($_POST['color'])) {
  $userId = $_SESSION['user_id'];
  $newColor = $_POST['color'];

  // Appeler la fonction pour changer la couleur du joueur
  changePlayerDataColor($userId, $newColor);
} else {
  echo "Erreur: utilisateur non connecté ou couleur invalide.";
}
?>
