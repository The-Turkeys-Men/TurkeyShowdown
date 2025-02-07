using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class receivingJSON : MonoBehaviour
{
    private string url = "http://192.168.1.237/PHP/playerJSONToUnity.php";
    public TextMeshProUGUI text;
    
    private void Start()
    {
       // StartCoroutine(FetchJSONValue());
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
                Debug.Log(jsonval);
                text.text = jsonval;
                PlayerJSON playerJSON = JsonUtility.FromJson<PlayerJSON>(jsonval);
                Debug.LogError("userPseudo : " + playerJSON.Pseudo);
                return playerJSON;
            }
        }
    }
}
