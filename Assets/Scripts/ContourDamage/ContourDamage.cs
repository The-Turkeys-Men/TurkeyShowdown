using UnityEngine;
using Image = UnityEngine.UI.Image;

public class ContourDamage : MonoBehaviour
{
    private Image _panelImage;
    Color currentColor;
    void Start()
    {
        _panelImage = GetComponent<Image>();
        if (_panelImage != null)
        {
            currentColor = _panelImage.color;
        }
    }

    void Update()
    {
        Dega();
         
    }
    void Dega()
    {
        if(Input.GetKey(KeyCode.D))
        {
             currentColor.a = 1;

            
            _panelImage.color = currentColor;
        }
        DeDamaag();
    }
    void DeDamaag()
    {
        if (currentColor.a!=0)
        {
            currentColor.a-=Time.deltaTime;
            _panelImage.color= currentColor;

        }
    }

}
