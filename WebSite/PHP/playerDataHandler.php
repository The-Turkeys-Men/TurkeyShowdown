<?php
include 'config.php';

function updatePlayerData($userId, $playerJson): void
{
  $db = Database::getInstance();

  if (!$db) {
    die("Erreur de connexion à la base de données.");
  }

  $jsonEncoded = json_encode($playerJson);
  if ($jsonEncoded === false) {
    die("Erreur lors de l'encodage JSON : " . json_last_error_msg());
  }

  $query = "UPDATE player_data SET score_table = ? WHERE user_id = ?";
  $stmt = $db->prepare($query);
  $stmt->bind_param("si", $jsonEncoded, $userId);

  if (!$stmt->execute()) {
    die("Erreur d'exécution de la requête : " . $stmt->error);
  }

  echo "Données mises à jour avec succès.";
  $stmt->close();
}

function savePlayerData($userId, $playerJson): void
{
  updatePlayerData($userId, $playerJson);
}

function getPlayerData($userId)
{
  $db = Database::getInstance();

  if (!$db) {
    die("Erreur de connexion à la base de données.");
  }

  $query = "SELECT score_table FROM player_data WHERE user_id = ?";
  $stmt = $db->prepare($query);
  $stmt->bind_param("i", $userId);
  $stmt->execute();
  $result = $stmt->get_result();

  if ($result->num_rows === 0) {
    echo "Aucune donnée trouvée pour cet utilisateur.";
    return null;
  }

  $row = $result->fetch_assoc();
  return $row['score_table'];
}

function changePlayerDataColor($userId, $newColor): void
{
  $playerDataJson = json_decode(getPlayerData($userId), true);
  if ($playerDataJson === null) {
    die("Aucune donnée de joueur trouvée pour cet utilisateur.");
  }

  $playerDataJson['color'] = $newColor;
  updatePlayerData($userId, $playerDataJson);
}
?>
