using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static WeaponDatabase Instance { get; private set; }

    [System.Serializable]
    public class WeaponData
    {
        public int weaponID;
        public Sprite weaponSprite;
    }

    public List<WeaponData> weaponsList = new List<WeaponData>();

    private Dictionary<int, Sprite> weaponDict = new Dictionary<int, Sprite>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var weapon in weaponsList)
            {
                weaponDict[weapon.weaponID] = weapon.weaponSprite;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Sprite GetWeaponSprite(int weaponID)
    {
        return weaponDict.TryGetValue(weaponID, out Sprite sprite) ? sprite : null;
    }
}
