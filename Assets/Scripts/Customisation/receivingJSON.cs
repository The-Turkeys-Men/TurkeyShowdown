using System;
using System.Threading.Tasks;
using Debugger;
using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class receivingJSON : NetworkBehaviour
{
    private string url = "http://192.168.1.237/PHP/playerJSONToUnity.php";
    
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
               
               // Remove any invisible characters
               jsonval = jsonval.Trim('\uFEFF', '\u200B', '\u200E', '\u200F');
       
               try
               {
                   PlayerJSON playerJSON = JsonUtility.FromJson<PlayerJSON>(jsonval);
                   return playerJSON;
               }
               catch (ArgumentException e)
               {
                   return null;
               }
           }
       }
   }
}
