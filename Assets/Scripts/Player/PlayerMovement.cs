using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private Grappler Grappler;
    public float MaxWalkSpeed = 7f; // Max walking speed
    public float Acceleration = 15f;
    [Range(0f,1f)]public float Friction = .1f;
    public float TimePas;

    private Rigidbody2D _rigidBody;

    private Vector2 _moveDirection;

    [SerializeField]float baseVolume;
    
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (!Grappler.IsGripped && _moveDirection == Vector2.zero)
        {
            Vector2 friction = _rigidBody.linearVelocity * Friction;
            _rigidBody.AddForce(-friction, ForceMode2D.Force);
        }
    }

    private void FixedUpdate()
    {
       
        if (!IsOwner)
        {
            return;
        }
        
        Move(_moveDirection);
        
        // Apply friction to gradually slow down
        //_rigidBody.linearVelocity *= (1 - Friction * Time.fixedDeltaTime);
    }

    public void TryMove(Vector2 movementDirection)
    {
        _moveDirection = movementDirection;
        Move(_moveDirection);
    }

    private void Move(Vector2 movementDirection)
    {
        // Normalize the movement direction to avoid faster movement diagonally
        if (movementDirection.magnitude > 0)
        {
            movementDirection.Normalize();
            if((TimePas-=Time.deltaTime)<=0)
            {
                AudioManager.Instance.PlaySFX("bruisDePas",transform.position,baseVolume);
                TimePas=0.3f;
            }
            
        }
        
        // Get the current velocity of the rigidbody
        Vector2 velocity = _rigidBody.linearVelocity;
        float projectedSpeed = Vector2.Dot(velocity, movementDirection);

        // Apply a force only if the projected speed is less than the max speed or if the speed is negative (slowing down)
        Vector2 force = new Vector2(movementDirection.x * Acceleration, movementDirection.y * Acceleration);
        if (Mathf.Abs(velocity.x) < MaxWalkSpeed)
        {
            Vector2 xForce = new Vector2(force.x, 0);
            _rigidBody.AddForce(xForce, ForceMode2D.Force);
        }
        
        if (Mathf.Abs(velocity.y) < MaxWalkSpeed)
        {
            Vector2 yForce = new Vector2(0, force.y);
            _rigidBody.AddForce(yForce, ForceMode2D.Force);
        }

        if (Grappler.IsGripped)
        {
            return;
        }

        if (Mathf.Sign(force.x) != Mathf.Sign(_rigidBody.linearVelocity.x))
        {
            Debug.Log("frein X");
            Vector2 xForce = force * 5;
            xForce.y = 0;
            _rigidBody.AddForce(xForce, ForceMode2D.Force);
        }
        
        if (Mathf.Sign(force.y) != Mathf.Sign(_rigidBody.linearVelocity.y))
        {
            Vector2 yForce = force * 5;
            yForce.x = 0;
            _rigidBody.AddForce(yForce, ForceMode2D.Force);
        }
    }
}
