using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedEntry : MonoBehaviour
{
    public TextMeshProUGUI KillerText { get; private set; }
    public TextMeshProUGUI KilledText { get; private set; }
    public Image WeaponImage { get; private set; }

    private void Awake()
    {
        KillerText = GetComponentInChildren<TextMeshProUGUI>();
        KilledText = GetComponentInChildren<TextMeshProUGUI>();
        WeaponImage = GetComponentInChildren<Image>();
    }

    public void Setup(string killerName, string killedName, int weaponId)
    {
        KillerText.text = killerName;
        KilledText.text = killedName;
        WeaponImage.sprite = WeaponDatabase.Instance.GetWeaponSprite(weaponId);
    }
}
