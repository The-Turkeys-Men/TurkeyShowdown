using UnityEngine;

public class SonsVites : MonoBehaviour
{
    public Rigidbody2D Rb;
   bool _active=true;
   public GameObject tempAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Rb.linearVelocity.magnitude > 2)
            {
                
                
                
                if(!_active)
                {
                     AudioManager.Instance.PlaySFX("vitesse",transform.position);
                tempAudio = AudioManager.Instance.tempAudio;
                tempAudio.transform .SetParent(transform);
                _active=false;
                }
               
                
            

            }
            if(Rb.linearVelocity.magnitude <= 2)
            {
                _active=true;
            }

          

            
            
           
            
    }

}
