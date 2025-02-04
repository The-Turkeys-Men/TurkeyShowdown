using UnityEngine;

public class FixCamera : MonoBehaviour
{
    private Transform _transform;
    [SerializeField] private Transform _followTransform;
    
    [SerializeField]private int _maxX, _maxY,_minX, _minY;
    private float newCameraPositionX ;
    private float newCameraPositionY ;
   
    void Awake()
    {
        _transform = GetComponent<Transform>();
    }
    void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        if (_followTransform.position.x < _maxX && _followTransform.position.x > _minX)
        {
           newCameraPositionX = _followTransform.position.x;
        }

        if (_followTransform.position.y < _maxY && _followTransform.position.y > _minY)
        {
            newCameraPositionY = _followTransform.position.y;
        }

        _transform.position = new Vector3(newCameraPositionX, newCameraPositionY, _transform.position.z);
    }
}
