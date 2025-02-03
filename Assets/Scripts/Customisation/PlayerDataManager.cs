using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public PlayerJSON playerData;
    
    private static PlayerDataManager instance = null;
    public static PlayerDataManager Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }
}
