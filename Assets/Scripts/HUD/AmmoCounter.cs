using System;
using TMPro;
using UnityEngine;

namespace HUD
{
    public class AmmoCounter : MonoBehaviour
    {
        [SerializeField] private PlayerWeapon _playerWeapon;

        [SerializeField] private TextMeshProUGUI _ammoText;
        private void Awake()
        {
            if (!_playerWeapon) _playerWeapon = GetComponentInParent<PlayerWeapon>();
        }

        private void Update()
        {
            if (_playerWeapon.EquipedWeapon)
            {
                _ammoText.gameObject.SetActive(true);
                _ammoText.text = _playerWeapon.EquipedWeapon.Ammo.Value.ToString();
            }
            else
            {
                _ammoText.gameObject.SetActive(false);
            }
        }
    } 
}