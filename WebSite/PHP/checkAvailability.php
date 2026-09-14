<?php
// Activer les messages d'erreurs pour le débogage
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

header('Content-Type: application/json; charset=utf-8');

try {
  include 'config.php';

  $conn = Database::getInstance();

  if (!isset($_GET['type']) || !isset($_GET['value'])) {
    echo json_encode(["error" => "Paramètres invalides."]);
    exit;
  }

  $type = $_GET['type'];
  $value = $_GET['value'];

  // Valider le type pour éviter les injections SQL
  $allowedTypes = ['pseudo', 'email'];
  if (!in_array($type, $allowedTypes)) {
    echo json_encode(["error" => "Type de vérification invalide."]);
    exit;
  }

  // Construire la requête SQL avec des paramètres préparés
  $query = "SELECT $type FROM utilisateurs WHERE $type = ?";
  $stmt = $conn->prepare($query);
  $stmt->bind_param("s", $value);
  $stmt->execute();
  $stmt->store_result();

  // Vérifier si des résultats existent
  $exists = ($stmt->num_rows > 0);
  echo json_encode(["exists" => $exists]);

  $stmt->close();
} catch (Exception $e) {
  // En cas d'erreur, renvoyer un JSON avec le message d'erreur
  echo json_encode(["error" => $e->getMessage()]);
}
?>
