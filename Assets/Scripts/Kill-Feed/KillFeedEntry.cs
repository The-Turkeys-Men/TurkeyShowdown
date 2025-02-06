using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedEntry : MonoBehaviour
{
    public TextMeshProUGUI killerText;
    public TextMeshProUGUI killedText;
    public Image weaponImage;

    public void Setup(string killerName, string killedName, int weaponID)
    {
        killerText.text = killerName;
        killedText.text = killedName;
        weaponImage.sprite = WeaponDatabase.Instance.GetWeaponSprite(weaponID);
    }
}
