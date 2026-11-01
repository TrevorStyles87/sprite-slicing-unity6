using UnityEngine;

namespace Slicing.Data
{
    [CreateAssetMenu(menuName = "Slicing/Settings/Hit Feedback")]
    public class HitFeedbackSO : ScriptableObject
    {
        public Color FlashColor = Color.white;
        public float FlashDuration = 0.08f;
        public float CameraShakeStrength = 0.2f;
        public float CameraShakeDuration = 0.12f;
        public bool EnableHitStop = true;
        public bool EnableAudio = true;

        public bool Validate()
        {
            return FlashDuration >= 0f && CameraShakeDuration >= 0f;
        }
    }
}
