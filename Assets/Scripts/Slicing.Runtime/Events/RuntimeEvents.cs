using System;
using UnityEngine;

namespace Slicing.Runtime
{
    public enum SliceModeType
    {
        Slice,
        Melee,
        Hybrid
    }

    public struct SwipeHitEvent
    {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Direction;
        public float Speed;
        public float Length;
    }

    public struct HitEvent
    {
        public GameObject Target;
        public Vector2 WorldPoint;
        public float Damage;
    }

    public struct SliceEvent
    {
        public GameObject Parent;
        public int SliceCount;
        public float Area;
    }

    public struct ComboEvent
    {
        public int CurrentCombo;
        public int Score;
    }

    public struct ModeEvent
    {
        public SliceModeType Mode;
    }

    public class RuntimeEventBus : MonoBehaviour
    {
        public static RuntimeEventBus Instance { get; private set; }

        public event Action<SwipeHitEvent> SwipeHit;
        public event Action<HitEvent> Hit;
        public event Action<SliceEvent> Slice;
        public event Action<ComboEvent> Combo;
        public event Action<ModeEvent> Mode;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PublishSwipeHit(SwipeHitEvent evt)
        {
            SwipeHit?.Invoke(evt);
        }

        public void PublishHit(HitEvent evt)
        {
            Hit?.Invoke(evt);
        }

        public void PublishSlice(SliceEvent evt)
        {
            Slice?.Invoke(evt);
        }

        public void PublishCombo(ComboEvent evt)
        {
            Combo?.Invoke(evt);
        }

        public void PublishMode(ModeEvent evt)
        {
            Mode?.Invoke(evt);
        }
    }
}
