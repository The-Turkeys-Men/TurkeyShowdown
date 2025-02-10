using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RespawnButton : MonoBehaviour
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
        time -= Time.deltaTime;
        _RespawnButtonText.text = (int)time + " Sec\nRespawn";
        if (time <= 0)
        {
            _RespawnButtonText.text = "Respawn"; 
            _RespawnButton.interactable = true;
        }
    }
    
    public void Respawn()
    { 
        PlayerSpawner.SpawnerInstance.RespawnPlayerServerRpc();
       _RespawnButton.interactable = false;
       DeathScreen.SetActive(false);
    }
}
