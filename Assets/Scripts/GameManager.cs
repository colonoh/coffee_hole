using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public event Action<int> OnScoreChanged;

    private AudioSource audioSource;
    private AudioClip scoreSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        scoreSound = GenerateCashRegisterSound();
    }

    public void AddScore(int points)
    {
        Score += points;
        OnScoreChanged?.Invoke(Score);
        audioSource.PlayOneShot(scoreSound);
    }

    private static AudioClip GenerateCashRegisterSound()
    {
        int sampleRate = 44100;
        float duration = 0.6f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        // Use a seed for deterministic "noise"
        System.Random rng = new System.Random(42);

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;

            // --- Mechanical "cha" (short percussive click) ---
            float cha = 0f;
            if (t < 0.04f)
            {
                float env = 1f - t / 0.04f;
                cha = ((float)rng.NextDouble() * 2f - 1f) * env * 0.4f;
            }

            // --- Bell "ching" (bright metallic ring) ---
            float ching1 = Mathf.Sin(2f * Mathf.PI * 1320f * t) * 0.4f;   // fundamental
            ching1 += Mathf.Sin(2f * Mathf.PI * 2640f * t) * 0.2f;         // 2nd harmonic
            ching1 += Mathf.Sin(2f * Mathf.PI * 3960f * t) * 0.1f;         // 3rd harmonic
            ching1 *= Mathf.Exp(-t * 7f);

            // --- Second higher ding (delayed 80ms) ---
            float t2 = t - 0.08f;
            float ching2 = 0f;
            if (t2 > 0f)
            {
                ching2 = Mathf.Sin(2f * Mathf.PI * 1760f * t2) * 0.35f;
                ching2 += Mathf.Sin(2f * Mathf.PI * 3520f * t2) * 0.15f;
                ching2 *= Mathf.Exp(-t2 * 6f);
            }

            samples[i] = Mathf.Clamp((cha + ching1 + ching2) * 0.7f, -1f, 1f);
        }

        AudioClip clip = AudioClip.Create("CashRegister", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
