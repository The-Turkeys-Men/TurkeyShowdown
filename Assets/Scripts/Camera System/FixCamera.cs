using UnityEngine;

public class FixCamera : MonoBehaviour
{
     [SerializeField]private Camera Camera;

    [SerializeField]private int _maxX, _maxY,_minX, _minY;
    private float newCameraPositionX ;
    private float newCameraPositionY ;
   
    void Awake()
    {
        Camera = GetComponent<Camera>();
    }
    void Update()
    {
        MoveCamera();
    }
    private void MoveCamera()
    {
        if (Camera != null)
        {
            if (transform.position.x < _maxX && transform.position.x > _minX)
            {
               newCameraPositionX=transform.position.x;
            }

            if (transform.position.y < _maxY && transform.position.y > _minY)
            {
                newCameraPositionY=transform.position.y;
            }
            Camera.transform.position=new Vector3(newCameraPositionX,newCameraPositionY,Camera.transform.position.z);


            
        }
    }
}
