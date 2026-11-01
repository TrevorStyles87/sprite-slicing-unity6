using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Melee Settings")]
    public class MeleeSettingsSO : ScriptableObject
    {
        public float Damage = 10f;
        public float Knockback = 4f;
        public float InvulnerabilityWindow = 0.2f;
        public float HitStun = 0.1f;
        public float Radius = 0.4f;

        public bool Validate()
        {
            return Damage > 0f && Knockback >= 0f && Radius > 0f;
        }
    }
}
