using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float MaxWalkSpeed = 7f; // Max walking speed
    public float Acceleration = 15f;
    public float Friction = 3f;
    public float BoostForce = 20f; // Force applied with the special key - To be removed

    private Rigidbody2D _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Apply a manual force with the Space key - To be removed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 boostDirection = _rigidBody.linearVelocity.normalized;
            _rigidBody.AddForce(boostDirection * BoostForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        TryMove();
    }

    private void TryMove()
    {
        Move();
    }

    private void Move()
    {
        // To be modified with actual inputs
        Vector2 movementDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movementDirection.y += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movementDirection.y -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movementDirection.x += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movementDirection.x -= 1;
        }

        // Normalize the movement direction to avoid faster movement diagonally
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();
        }

        // Get the current velocity of the rigidbody
        Vector2 velocity = _rigidBody.linearVelocity;
        float projectedSpeed = Vector2.Dot(velocity, movementDirection);

        // Apply a force only if the projected speed is less than the max speed or if the speed is negative (slowing down)
        if (projectedSpeed < MaxWalkSpeed || projectedSpeed < 0)
        {
            _rigidBody.AddForce(movementDirection * Acceleration, ForceMode2D.Force);
        }

        // Apply friction to gradually slow down
        _rigidBody.linearVelocity *= (1 - Friction * Time.fixedDeltaTime);
    }
}
