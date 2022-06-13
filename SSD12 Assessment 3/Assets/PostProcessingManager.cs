using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
 * Handles game post processing i.e. screen effects such as different colour filters, blur etc
 */
public class PostProcessingManager : MonoBehaviour
{
    [SerializeField] private Volume normal;
    [SerializeField] private Volume hit;
    [SerializeField] private Volume dead;

    [SerializeField] private bool isDead;
    [SerializeField] private float hitRecovery_kP;

    [SerializeField] private float hitEffectMultiplier;

    private float normalStrength;
    private float hitStrength;
    private float deadStrength;
    private float suppressedStrength;

    public bool IsDead { get => isDead; set => isDead = value; }
    public float RecoveryRate { get => hitRecovery_kP; set => hitRecovery_kP = value; }
    public float NormalStrength { get => normalStrength; set => normalStrength = value; }
    public float HitStrength { get => hitStrength; set => hitStrength = value; }
    public float DeadStrength { get => deadStrength; set => deadStrength = value; }

    // Called at a fixed rate
    void FixedUpdate()
    {
        UpdatePostProcess();
    }

    /*
     * Update the weighting of different post processing profiles
     */
    public void UpdatePostProcess()
    {
        normal.weight += (NormalStrength - normal.weight) / 3;
        hit.weight += (HitStrength - hit.weight) / 3;
        dead.weight += (DeadStrength - dead.weight) / 3;

        if (IsDead)
        {
            deadStrength = 1;
            hitStrength = 0;
            normalStrength = 0;
        }
        else
        {
            deadStrength = 0;
            normalStrength = 1 - HitStrength;
            hitStrength += (0 - HitStrength) * hitRecovery_kP;
        }
    }

    /*
     * Apply hit post processing weight
     */
    public void Hit(float strength)
    {
        hitStrength += strength * hitEffectMultiplier;
        hitStrength = Mathf.Clamp(HitStrength, 0, 1.5f);
    }
}
