using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JSONSender : MonoBehaviour
{
// Keep URL as it is
    private string url = "http://192.168.1.237/PHP/playerJSONFromUnity.php";
    private void Start()
    {
        SendPlayerData();
    }

    public void SendPlayerData()
    {
        // Create PlayerJSON object dynamically based on actual player's data
        PlayerJSON player = new PlayerJSON()
        {
            Id = 4,
            Pseudo = "MonPseudo",   // Replace with dynamical data
            HighScore = 100,        // Replace with dynamical data
            ScoreTable = new int[] { 10, 20, 30 }, // Replace with dynamical data
            NbrVictory = 5,    // Replace with dynamical data
            NbrDefeat = 2,     // Replace with dynamical data
            Skins = new Skin[] // Replace with dynamical data
            {
                new Skin { Id = "skin1", Enabled = true },
                new Skin { Id = "skin2", Enabled = true },
                new Skin { Id = "skibidi", Enabled = true }
            }
        };
        // Convert the object to JSON and Send JSON to the server
        StartCoroutine(SendJsonToServer(JsonUtility.ToJson(player)));
    }

    public IEnumerator SendJsonToServer(string jsonData)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
        {
            www.uploadHandler = new UploadHandlerRaw(new UTF8Encoding().GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // send request and wait
            yield return www.SendWebRequest();

            while (!www.isDone)
                 yield return null;

            if (www.result != UnityWebRequest.Result.Success || www.isHttpError || www.isNetworkError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                // Log the response body and validate it if necessary
                Debug.Log("Received: " + www.downloadHandler.text);
                
                if(www.downloadHandler.text == "Expected response")
                {
                    // expected response handling
                }
                else
                {
                    // unexpected response handling
                }
            }

            // prevent any leftover after using Dispose on UnityWebRequest
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
