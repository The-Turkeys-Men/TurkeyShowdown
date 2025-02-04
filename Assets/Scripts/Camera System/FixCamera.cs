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
        newCameraPositionX = Mathf.Clamp(_followTransform.position.x, _minX, _maxX);
        newCameraPositionY = Mathf.Clamp(_followTransform.position.y, _minY, _maxY);
        _transform.position = new Vector3(newCameraPositionX, newCameraPositionY, _transform.position.z);
    }
}
