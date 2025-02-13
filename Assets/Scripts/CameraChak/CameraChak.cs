using Unity.Cinemachine;
using UnityEngine;


public class CameraChak : MonoBehaviour
{
    public void shak(float durer,float magnitud,CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(magnitud);
    }
}
        
    
