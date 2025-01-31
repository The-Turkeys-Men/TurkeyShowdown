using Unity.Netcode;
using UnityEngine;

public class ColorChanger : NetworkBehaviour
{
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    
  [SerializeField] private Renderer[] renderers;

  public override void OnNetworkSpawn()
  {
      //playerColor.OnValueChanged += OnColorChanged;
      OnColorChanged(playerColor.Value);
  }

  private void OnDestroy()
  {
      //playerColor.OnValueChanged -= OnColorChanged;
  }

  private void OnColorChanged( Color newValue)
  {
      foreach (var renderer in renderers)
      {
          renderer.material.color = newValue;
      }
  }

  [ServerRpc]
  public void ChangeColorServerRpc(Color newValue)
  {
      playerColor.Value = newValue;
  }

  public void RequestColorChange(Color newValue)
  {
      if (IsOwner)
      {
          ChangeColorServerRpc(newValue);
      }
  }
}
