<?php

require 'TestUnitaire.php';

$test = new AuthentificationTest();

try {
  $test->setUp();
  $test->testUserCreation();
  fwrite(STDOUT, "testUserCreation passed.\n");
  fflush(STDOUT);

  $test->setUp();
  $test->testUserLogin();
  fwrite(STDOUT, "testUserLogin passed.\n");
  fflush(STDOUT);

  $test->setUp();
  $test->testUserLogout();
  fwrite(STDOUT, "testUserLogout passed.\n");
  fflush(STDOUT);
} catch (Exception $e) {
  fwrite(STDERR, "Test failed: " . $e->getMessage() . "\n");
}
