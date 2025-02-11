using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ActivationMap : NetworkBehaviour
{

    [SerializeField] private Image _fillAmountImage;
    [SerializeField] private GameObject _miniMap;
    
    private float _holdDuration = 1f;
    private float _holdTimer;

    private bool _isHolding;

    private ulong _localPlayerId;

    private void Start()
    {
        _fillAmountImage.type = Image.Type.Filled;
        _localPlayerId = NetworkManager.LocalClient.ClientId;
        Reset();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (collision.GetComponent<NetworkObject>().OwnerClientId != _localPlayerId)
            {
                return;
            }

            _isHolding = true;
            _holdTimer += Time.deltaTime;
            _fillAmountImage.fillAmount = _holdTimer / _holdDuration;
            if (_holdTimer >= _holdDuration)
            {
                _miniMap.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            if (collision.GetComponent<NetworkObject>().OwnerClientId != _localPlayerId)
            {
                return;
            }

            _isHolding = false;
            _miniMap.SetActive(false);
            Reset();
        }
    }

    private void Reset()
    {
        _holdTimer = 0f;
        _fillAmountImage.fillAmount = 0f;
    }
}
