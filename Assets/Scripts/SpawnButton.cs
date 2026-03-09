using UnityEngine;
using System.Collections;

public class SpawnButton : MonoBehaviour, IInteractable
{
    [SerializeField] private float spawnHeight = 3f;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0f, 1.5f);
    [SerializeField] private float cooldown = 0.5f;

    private Transform buttonTop;
    private float lastPressTime = -1f;

    private void Awake()
    {
        buttonTop = transform.Find("ButtonTop");
    }

    public void Interact()
    {
        if (Time.time - lastPressTime < cooldown) return;
        lastPressTime = Time.time;

        Vector3 spawnPos = transform.position + Vector3.up * spawnHeight + spawnOffset;
        CoffeeCupGenerator.Create(spawnPos);

        if (buttonTop != null)
            StartCoroutine(AnimatePress());
    }

    private IEnumerator AnimatePress()
    {
        Vector3 original = buttonTop.localPosition;
        buttonTop.localPosition = original + Vector3.down * 0.03f;
        yield return new WaitForSeconds(0.12f);
        buttonTop.localPosition = original;
    }
}
