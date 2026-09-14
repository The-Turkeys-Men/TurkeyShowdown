<?php

class PlayerJSON {
  public int $id;
  public int $userId;
  public int $highScore;
  public array $scoreTable;
  public int $nbrVictory;
  public int $nbrDefeat;
  public string $color;

  public function __construct() {
    $this->scoreTable = [];
    $this->color = 'default';
  }

  // Load player data from the database
  public static function loadByUserId(mysqli $conn, int $userId): ?PlayerJSON {
    $sql = "SELECT * FROM player_data WHERE user_id = ?";
    $stmt = $conn->prepare($sql);
    $stmt->bind_param("i", $userId);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($row = $result->fetch_assoc()) {
      $player = new self();
      $player->id = $row['id'];
      $player->userId = $row['user_id'];
      $player->highScore = $row['high_score'];
      $player->scoreTable = json_decode($row['score_table'], true);
      $player->nbrVictory = $row['nbr_victory'];
      $player->nbrDefeat = $row['nbr_defeat'];
      $player->color = $row['color'];
      return $player;
    }

    return null; // Return null if no data is found
  }

  // Save player data to the database
  public function save(mysqli $conn): bool {
    $sql = "INSERT INTO player_data (user_id, high_score, score_table, nbr_victory, nbr_defeat, color)
                VALUES (?, ?, ?, ?, ?, ?)";
    $stmt = $conn->prepare($sql);
    $scoreTableJson = json_encode($this->scoreTable);
    $stmt->bind_param(
      "iisiis",
      $this->userId,
      $this->highScore,
      $scoreTableJson,
      $this->nbrVictory,
      $this->nbrDefeat,
      $this->color
    );

    return $stmt->execute();
  }

  // Update player data in the database
  public function update(mysqli $conn): bool {
    $sql = "UPDATE player_data SET high_score = ?, score_table = ?, nbr_victory = ?, nbr_defeat = ?, color = ?
                WHERE user_id = ?";
    $stmt = $conn->prepare($sql);
    $scoreTableJson = json_encode($this->scoreTable);
    $stmt->bind_param(
      "isiisi",
      $this->highScore,
      $scoreTableJson,
      $this->nbrVictory,
      $this->nbrDefeat,
      $this->color,
      $this->userId
    );

    return $stmt->execute();
  }
}
