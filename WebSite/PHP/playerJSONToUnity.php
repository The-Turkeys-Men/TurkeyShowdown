<?php
include 'playerDataHandler.php';

header("Content-Type: application/json; charset=UTF-8");

session_start();

if (!isset($_SESSION['user_id'])) {
  echo json_encode(["error" => "User not authenticated"]);
  exit;
}

$userId = $_SESSION['user_id'];

$playerDataJson = getPlayerData($userId);

if ($playerDataJson === null) {
  echo json_encode(["error" => "No player data found"]);
  exit;
}

echo json_encode([
  "success" => true,
  "playerData" => json_decode($playerDataJson, true)
]);
?>
