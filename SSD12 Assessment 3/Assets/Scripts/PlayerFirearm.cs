using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFirearm : PlayerItem
{
    // static constants
    [SerializeField] private float recoilRecovery_kP;
    private static float
        recoilVelocity_kP = 0.3f,
        aimShiftMultiplier = 0.08f,
        aimSwayMultiplier = 0.4f;

    private float 
        recoilRecovery_kI,
        recoilRecovery_kD;

    // recoil Z axis
    [SerializeField]
    private float recoilPosRecovery_kP;

    private float
        recoilPosRecovery_kI,
        recoilPosRecovery_kD;

    [SerializeField] private float spreadAmountHip;
    [SerializeField] private float spreadAmountADS;

    [SerializeField] private int numberOfProjectiles;

    [SerializeField] private Vector3 hitmarkerPosition;

    private bool aimedDownSights;
    private bool reloading;
    private bool reloadingFlag;

    // control variables
    [Header("Instantiation and Usage")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private GameObject useAudio;
    [SerializeField] private GameObject useEffect;
    [SerializeField] private GameObject casing;
    [SerializeField] private Transform barrelPoint;
    [SerializeField] private Transform casingPoint;

    private int ammo;
    [SerializeField] private bool chambered;
    private bool repetitiveReloading;
    private bool stopRepetitiveReload;

    [SerializeField] private bool useAnimationTrigger;

    [Header("Weapon stats")]
    [SerializeField] private Vector3 gunRecoil;
    [SerializeField] private Vector3 aimRecoil;
    [SerializeField] private Vector3 positionCoefficients;
    [SerializeField] private float itemRecovery_kP_ads;
    [SerializeField] private float cooldown;
    [SerializeField] private int maxAmmo; // magazine size
    [SerializeField] private bool useRepetitiveReload;
    [SerializeField] private float reloadTime;

    private Vector3
        gunRecoilVelocity,
        targetGunRecoilVelocity,
        aimRecoilVelocity,
        gunRecoilDisplacement; // to be applied to transform

    private PIDController recoilPID_x;
    private PIDController recoilPID_y;
    private PIDController recoilPID_z;

    [SerializeField]
    private AnimationCurve penetrationDropOff;
    [SerializeField]
    private AnimationCurve damageDropOff;

    [Header("Weapon Physical Aspects")]

    [SerializeField] private float weaponDrawLength;
    [SerializeField] private float weaponRaiseStart;
    [SerializeField] private float weaponRaiseCoefficient;
    [SerializeField] private float distanceMin;
    [SerializeField] private float weaponDrawRaiseDownCoeff;
    private float weaponDraw_kP = 0.13f;

    private float weaponDrawBackTarg;
    private float weaponDrawRaiseTarg;
    private float weaponDrawBack;
    private float weaponDrawRaise;

    private float weaponDrawRaiseDown;
    private bool weaponADSObstruction;
    private bool weaponFireObstruction;

    [Header("Projectile stats")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileDamage;
    [SerializeField] private float projectileGravity;
    [SerializeField] private float deformationSize;
    [SerializeField] private float deformationChance;
    [SerializeField] private float penetration;
    [SerializeField] protected bool useLimbMultipliers;

    [SerializeField] private GameObject reloadSound;

    private GameObject
        baseHitmarker,
        killHitmarker;

    [SerializeField] private GameObject sightAttachmentParent;
    [SerializeField] private int sightAttachment;
    private WeaponSight activeSight;

    [Header("Item Body for ADS")]
    [SerializeField] private Vector3 adsPosition;
    [SerializeField] private Vector3 adsRotation;
    private float adsWeight = 0;
    [SerializeField] private float ads_kP;

    public bool AimedDownSights { get => aimedDownSights; set => aimedDownSights = value; }
    public Projectile Projectile { get => projectile; set => projectile = value; }
    public Transform BarrelPoint { get => barrelPoint; set => barrelPoint = value; }
    public Vector3 AimRecoilVelocity { get => aimRecoilVelocity; set => aimRecoilVelocity = value; }
    public bool Reloading { get => reloading; set => reloading = value; }
    public bool Chambered { get => chambered; set => chambered = value; }
    public float Cooldown { get => cooldown; set => cooldown = value; }
    public bool UseAnimationTrigger { get => useAnimationTrigger; set => useAnimationTrigger = value; }
    protected float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }
    protected float ProjectileDamage { get => projectileDamage; set => projectileDamage = value; }
    protected float ProjectileGravity { get => projectileGravity; set => projectileGravity = value; }
    protected Vector3 GunRecoilVelocity { get => gunRecoilVelocity; set => gunRecoilVelocity = value; }
    protected Vector3 GunRecoil { get => gunRecoil; set => gunRecoil = value; }
    public Vector3 AimRecoil { get => aimRecoil; set => aimRecoil = value; }
    public GameObject UseAudio { get => useAudio; set => useAudio = value; }
    public GameObject UseEffect { get => useEffect; set => useEffect = value; }
    public int Ammo { get => ammo; set => ammo = value; }
    public WeaponSight ActiveSight { get => activeSight; set => activeSight = value; }
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public bool WeaponADSObstruction { get => weaponADSObstruction; set => weaponADSObstruction = value; }
    public bool WeaponFireObstruction { get => weaponFireObstruction; set => weaponFireObstruction = value; }
    public float SpreadAmountHip { get => spreadAmountHip; set => spreadAmountHip = value; }
    public float SpreadAmountADS { get => spreadAmountADS; set => spreadAmountADS = value; }
    public int NumberOfProjectiles { get => numberOfProjectiles; set => numberOfProjectiles = value; }
    public bool RepetitiveReloading { get => repetitiveReloading; set => repetitiveReloading = value; }
    public bool UseRepetitiveReload { get => useRepetitiveReload; set => useRepetitiveReload = value; }
    public bool ReloadingFlag { get => reloadingFlag; set => reloadingFlag = value; }
    public bool StopRepetitiveReload { get => stopRepetitiveReload; set => stopRepetitiveReload = value; }
    public float DeformationSize { get => deformationSize; set => deformationSize = value; }
    public float Penetration { get => penetration; set => penetration = value; }
    public float DeformationChance { get => deformationChance; set => deformationChance = value; }
    public AnimationCurve DamageDropOff { get => damageDropOff; set => damageDropOff = value; }
    public AnimationCurve PenetrationDropOff { get => penetrationDropOff; set => penetrationDropOff = value; }
    public GameObject SightAttachmentParent { get => sightAttachmentParent; set => sightAttachmentParent = value; }
    public int SightAttachment { get => sightAttachment; set => sightAttachment = value; }

    public override void Start()
    {
        base.Start();
        recoilPID_x = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
        recoilPID_y = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
        recoilPID_z = new PIDController(recoilPosRecovery_kP, recoilPosRecovery_kI, recoilPosRecovery_kD);

        //Chambered = true;
        recoilPID_x.Init();
        recoilPID_y.Init();
        recoilPID_z.Init();
        InitHitmarkers();
    }

    private void InitHitmarkers()
    {
        baseHitmarker = Instantiate(Resources.Load("Hitmarker Base"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
        baseHitmarker.transform.SetParent(BarrelPoint.transform);
        baseHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z);

        killHitmarker = Instantiate(Resources.Load("Hitmarker Kill"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
        killHitmarker.transform.SetParent(BarrelPoint.transform);
        killHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
    }

    private void UpdateHitmarkers()
    {
        RectTransform rTransformBase = baseHitmarker.GetComponent<RectTransform>(); // base hitmarker
        rTransformBase.eulerAngles = new Vector3(rTransformBase.eulerAngles.x, rTransformBase.eulerAngles.y, 0);

        RectTransform rTransformKill = killHitmarker.GetComponent<RectTransform>(); // kill hitmarker
        rTransformKill.eulerAngles = new Vector3(rTransformKill.eulerAngles.x, rTransformKill.eulerAngles.y, 0);
    }

    public override void UpdateTransforms()
    {
        if (aimedDownSights)
        {
            ItemSway *= aimSwayMultiplier;
            adsWeight += (1 - adsWeight) * ads_kP;
        }
        else { adsWeight += (0 - adsWeight) * ads_kP; }

        if (ActiveSight)
        {
            PositionOffset = new Vector3(
                (gunRecoilDisplacement.x * positionCoefficients.x) + ActiveSight.PositionalOffset.x + ItemShift.x + (adsWeight * adsPosition.x),
                (gunRecoilDisplacement.y * positionCoefficients.y) + ActiveSight.PositionalOffset.y + ItemShift.y - (weaponDrawRaise * weaponDrawRaiseDownCoeff) + (adsWeight * adsPosition.y),
                gunRecoilDisplacement.z + ActiveSight.PositionalOffset.z + weaponDrawBack + (adsWeight * adsPosition.z));
        } 
        else
        {
            PositionOffset = new Vector3(
                (gunRecoilDisplacement.x * positionCoefficients.x) + ItemShift.x + (adsWeight * adsPosition.x),
                (gunRecoilDisplacement.y * positionCoefficients.y) + ItemShift.y - (weaponDrawRaise * weaponDrawRaiseDownCoeff) + (adsWeight * adsPosition.y),
                gunRecoilDisplacement.z + weaponDrawBack + (adsWeight * adsPosition.z));
        }
        RotationOffset = new Vector3(ItemSwing_y + gunRecoilDisplacement.y + ItemSway.x + weaponDrawRaise + (adsWeight * adsRotation.x), (adsWeight * adsRotation.y), ItemSwing_x + gunRecoilDisplacement.x + ItemSway.y + (adsWeight * adsRotation.z));

        if (!aimedDownSights)
        {
            ItemSwing_x += (0 - ItemSwing_x) * ItemRecovery_kP;
            ItemSwing_y += (0 - ItemSwing_y) * ItemRecovery_kP;
        }
        else
        {
            ItemSwing_x += (0 - ItemSwing_x) * itemRecovery_kP_ads;
            ItemSwing_y += (0 - ItemSwing_y) * itemRecovery_kP_ads;
        }

        this.transform.localPosition = new Vector3(0, 0, 0);
        this.transform.Translate(PositionOffset, ForwardTransform);

        this.transform.localEulerAngles = ConvertVectorDirections(RotationOffset);
        UpdateHitmarkers();
        UpdateRecoil();
    }

    public void UpdateRecoil()
    {
        targetGunRecoilVelocity = new Vector3(recoilPID_x.GetOutput(-gunRecoilDisplacement.x), recoilPID_y.GetOutput(-gunRecoilDisplacement.y), recoilPID_z.GetOutput(-gunRecoilDisplacement.z));
        GunRecoilVelocity += new Vector3(
            (targetGunRecoilVelocity.x - GunRecoilVelocity.x) * recoilVelocity_kP,
            (targetGunRecoilVelocity.y - GunRecoilVelocity.y) * recoilVelocity_kP,
            (targetGunRecoilVelocity.z - GunRecoilVelocity.z) * recoilVelocity_kP
        );

        AimRecoilVelocity += new Vector3(
            (0 - AimRecoilVelocity.x) * recoilVelocity_kP,
            (0 - AimRecoilVelocity.y) * recoilVelocity_kP,
            (0 - AimRecoilVelocity.z) * recoilVelocity_kP
        );

        gunRecoilDisplacement += GunRecoilVelocity;
    }

    public override bool PrimaryUseItem(bool input)
    {
        if (input) StopRepetitiveReload = true;
        if (input && Chambered && !Sprinting && itemEnabled)
        {
            //print(" primary use  ");
            print("use item");
            if (aimedDownSights) Shoot(SpreadAmountADS);
            else Shoot(SpreadAmountHip);

            Chambered = false;
            UseAnimationTrigger = true;
            return true;
        }
        else
        {
            if (!Chambered && !IsInvoking("ChamberWeapon"))
            {
                Invoke("ChamberWeapon", Cooldown);
            }
        }
        return false;
    }

    public override void SecondaryUseItem(bool input)
    {
        if (!WeaponADSObstruction && !Sprinting)
        {
            AimedDownSights = input;
        } 
        else
        {
            AimedDownSights = false;
        }
    }

    public virtual void Shoot(float spreadAmount)
    {
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            if (Projectile)
            {
                Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
                spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers);
                spawnedProjectile.Item = this;
                spawnedProjectile.transform.Rotate(new Vector3(Random.Range(-spreadAmount / 2, spreadAmount / 2), Random.Range(-spreadAmount / 2, spreadAmount / 2), 0));
                if (GameObject.Find("Temp")) spawnedProjectile.transform.parent = GameObject.Find("Temp").transform;
            }
        }

        GunRecoilVelocity += new Vector3 (Random.Range(-GunRecoil.x, GunRecoil.x), GunRecoil.y, GunRecoil.z);
        AimRecoilVelocity += new Vector3(Random.Range(-AimRecoil.x, AimRecoil.x), AimRecoil.y, Random.Range(-AimRecoil.z, AimRecoil.z));
        if (UseAudio)
        {
            GameObject sound = Instantiate(UseAudio, BarrelPoint.transform.position, transform.rotation);
            sound.transform.parent = barrelPoint.transform;
        }
        if (UseEffect)
        {
            GameObject effect = Instantiate(UseEffect, BarrelPoint.transform.position, BarrelPoint.transform.rotation);
            if (GameObject.Find("Temp")) effect.transform.parent = GameObject.Find("Temp").transform;
        }
        DepleteAmmo(1);
        ProduceCasing();
    }

    public virtual void Shoot()
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            if (Projectile)
            {
                Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
                spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, DeformationSize, Penetration, DeformationChance, DamageDropOff, PenetrationDropOff, useLimbMultipliers);
                spawnedProjectile.Item = this;
                if (GameObject.Find("Temp")) spawnedProjectile.transform.parent = GameObject.Find("Temp").transform;
            }
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
        ProduceCasing();
    }

    protected void ProduceCasing()
    {
        if (casing)
        {
            GameObject newCasing = Instantiate(casing, casingPoint.transform.position, casingPoint.transform.rotation);
            if (GameObject.Find("Temp")) newCasing.transform.parent = GameObject.Find("Temp").transform;
        }
    }

    public virtual void ChamberWeapon()
    {
        if (Ammo > 0 && !Reloading)
        {
            Chambered = true;          
        }
    }

    public void DepleteAmmo(int amount)
    {
        Ammo -= amount;
    }

    public virtual void CallReload()
    {
        if (!UseRepetitiveReload)
        {
            if (!Reloading)
            {
                StopRepetitiveReload = false;
                reloadingFlag = false;
                Chambered = false;
                Reloading = true;
                Invoke("CompleteReload", reloadTime);
                if (reloadSound)
                {
                    GameObject reloadAudio = Instantiate(reloadSound, this.transform.position, transform.rotation);
                    reloadAudio.transform.SetParent(this.transform);
                }
            } 
        }
        else if (!repetitiveReloading)
        {
            StopRepetitiveReload = false;
            Reloading = true;
            Chambered = false;
            RepetitiveReloading = true;
            Invoke("RepetitiveReload", reloadTime);
            if (reloadSound)
            {
                GameObject reloadAudio = Instantiate(reloadSound, this.transform.position, transform.rotation);
                reloadAudio.transform.SetParent(this.transform);
            }
        }
    }

    public bool CanReload()
    {
        return ammo < maxAmmo;
    }

    public void CompleteReload()
    {

        Reloading = false;
        Ammo = MaxAmmo;
        Chambered = true;
    }

    public void RepetitiveReload()
    {
        ammo++;
        if (ammo >= maxAmmo || StopRepetitiveReload)
        {
            RepetitiveReloading = false;
            Reloading = false;
            chambered = true;
            StopRepetitiveReload = false;
        } 
        else
        {
            RepetitiveReloading = true;
            Invoke("RepetitiveReload", reloadTime);
            if (reloadSound)
            {
                GameObject reloadAudio = Instantiate(reloadSound, this.transform.position, transform.rotation);
                reloadAudio.transform.SetParent(this.transform);
            }
        }
    }

    public void HitAnotherTarget(bool killShot)
    {
        if (killShot)
        {
            killHitmarker.GetComponent<Hitmarker>().HitAnimation();
            print("hit another target kill");
        } 
        else 
        {
            baseHitmarker.GetComponent<Hitmarker>().HitAnimation();
            print("hit another target base");
        }
    }

    public void SetActiveSight()
    {
        if (SightAttachmentParent)
        {
            foreach (WeaponSight sight in SightAttachmentParent.GetComponentsInChildren<WeaponSight>())
            {
                if (sight.gameObject.activeSelf)
                {
                    ActiveSight = sight;
                }
            }
        }
    }

    public void SetAsLocalPlayer()
    {
        SetActiveSight();
        if (activeSight) ActiveSight.EnableCamera();
    }

    public override void UpdateItemShift(float x, float y)
    {
        if (!aimedDownSights)
        {
            TargItemShift = new Vector3(
                Mathf.Clamp(x * ItemShiftAmount_x, -ItemShiftCap, ItemShiftCap),
                Mathf.Clamp(y * ItemShiftAmount_y, -ItemShiftCap, ItemShiftCap),
                0
            );
        } 
        else
        {
            TargItemShift = new Vector3(
               Mathf.Clamp(x * ItemShiftAmount_x * aimShiftMultiplier, -ItemShiftCap, ItemShiftCap),
               Mathf.Clamp(y * ItemShiftAmount_y * aimShiftMultiplier, -ItemShiftCap, ItemShiftCap),
               0
           );
        }

        ItemShift += new Vector3(
            (TargItemShift.x - ItemShift.x) * ItemShift_kP,
            (TargItemShift.y - ItemShift.y) * ItemShift_kP,
            0
        );
    }

    public void SetSightAttachment(int sight)
    {
        print("set sight attachmment " + sight);
        sightAttachment = sight;
        if (SightAttachmentParent)
        {
            for (int i = 0; i < SightAttachmentParent.transform.childCount; i++)
            {
                WeaponSight weaponSight = SightAttachmentParent.transform.GetChild(i).GetComponent<WeaponSight>();
                if (i == sightAttachment) weaponSight.gameObject.SetActive(true);
                else weaponSight.gameObject.SetActive(false);
            }
        }
    }

    public WeaponSight GetSightAttachment()
    {
        print("sight targ " + sightAttachment);
        if (SightAttachmentParent)
        {
            for (int i = 0; i < SightAttachmentParent.transform.childCount; i++)
            {
                WeaponSight weaponSight = SightAttachmentParent.transform.GetChild(i).GetComponent<WeaponSight>();
                if (i == sightAttachment)
                {
                    print("found sight " + sightAttachment + " name " + weaponSight.name);
                    return weaponSight;
                }
            }
        }
        return null;
    }

    public void SetWeaponDraw(float distance)
    {
        float clampedDistance = distance;
        if (clampedDistance < distanceMin) clampedDistance = distanceMin;
        if (clampedDistance < weaponDrawLength && !Sprinting)
        {
            WeaponADSObstruction = true;
            weaponDrawBackTarg = -(weaponDrawLength - clampedDistance);
            if (clampedDistance < weaponRaiseStart)
            {
                weaponDrawRaiseTarg = -(weaponRaiseStart - clampedDistance) * weaponRaiseCoefficient;
                WeaponFireObstruction = true;
            }
            else
            {
                WeaponFireObstruction = false;
            }
        }
        else
        {
            weaponDrawBackTarg = 0;
            weaponDrawRaiseTarg = 0;
            WeaponADSObstruction = false;
            WeaponFireObstruction = false;
        }

        weaponDrawBack += (weaponDrawBackTarg - weaponDrawBack) * weaponDraw_kP;
        weaponDrawRaise += (weaponDrawRaiseTarg - weaponDrawRaise) * weaponDraw_kP;
    }
}
