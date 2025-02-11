using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject PanelTouche;
    public void ActivePanelTouche()
    {
        PanelTouche.SetActive(!PanelTouche.activeSelf);
    }
    
}
