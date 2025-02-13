using System.Collections;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class JSONSender : NetworkBehaviour
{
// Keep URL as it is
    private string url = "http://192.168.1.237/PHP/playerJSONFromUnity.php";

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

            // prevent any leftover after using Dispose on UnityWebRequest
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
