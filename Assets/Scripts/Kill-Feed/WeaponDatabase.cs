using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance { get; private set; }

    [System.Serializable]
    public class WeaponData
    {
        public int WeaponId;
        public Sprite WeaponSprite;
    }

    public List<WeaponData> WeaponsList = new List<WeaponData>();

    private Dictionary<int, Sprite> _weaponDict = new Dictionary<int, Sprite>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var weapon in WeaponsList)
            {
                _weaponDict[weapon.WeaponId] = weapon.WeaponSprite;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetWeaponSprite(int weaponId)
    {
        return _weaponDict.TryGetValue(weaponId, out Sprite sprite) ? sprite : null;
    }
}
