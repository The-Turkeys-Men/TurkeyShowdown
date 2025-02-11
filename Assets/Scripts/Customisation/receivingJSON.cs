using System;
using System.Threading.Tasks;
using Debugger;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class receivingJSON : NetworkBehaviour
{
    private string url = "http://192.168.1.237/PHP/playerJSONToUnity.php";
    
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
               return null;
           }
           else
           {
               string jsonval = request.downloadHandler.text;
               
               Debug.Log("Received JSON: " + jsonval);
               
               Debug.Log("Trying without trim from json: " + JsonUtility.FromJson<PlayerJSON>(jsonval));
               
               // Remove any invisible characters
               jsonval = jsonval.Trim().Replace("\uFEFF", "");
               Debug.Log("Json trimmed: " + jsonval);
       
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
