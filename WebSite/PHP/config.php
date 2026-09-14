<?php
if (session_status() === PHP_SESSION_NONE) {
  session_set_cookie_params([
    'lifetime' => 0,
    'path' => '/',
    'domain' => 'localhost',
    'secure' => true,
    'httponly' => true,
    'samesite' => 'None'
  ]);
  session_start();
}

if (!class_exists('Database')) {
  class Database {
    private static $instance = null;
    private $conn;
    private string $host;
    private string $username;
    private string $password;
    private string $dbname;

    private function __construct() {
      if ($_SERVER['SERVER_NAME'] === 'localhost') {
        // Configuration locale
        $this->host = 'localhost';
        $this->username = 'root';
        $this->password = '';
        $this->dbname = 'utilisateurs';
      } else {
        // Configuration serveur via variables d'environnement
        $this->host = getenv('DB_HOST') ?: 'localhost';
        $this->username = getenv('DB_USER') ?: 'root';
        $this->password = getenv('DB_PASS') ?: '';
        $this->dbname = getenv('DB_NAME') ?: 'utilisateurs';
      }

      $this->conn = new mysqli($this->host, $this->username, $this->password, $this->dbname);

      if ($this->conn->connect_error) {
        die("Erreur de connexion : " . $this->conn->connect_error);
      }

      $this->createTables();
    }

    private function createTables() {
      $sql_create_users_table = "
                CREATE TABLE IF NOT EXISTS users (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    nom VARCHAR(255) NOT NULL,
                    pseudo VARCHAR(255) NOT NULL UNIQUE,
                    email VARCHAR(255) NOT NULL UNIQUE,
                    mot_de_passe TEXT NOT NULL,
                    date_creation TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )";

      if (!$this->conn->query($sql_create_users_table)) {
        die("Erreur lors de la création de la table users : " . $this->conn->error);
      }

      $sql_create_player_data_table = "
                CREATE TABLE IF NOT EXISTS player_data (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    user_id INT NOT NULL UNIQUE,
                    high_score INT DEFAULT 0,
                    score_table JSON DEFAULT NULL,
                    nbr_victory INT DEFAULT 0,
                    nbr_defeat INT DEFAULT 0,
                    color VARCHAR(50) DEFAULT 'default',
                    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
                )";

      if (!$this->conn->query($sql_create_player_data_table)) {
        die("Erreur lors de la création de la table player_data : " . $this->conn->error);
      }
    }

    public static function getInstance() {
      if (self::$instance === null) {
        self::$instance = new Database();
      }
      return self::$instance->conn;
    }
  }
}
?>
