using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Swipe Mode")]
    public class SwipeModeSO : ScriptableObject
    {
        public bool AllowSliceMode = true;
        public bool AllowMeleeMode = true;
        public bool AllowHybridMode = true;
        public float SwipeThreshold = 0.1f;
        public float OneFingerOnly = 1f;

        public bool Validate()
        {
            return SwipeThreshold > 0f;
        }
    }
}
