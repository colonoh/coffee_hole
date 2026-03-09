using UnityEngine;

/// <summary>
/// Trigger zone on top of the spawn button. Detects physics objects
/// falling onto it and the player (CharacterController) walking over it.
/// </summary>
public class ButtonTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null && other.GetComponent<CharacterController>() == null)
            return;

        var button = GetComponentInParent<SpawnButton>();
        if (button != null)
            button.Interact();
    }
}
