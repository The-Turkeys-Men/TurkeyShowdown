using UnityEngine;

public class DesponeTraine : MonoBehaviour
{
    private LineRenderer _visibiliti;


     void Start()
    {
        _visibiliti = GetComponent<LineRenderer>();
         _visibiliti.startWidth=0.5f;
            _visibiliti.endWidth=0.5f;
        
    }
    void Update()
    {
        DeDamaag();
    }
     void DeDamaag()
    {
        
        
            _visibiliti.startWidth-=Time.deltaTime;
            _visibiliti.endWidth-=Time.deltaTime;
            if (_visibiliti.startWidth==0.001)
            {
                Destroy(gameObject);
            }

        
    }
}
