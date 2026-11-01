using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Slicing.Runtime
{
    public class SwipeInputController : MonoBehaviour
    {
        public event Action<Vector2, Vector2> SwipePerformed;

        [SerializeField] private float minimumDistance = 0.12f;
        [SerializeField] private float maximumTime = 0.75f;

        private Vector2 startPosition;
        private float startTime;
        private bool tracking;

        private void Update()
        {
            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    startPosition = Mouse.current.position.ReadValue();
                    startTime = Time.unscaledTime;
                    tracking = true;
                }
                else if (Mouse.current.leftButton.isPressed && tracking)
                {
                    Vector2 current = Mouse.current.position.ReadValue();
                    float distance = Vector2.Distance(startPosition, current);
                    if (distance >= minimumDistance)
                    {
                        Vector2 delta = current - startPosition;
                        SwipePerformed?.Invoke(startPosition, current);
                        tracking = false;
                    }
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame && tracking)
                {
                    tracking = false;
                }
            }

            if (Touchscreen.current != null)
            {
                var touch = Touchscreen.current.primaryTouch;
                if (touch != null)
                {
                    if (touch.press.wasPressedThisFrame)
                    {
                        startPosition = touch.position.ReadValue();
                        startTime = Time.unscaledTime;
                        tracking = true;
                    }
                    else if (touch.press.isPressed && tracking)
                    {
                        Vector2 current = touch.position.ReadValue();
                        float distance = Vector2.Distance(startPosition, current);
                        if (distance >= minimumDistance)
                        {
                            SwipePerformed?.Invoke(startPosition, current);
                            tracking = false;
                        }
                    }
                    else if (touch.press.wasReleasedThisFrame && tracking)
                    {
                        tracking = false;
                    }
                }
            }
        }
    }
}
