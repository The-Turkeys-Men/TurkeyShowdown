using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Debugger
{
    public class DebuggerConsole : NetworkBehaviour
    {
        public static DebuggerConsole Instance;
        [SerializeField] private TextMeshProUGUI _debugText;
        [SerializeField] private CanvasGroup _canvasGroup;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void Log(string message)
        {
            _debugText.text = message + "\n" + _debugText.text;
        }

        [ServerRpc(RequireOwnership = false)]
        public void LogServerRpc(string message)
        {
            LogClientRpc(message);
        }
        
        [ClientRpc(RequireOwnership = false)]
        public void LogClientRpc(string message)
        {
            Log(message);
        }
    }
}