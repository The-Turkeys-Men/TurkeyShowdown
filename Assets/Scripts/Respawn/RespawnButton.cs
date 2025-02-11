using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RespawnButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _RespawnButtonText;
    [SerializeField] private Button _RespawnButton;
    [SerializeField]private float timer;
    [SerializeField] private GameObject DeathScreen;
    
    private float time;

    private void OnEnable()
    {
        time = timer;
        DeathScreen.SetActive(true);
    }

 
    
    private void Update()
    {
        if (!_RespawnButton.interactable)
        {
            time -= Time.deltaTime;
            _RespawnButtonText.text = (int)time + " Sec\nRespawn";
        }
        if (time <= 0 && !_RespawnButton.interactable)
        {
            _RespawnButtonText.text = "Respawn"; 
            _RespawnButton.interactable = true;
            Debug.Log("Respawn");
        }
    }
    
    public void Respawn()
    { 
        PlayerSpawner.SpawnerInstance.RespawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
       _RespawnButton.interactable = false;
       DeathScreen.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPressRaycast.gameObject == _RespawnButton.gameObject &&
            eventData.button == PointerEventData.InputButton.Left && 
            time <= 0)
        { 
            Respawn();
        }
    }
}
