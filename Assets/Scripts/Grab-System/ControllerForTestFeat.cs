using UnityEngine;

public class ControllerForTestFeat : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;

    private Rigidbody2D _rb;

    private Vector2 _movementDir;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        _movementDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _movementDir * _movementSpeed;
    }

}
