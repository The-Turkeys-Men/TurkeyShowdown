<?php

include 'authentification.php';

class AuthentificationTest
{
  public function setUp(): void
  {
    $_SESSION = [];
    $_POST = [];
  }

  public function testUserCreation()
  {
    $_POST = [
      'nom' => 'Test User',
      'pseudo' => 'testuser',
      'email' => 'testuser@example.com',
      'password' => 'securePassword',
      'create' => true
    ];

    ob_start();
    include 'authentification.php';
    $output = ob_get_clean();

    if (strpos($output, 'Erreur') !== false || empty($_SESSION['user_id'])) {
      throw new Exception('testUserCreation failed.');
    }

    echo "testUserCreation passed.\n";
  }

  public function testUserLogin()
  {
    $_POST = [
      'pseudo' => 'testuser',
      'email' => 'testuser@example.com',
      'password' => 'securePassword',
      'connect' => true
    ];

    ob_start();
    include 'authentification.php';
    $output = ob_get_clean();

    if (strpos($output, 'Erreur') !== false || empty($_SESSION['user_id']) || $_SESSION['user_pseudo'] !== 'testuser') {
      throw new Exception('testUserLogin failed.');
    }

    echo "testUserLogin passed.\n";
  }

  public function testUserLogout()
  {
    $_SESSION['user_id'] = 1;

    session_destroy();

    if (isset($_SESSION['user_id'])) {
      throw new Exception('testUserLogout failed.');
    }

    echo "testUserLogout passed.\n";
  }
}

$test = new AuthentificationTest();

try {
  $test->setUp();
  $test->testUserCreation();

  $test->setUp();
  $test->testUserLogin();

  $test->setUp();
  $test->testUserLogout();
} catch (Exception $e) {
  echo $e->getMessage() . "\n";
}
