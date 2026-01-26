using UnityEngine;

namespace JankyStartupWindow
{
    [CreateAssetMenu(fileName = "JankyStartupWindowInfo", menuName = "JankyStartupWindow/Startup Window Info")]
    public class JankyStartupWindowInfo : ScriptableObject
    {
        public string title;
        public Sprite logo;
        [TextArea]
        public string description;
        public bool viewInAssetStoreVisible;
        public string viewInAssetStoreLink;
        public bool documentationVisible;
        public string documentationLink;
        public bool exampleVisible;
        public string exampleLink;
    }
}
