using System;
using System.Threading.Tasks;
using Debugger;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class receivingJSON : MonoBehaviour
{
    private string url = "http://localhost/PHP/playerJSONToUnity.php";
    public TextMeshProUGUI text;
    
    private void Start()
    {
        //StartCoroutine(FetchJSONValue());
    }

   /* public IEnumerator FetchJSONValue()
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
    }*/
    
   
   public async Task<PlayerJSON> FetchJSONValue()
   {
       using (UnityWebRequest request = UnityWebRequest.Get(url))
       {
           await request.SendWebRequest();
           if (request.result != UnityWebRequest.Result.Success)
           {
               text.text = "tmauvais";
               return null;
           }
           else
           {
               string jsonval = request.downloadHandler.text;
               Debug.Log("Received JSON: " + jsonval);

               // Remove any invisible characters
               jsonval = jsonval.Trim().Replace("\uFEFF", "");

               try
               {
                   PlayerJSON playerJSON = JsonUtility.FromJson<PlayerJSON>(jsonval);
                   Debug.Log("Deserialized JSON: " + JsonUtility.ToJson(playerJSON));
                   return playerJSON;
               }
               catch (ArgumentException e)
               {
                   Debug.LogError("JSON parse error: " + e.Message);
                   return null;
               }
           }
       }
   }
}
