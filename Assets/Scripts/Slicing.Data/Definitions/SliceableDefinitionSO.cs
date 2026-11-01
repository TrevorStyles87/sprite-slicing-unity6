using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Definition/Sliceable Definition")]
    public class SliceableDefinitionSO : ScriptableObject
    {
        public Sprite Sprite;
        public int MaxHealth = 1;
        public bool CanSlice = true;
        public bool CanBeDamaged = true;
        public float DamageMultiplier = 1f;
        public int ScoreValue = 10;
        public float MassScale = 1f;
        public Vector2 Pivot = Vector2.zero;

        public bool Validate()
        {
            return Sprite != null && MaxHealth > 0 && MassScale > 0f;
        }
    }
}
