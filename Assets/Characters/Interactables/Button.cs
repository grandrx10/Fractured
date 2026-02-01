using System.Collections;
using UnityEngine;

namespace Characters.Interactables
{
    public class ButtonInteractable : Interactable
    {
        [Header("Button Settings")]
        [Tooltip("The object that visually moves when pressed")]
        public Transform buttonMesh;

        [Tooltip("How far the button moves when pressed")]
        public float pressDistance = 0.1f;

        [Tooltip("Time (seconds) to fully press the button")]
        public float pressDuration = 0.15f;

        [Tooltip("Time (seconds) to fully release the button")]
        public float releaseDuration = 0.2f;

        [Tooltip("Should the button stay pressed")]
        public bool stayPressed = false;

        private Vector3 initialLocalPosition;
        private bool isPressed = false;
        private Coroutine pressRoutine;

        private void Awake()
        {
            if (buttonMesh == null)
                buttonMesh = transform;

            initialLocalPosition = buttonMesh.localPosition;
        }

        public override void Interact(GameObject player)
        {
            base.Interact(player);
            if (!canInteract || isPressed)
                return;

            if (pressRoutine != null)
                StopCoroutine(pressRoutine);

            pressRoutine = StartCoroutine(PressButton());
        }

        private IEnumerator PressButton()
        {
            isPressed = true;

            Vector3 pressedPosition =
                initialLocalPosition - buttonMesh.forward * pressDistance;

            // Press in
            yield return MoveButton(
                buttonMesh.localPosition,
                pressedPosition,
                pressDuration
            );

            Action();

            // Stay pressed or release
            if (!stayPressed)
            {
                yield return MoveButton(
                    buttonMesh.localPosition,
                    initialLocalPosition,
                    releaseDuration
                );

                isPressed = false;
            }
        }

        private IEnumerator MoveButton(Vector3 from, Vector3 to, float duration)
        {
            if (duration <= 0f)
            {
                buttonMesh.localPosition = to;
                yield break;
            }

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                buttonMesh.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }

            buttonMesh.localPosition = to;
        }

        // Optional: allow reset from code
        public void ResetButton()
        {
            if (pressRoutine != null)
                StopCoroutine(pressRoutine);

            buttonMesh.localPosition = initialLocalPosition;
            isPressed = false;
        }

        public virtual void Action()
        {
            
        }
    }
}
