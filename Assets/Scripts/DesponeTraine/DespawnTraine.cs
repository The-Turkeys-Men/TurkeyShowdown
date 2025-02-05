using UnityEngine;

public class DespawnTraine : MonoBehaviour
{
    private LineRenderer _visibility;

    public float StartWidth = 0.5f;

     void Start()
    {
        _visibility = GetComponent<LineRenderer>();
         _visibility.startWidth= StartWidth;
            _visibility.endWidth= StartWidth;
        
    }
    void Update()
    {
        Despawning();
    }
     void Despawning()
    {
        _visibility.startWidth-=Time.deltaTime;
        _visibility.endWidth-=Time.deltaTime;
        if (_visibility.startWidth<=0.001)
        {
            Destroy(gameObject);
        }
    }
}
