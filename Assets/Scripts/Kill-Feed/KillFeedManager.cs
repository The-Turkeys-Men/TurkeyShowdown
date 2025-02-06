using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KillFeedManager : MonoBehaviour
{
    public static KillFeedManager Instance { get; private set; }

    public GameObject killFeedPanel;
    public GameObject[] killFeedEntries;
    public TextMeshProUGUI[] killerTexts;
    public TextMeshProUGUI[] killedTexts;
    public Image[] weaponImages;

    private int maxKillEntries = 3;
    private float showDuration = 3f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Une instance de KillFeedManager existe déjà !");
            Destroy(gameObject);
            return;
        }

        if (killFeedPanel == null)
        {
            Debug.LogError("Le panel 'killFeedPanel' n'est pas assigné !");
            return;
        }

        int childCount = killFeedPanel.transform.childCount;
        int entriesToInitialize = Mathf.Min(childCount, maxKillEntries);

        killFeedEntries = new GameObject[entriesToInitialize];
        killerTexts = new TextMeshProUGUI[entriesToInitialize];
        killedTexts = new TextMeshProUGUI[entriesToInitialize];
        weaponImages = new Image[entriesToInitialize];

        for (int i = 0; i < entriesToInitialize; i++)
        {
            killFeedEntries[i] = killFeedPanel.transform.GetChild(i).gameObject;
            killerTexts[i] = killFeedEntries[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            killedTexts[i] = killFeedEntries[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            weaponImages[i] = killFeedEntries[i].transform.GetChild(2).GetComponent<Image>();
            killFeedEntries[i].SetActive(false);
        }
    }

    public void AddKill(string killerName, string killedName, int weaponID)
    {
        // Décale toutes les entrées existantes vers le bas
        for (int i = 0; i < maxKillEntries - 1; i++)
        {
            killerTexts[i].text = killerTexts[i + 1].text;
            killedTexts[i].text = killedTexts[i + 1].text;
            weaponImages[i].sprite = weaponImages[i + 1].sprite;
            killFeedEntries[i].SetActive(killFeedEntries[i + 1].activeSelf);
        }

        // Ajoute la nouvelle entrée en haut
        int topIndex = maxKillEntries - 1;
        killerTexts[topIndex].text = killerName;
        killedTexts[topIndex].text = killedName;
        weaponImages[topIndex].sprite = WeaponDatabase.Instance.GetWeaponSprite(weaponID);
        killFeedEntries[topIndex].SetActive(true);

        // Lance la suppression après un délai
        StartCoroutine(RemoveOldestKillAfterDelay());
    }

    private IEnumerator RemoveOldestKillAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        
        // Décale les entrées restantes vers le haut
        for (int i = 0; i < maxKillEntries - 1; i++)
        {
            killerTexts[i].text = killerTexts[i + 1].text;
            killedTexts[i].text = killedTexts[i + 1].text;
            weaponImages[i].sprite = weaponImages[i + 1].sprite;
            killFeedEntries[i].SetActive(killFeedEntries[i + 1].activeSelf);
        }

        // Désactive la dernière entrée
        killFeedEntries[maxKillEntries - 1].SetActive(false);
    }
}
