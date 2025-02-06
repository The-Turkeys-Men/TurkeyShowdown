using UnityEngine;

public class FixCamera : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private Transform _followTransform;
     [SerializeField] private float _speed = 5;
    
    private int _maxX, _maxY,_minX, _minY;
    private float _newCameraPositionX ;
    private float _newCameraPositionY ;
    public bool IsSemiLock;
   
    void Awake()
    {
        _transform = GetComponent<Transform>();
    }
    void Update()
    {
       if(IsSemiLock)
        {
            MoveCameraSemiLock();
            Debug.Log (IsSemiLock);
        }
        else
        {
            MoveCamera();
        }

    }
    private void MoveCamera()
    {
        _newCameraPositionX = Mathf.Clamp(_followTransform.position.x, _minX, _maxX);

        _newCameraPositionY = Mathf.Clamp(_followTransform.position.y, _minY, _maxY);

        _transform.position = new Vector3(_newCameraPositionX, _newCameraPositionY, _transform.position.z);
    }
     private void MoveCameraSemiLock()
    {
       
        
            Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.width - 0.5f, Input.mousePosition.y / Screen.height - 0.5f);
            mousePos.x = Mathf.Clamp(mousePos.x, -0.5f, 0.5f);
            mousePos.y = Mathf.Clamp(mousePos.y, -0.5f, 0.5f);
            Vector3 newPosition = (Vector2)_followTransform.position + mousePos * _speed;
            newPosition.z = -10;
            newPosition.x = Mathf.Clamp(newPosition.x, _minX, _maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, _minY, _maxY);
            _transform.position = newPosition;

            
    }  
}


