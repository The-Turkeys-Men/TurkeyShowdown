<?php
include 'playerDataHandler.php';

header("Content-Type: application/json; charset=UTF-8");

$data = file_get_contents("php://input");
$decodedData = json_decode($data, true);

if (json_last_error() !== JSON_ERROR_NONE) {
  echo json_encode(["error" => "Invalid JSON received"]);
  exit;
}

session_start();

if (!isset($_SESSION['user_id'])) {
  echo json_encode(["error" => "User not authenticated"]);
  exit;
}

$userId = $_SESSION['user_id'];

if (!isset($decodedData['scoreTable'], $decodedData['highScore'], $decodedData['nbrVictory'], $decodedData['nbrDefeat'], $decodedData['color'])) {
  echo json_encode(["error" => "Missing required fields in JSON"]);
  exit;
}

$playerData = [
  'scoreTable' => $decodedData['scoreTable'],
  'highScore' => (int)$decodedData['highScore'],
  'nbrVictory' => (int)$decodedData['nbrVictory'],
  'nbrDefeat' => (int)$decodedData['nbrDefeat'],
  'color' => $decodedData['color']
];

savePlayerData($userId, $playerData);

echo json_encode(["success" => "Player data updated successfully"]);
?>
