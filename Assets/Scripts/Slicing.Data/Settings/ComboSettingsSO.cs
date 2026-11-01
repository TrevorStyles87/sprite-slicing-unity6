using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Combo Settings")]
    public class ComboSettingsSO : ScriptableObject
    {
        public int BaseScore = 10;
        public float MultiplierIncrement = 0.2f;
        public float MaxMultiplier = 8f;
        public float ComboWindow = 1.5f;
        public bool Validate()
        {
            return BaseScore > 0 && MaxMultiplier > 0f && ComboWindow > 0f;
        }
    }
}
