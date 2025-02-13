using UnityEngine;

namespace MapVote
{
    [CreateAssetMenu(fileName = "MapData", menuName = "MapData", order = 0)]
    public class MapData : ScriptableObject
    {
        public string SceneName = "Nom scene";
        public Sprite MapPreview;
        public string MapName = "Nom map";
    }
}