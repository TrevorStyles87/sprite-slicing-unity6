using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Blade Settings")]
    public class BladeSettingsSO : ScriptableObject
    {
        public float Length = 1.2f;
        public float Width = 0.08f;
        public float HitCooldown = 0.05f;
        public float MinimumSpeed = 1.2f;
        public float Damage = 1f;
        public float KnockbackStrength = 2f;

        public bool Validate()
        {
            return Length > 0f && Width > 0f && MinimumSpeed > 0f;
        }
    }
}
