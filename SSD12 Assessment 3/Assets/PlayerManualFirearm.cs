using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManualFirearm : PlayerFirearm
{
    private bool lastInput = false;
    private bool currentlyRecycling = false;

    [SerializeField] private float casingDelay;

    [SerializeField] private GameObject manualSound;

    public bool CurrentlyRecycling { get => currentlyRecycling; set => currentlyRecycling = value; }

    public override bool PrimaryUseItem(bool input)
    {
        if (input) StopRepetitiveReload = true;
        if (input && Chambered && !Sprinting && !WeaponFireObstruction && itemEnabled)
        {
            if (AimedDownSights) Shoot(SpreadAmountADS);
            else Shoot(SpreadAmountHip);
            Chambered = false;
            return true;
        }
        if (!input && !CurrentlyRecycling && !Reloading && !Sprinting && !Chambered && itemEnabled)
        {
            RecycleFirearm();
        }
        lastInput = input;
        return false;
    }

    public override void SecondaryUseItem(bool input)
    {
        if (!Sprinting && !WeaponADSObstruction)
        {
            AimedDownSights = input;
        }
        else
        {
            AimedDownSights = false;
        }
    }

    private void RecycleFirearm()
    {
        if (Ammo > 0)
        {
            CurrentlyRecycling = true;
            if (manualSound)
            {
                GameObject manualAudio = Instantiate(manualSound, this.transform.position, transform.rotation);
                manualAudio.transform.SetParent(this.transform);
            }
            Invoke("ChamberWeapon", Cooldown);
            Invoke("ProduceCasing", casingDelay);
            UseAnimationTrigger = true; // used for rechamber animation in this case
        }
    }

    public override void CallReload()
    {
        if (!CurrentlyRecycling && !Reloading)
        {
            base.CallReload();
        }
    }

    public override void Shoot(float spreadAmount)
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
            spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers);
            spawnedProjectile.Item = this;
            spawnedProjectile.transform.Rotate(new Vector3(Random.Range(-spreadAmount / 2, spreadAmount / 2), Random.Range(-spreadAmount / 2, spreadAmount / 2), 0));
            if (GameObject.Find("Temp")) spawnedProjectile.transform.parent = GameObject.Find("Temp").transform;
        }

        GunRecoilVelocity += new Vector3(Random.Range(-GunRecoil.x, GunRecoil.x), GunRecoil.y, GunRecoil.z);
        AimRecoilVelocity += new Vector3(Random.Range(-AimRecoil.x, AimRecoil.x), AimRecoil.y, Random.Range(-AimRecoil.z, AimRecoil.z));
        if (UseAudio)
        {
            GameObject sound = Instantiate(UseAudio, BarrelPoint.transform.position, transform.rotation);
            if (GameObject.Find("Temp")) sound.transform.parent = GameObject.Find("Temp").transform;
        }
        if (UseEffect)
        {
            GameObject effect = Instantiate(UseEffect, BarrelPoint.transform.position, BarrelPoint.transform.rotation);
            if (GameObject.Find("Temp")) effect.transform.parent = GameObject.Find("Temp").transform;
        }
        DepleteAmmo(1);
    }

    public override void Shoot()
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
            spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers);
            spawnedProjectile.Item = this;
            if (GameObject.Find("Temp")) spawnedProjectile.transform.parent = GameObject.Find("Temp").transform;
        }

        GunRecoilVelocity += new Vector3(Random.Range(-GunRecoil.x, GunRecoil.x), GunRecoil.y, GunRecoil.z);
        AimRecoilVelocity += new Vector3(Random.Range(-AimRecoil.x, AimRecoil.x), AimRecoil.y, Random.Range(-AimRecoil.z, AimRecoil.z));
        if (UseAudio)
        {
            GameObject sound = Instantiate(UseAudio, BarrelPoint.transform.position, transform.rotation);
            if (GameObject.Find("Temp")) sound.transform.parent = BarrelPoint.transform;
        }
        if (UseEffect)
        {
            GameObject effect = Instantiate(UseEffect, BarrelPoint.transform.position, BarrelPoint.transform.rotation);
            if (GameObject.Find("Temp")) effect.transform.parent = GameObject.Find("Temp").transform;
        }
        DepleteAmmo(1);
    }

    public override void ChamberWeapon()
    {
        base.ChamberWeapon();
        CurrentlyRecycling = false;
    }
}
    