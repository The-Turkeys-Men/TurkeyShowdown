using System;
using Unity.Netcode;
using UnityEngine;

public class KillFeedPlayerActivator : NetworkBehaviour
{
    [SerializeField] private GameObject _killfeedPanel;
    private void Start()
    {
        if (IsOwner)
        {
            KillFeedManager.Instance.SetPanel(_killfeedPanel);
        }
    }
}
