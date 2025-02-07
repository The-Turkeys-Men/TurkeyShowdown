using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KillFeedManager : MonoBehaviour
{
    public static KillFeedManager Instance { get; private set; }

    public GameObject KillFeedPanel;
    public GameObject[] KillFeedEntries;
    public TextMeshProUGUI[] KillerTexts;
    public TextMeshProUGUI[] KilledTexts;
    public Image[] WeaponImages;

    private int _maxKillEntries = 3; // Maximum number of kill feed entries
    private float _showDuration = 5f; // Display duration of each entry

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("KillFeedManager instantiated!");
        }
        else
        {
            Debug.LogError("An instance of KillFeedManager already exists!");
            Destroy(gameObject);
            return;
        }

        if (KillFeedPanel == null)
        {
            Debug.LogError("The 'KillFeedPanel' is not assigned!");
            return;
        }

        int childCount = KillFeedPanel.transform.childCount;
        int entriesToInitialize = Mathf.Min(childCount, _maxKillEntries);

        KillFeedEntries = new GameObject[entriesToInitialize];
        KillerTexts = new TextMeshProUGUI[entriesToInitialize];
        KilledTexts = new TextMeshProUGUI[entriesToInitialize];
        WeaponImages = new Image[entriesToInitialize];

        for (int i = 0; i < entriesToInitialize; i++)
        {
            KillFeedEntries[i] = KillFeedPanel.transform.GetChild(i).gameObject;
            KillerTexts[i] = KillFeedEntries[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            KilledTexts[i] = KillFeedEntries[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            WeaponImages[i] = KillFeedEntries[i].transform.GetChild(2).GetComponent<Image>();
            KillFeedEntries[i].SetActive(false);
        }
    }

    public void AddKill(string killerName, string killedName, int weaponID)
    {
        for (int i = 0; i < _maxKillEntries; i++)
        {
            if (!KillFeedEntries[i].activeSelf)
            {
                KillerTexts[i].text = killerName;
                KilledTexts[i].text = killedName;
                WeaponImages[i].sprite = WeaponDatabase.Instance.GetWeaponSprite(weaponID);
                KillFeedEntries[i].SetActive(true);
                StartCoroutine(RemoveOldestKillAfterDelay());
                return;
            }
        }

        for (int i = 0; i < _maxKillEntries - 1; i++)
        {
            KillerTexts[i].text = KillerTexts[i + 1].text;
            KilledTexts[i].text = KilledTexts[i + 1].text;
            WeaponImages[i].sprite = WeaponImages[i + 1].sprite;
            KillFeedEntries[i].SetActive(KillFeedEntries[i + 1].activeSelf);
        }

        int bottomIndex = _maxKillEntries - 1;
        KillerTexts[bottomIndex].text = killerName;
        KilledTexts[bottomIndex].text = killedName;
        WeaponImages[bottomIndex].sprite = WeaponDatabase.Instance.GetWeaponSprite(weaponID);
        KillFeedEntries[bottomIndex].SetActive(true);

        StartCoroutine(RemoveOldestKillAfterDelay());
    }

    private IEnumerator RemoveOldestKillAfterDelay()
    {
        yield return new WaitForSeconds(_showDuration);

        for (int i = 0; i < _maxKillEntries - 1; i++)
        {
            KillerTexts[i].text = KillerTexts[i + 1].text;
            KilledTexts[i].text = KilledTexts[i + 1].text;
            WeaponImages[i].sprite = WeaponImages[i + 1].sprite;
            KillFeedEntries[i].SetActive(KillFeedEntries[i + 1].activeSelf);
        }

        KillFeedEntries[_maxKillEntries - 1].SetActive(false);
    }
}