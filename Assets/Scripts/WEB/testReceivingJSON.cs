using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class testReceivingJSON : MonoBehaviour
{
    private string url = "http://192.168.1.237/PHP/playerJSONToUnity.php";
    public TextMeshProUGUI text;
    
    private void Start()
    {
        StartCoroutine(FetchJSONValue());
    }

    IEnumerator FetchJSONValue()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                text.text = "tmauvais";
            }
            else
            {
                string jsonval = request.downloadHandler.text;
                Debug.Log(jsonval);
                text.text = jsonval;
                PlayerJSON playerJSON = JsonUtility.FromJson<PlayerJSON>(jsonval);
                Debug.LogError("userPseudo : " + playerJSON.Pseudo);
            }
        }
    }
}