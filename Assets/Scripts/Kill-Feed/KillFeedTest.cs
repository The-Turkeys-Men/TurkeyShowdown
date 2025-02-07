using UnityEngine;
using System.Collections;

public class KillFeedTest : MonoBehaviour
{
    void Start()
    {
        if (KillFeedManager.Instance != null)
        {
            Debug.Log("KillFeedManager est initialisé !");
            StartCoroutine(AddKillsWithDelay());
        }
        else
        {
            Debug.LogError("KillFeedManager est NULL, vérifie qu'il est bien dans la scène !");
        }
    }

    IEnumerator AddKillsWithDelay()
    {
        yield return new WaitForSeconds(1f);
        KillFeedManager.Instance.AddKill("1", "Stéphane", 1);

        yield return new WaitForSeconds(2f);
        KillFeedManager.Instance.AddKill("2", "John", 0);

        yield return new WaitForSeconds(0.5f);
        KillFeedManager.Instance.AddKill("3", "JON2", 4);

        yield return new WaitForSeconds(0.5f);
        KillFeedManager.Instance.AddKill("4", "Li", 2);
        
        yield return new WaitForSeconds(0.5f);
        KillFeedManager.Instance.AddKill("5", "Li", 2);
        
        yield return new WaitForSeconds(1f);
        KillFeedManager.Instance.AddKill("6", "MBAPE", 3);
    }
}
