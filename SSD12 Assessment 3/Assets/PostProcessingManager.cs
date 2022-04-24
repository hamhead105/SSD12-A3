using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume normal;
    [SerializeField] private Volume hit;
    [SerializeField] private Volume dead;
    [SerializeField] private Volume insane;
    [SerializeField] private Volume suppressed;

    [SerializeField] private bool isDead;
    [SerializeField] private float hitRecoveryRate;
    [SerializeField] private float suppressedRecoveryRate;

    [SerializeField] private float hitEffectMultiplier;
    [SerializeField] private float suppressedEffectMultiplier;

    private float normalStrength;
    private float hitStrength;
    private float deadStrength;
    private float suppressedStrength;

    private MotionBlur insaneMotionBlur;

    [Header("Insanity Effect Filter")]
    [SerializeField] private float insaneMotionBlurAmount;
    [SerializeField] private float insanityLevel;
    private float insaneWeight;
    private float insaneWeightTarget;
    [SerializeField] private float insaneWeight_kP;
    [SerializeField] private float insaneWeightPulseSpeed;
    [SerializeField] private float insaneWeightPulseAmount;

    public float InsanityLevel { get => insanityLevel; set => insanityLevel = value; }
    public Volume Normal { get => normal; set => normal = value; }
    public Volume Hit1 { get => hit; set => hit = value; }
    public Volume Dead { get => dead; set => dead = value; }
    public Volume Insane { get => insane; set => insane = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public float RecoveryRate { get => hitRecoveryRate; set => hitRecoveryRate = value; }
    public float NormalStrength { get => normalStrength; set => normalStrength = value; }
    public float HitStrength { get => hitStrength; set => hitStrength = value; }
    public float DeadStrength { get => deadStrength; set => deadStrength = value; }
    public MotionBlur InsaneMotionBlur { get => insaneMotionBlur; set => insaneMotionBlur = value; }
    public float SuppressedStrength { get => suppressedStrength; set => suppressedStrength = value; }
    public Volume Suppressed { get => suppressed; set => suppressed = value; }

    // Update is called once per frame
    void FixedUpdate()
    {
        //PostProcessUpdate();
    }

    private void PostProcessUpdate()
    {
        Normal.weight += (NormalStrength - Normal.weight) / 3;
        Hit1.weight += (HitStrength - Hit1.weight) / 3;
        Dead.weight += (DeadStrength - Dead.weight) / 3;
        Suppressed.weight += (SuppressedStrength - Suppressed.weight) / 3;

        if (IsDead)
        {
            DeadStrength = 1;
            HitStrength = 0;
            NormalStrength = 0;
            SuppressedStrength = 0;
        }
        else
        {
            DeadStrength = 0;
            NormalStrength = 1 - HitStrength;
            HitStrength += (0 - HitStrength) * hitRecoveryRate;
            SuppressedStrength += (0 - SuppressedStrength) * suppressedRecoveryRate;
        }
    }

    public void Hit(float strength)
    {
        HitStrength += strength * hitEffectMultiplier;
        HitStrength = Mathf.Clamp(HitStrength, 0, 1.5f);
    }

    public void UpdateInsanityEffect()
    {
        Insane.profile.TryGet(out insaneMotionBlur);
        InsaneMotionBlur.intensity.value = insaneMotionBlurAmount;
        Insane.weight = insaneWeight;

        if (insanityLevel == 1) insaneWeightTarget = 0.2f;
        if (insanityLevel == 2) insaneWeightTarget = 0.6f;
        if (insanityLevel == 3) insaneWeightTarget = 1f;

        if (insanityLevel > 0)
        { 
            insaneWeight += ((insaneWeightTarget - insaneWeight) * insaneWeight_kP) + (Mathf.Sin(Time.time * insaneWeightPulseSpeed) * insaneWeightPulseAmount);
            insaneWeight = Mathf.Clamp(insaneWeight, 0, 1);
        }     
        else { insaneWeight = 0; }
    }
}
