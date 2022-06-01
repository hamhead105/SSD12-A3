using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Firearms which are rechambered after every use. For example, a bolt action rifle or a pump action shotgun would use this class, since they must be
 * recycled manually, with either the bolt or the pump.
 */
public class PlayerManualFirearm : PlayerFirearm
{
    private bool lastInput = false;
    private bool currentlyRecycling = false;

    [SerializeField] private float casingDelay;

    [SerializeField] private GameObject manualSound;

    public bool CurrentlyRecycling { get => currentlyRecycling; set => currentlyRecycling = value; }

    /*
     * The primary use of the item, call shooting subroutines
     * 
     * @param input - whether the input for this is enabled
     * @return - whether the input worked and the character used their items.
     */
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

    /*
     * Aim the weapon or scope in
     * 
     * @param input - whether the input is enabled
     */
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

    /*
     * Call the user to rechamber their weapon, through an animation or cooldown.
     */
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

    /*
     * Reload the weapon
     */
    public override void CallReload()
    {
        if (!CurrentlyRecycling && !Reloading)
        {
            base.CallReload();
        }
    }

    /*
     * Process of shooting the weapon, spawn a projectile, muzzleflash, audio and effects
     * 
     * @param spreadAmount - the angle at which the projectile should deviate from its original angle
     */
    public override void Shoot(float spreadAmount)
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
            spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, useLimbMultipliers, projectileLifeTime);
            spawnedProjectile.Item = this;
            spawnedProjectile.transform.Rotate(new Vector3(Random.Range(-spreadAmount / 2, spreadAmount / 2), Random.Range(-spreadAmount / 2, spreadAmount / 2), 0));
            spawnedProjectile.Init();
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

    /*
     * Process of shooting the weapon, spawn a projectile, muzzleflash, audio and effects
     */
    public override void Shoot()
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
            spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, useLimbMultipliers, projectileLifeTime);
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

    /*
     * Set the weapon chambered state to true.
     * Set whether the weapon is recycling to false.
     */
    public override void ChamberWeapon()
    {
        base.ChamberWeapon();
        CurrentlyRecycling = false;
    }
}
    