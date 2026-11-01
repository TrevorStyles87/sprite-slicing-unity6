using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Slice Settings")]
    public class SliceSettingsSO : ScriptableObject
    {
        [Header("Limits")]
        public int MaxSlicesPerObject = 8;
        public int MaxPooledPieces = 256;
        public int MaxSimultaneousSliceables = 100;

        [Header("Gameplay")]
        public float MinimumSliceSpeed = 0.6f;
        public float MinimumSliceCooldown = 0.08f;
        public float MaxRecursiveDepth = 8f;
        public float MinimumPieceArea = 0.03f;
        public float AreaTolerance = 0.01f;

        public bool Validate()
        {
            return MaxSlicesPerObject > 0 && MaxPooledPieces > 0 && MaxSimultaneousSliceables > 0;
        }
    }
}
