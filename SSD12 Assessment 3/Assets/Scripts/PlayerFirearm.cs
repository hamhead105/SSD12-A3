using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Base class for player items which are firearms, I.e items which shoot projectiles on use, have ammunition etc.
 */
public class PlayerFirearm : PlayerItem
{
    // static constants
    protected static float
        recoilVelocity_kP = 0.3f,
        aimShiftMultiplier = 0.08f,
        aimSwayMultiplier = 0.4f,
        projectileLifeTime = 12,
        lookAtPointTransition_kP = 0.1f,
        lookAtPointShift_kP = 0.1f;

    private static Vector3 maxItemRot = new Vector3(80, 80, 0);

    [SerializeField] private float recoilRecovery_kP;
    [SerializeField]
    private float 
        recoilRecovery_kI = 0.04f,
        recoilRecovery_kD = 0.05f;

    // recoil Z axis
    [SerializeField]
    private float recoilPosRecovery_kP;
    [SerializeField]
    private float
        recoilPosRecovery_kI = 0.1f,
        recoilPosRecovery_kD = 0.1f;

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
    [SerializeField] protected bool useLimbMultipliers;

    [SerializeField] private GameObject reloadSound;

    private GameObject
        baseHitmarker,
        killHitmarker,
        reticle;

    [SerializeField] private GameObject sightAttachmentParent;
    [SerializeField] private int sightAttachment;
    private WeaponSight activeSight;

    [Header("Item Body for ADS")]
    [SerializeField] private Vector3 adsPosition;
    [SerializeField] private Vector3 adsRotation;
    [SerializeField] private Vector3 rotationOffsets; // used to aim the gun at where the camera is aiming;
    private float adsWeight = 0;
    [SerializeField] private float ads_kP;

    [SerializeField] protected Transform lookPoint;
    private float lookPointWeight; // for enabling and disabling target point aiming
    private Vector3 lookPointRotation;
    private Vector3 lookPointTargetRotation;

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
    public AnimationCurve DamageDropOff { get => damageDropOff; set => damageDropOff = value; }
    public AnimationCurve PenetrationDropOff { get => penetrationDropOff; set => penetrationDropOff = value; }
    public GameObject SightAttachmentParent { get => sightAttachmentParent; set => sightAttachmentParent = value; }
    public int SightAttachment { get => sightAttachment; set => sightAttachment = value; }

    // called when the item is first instantiated
    public override void Start()
    {
        base.Start();
        recoilPID_x = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
        recoilPID_y = new PIDController(recoilRecovery_kP, recoilRecovery_kI, recoilRecovery_kD);
        recoilPID_z = new PIDController(recoilPosRecovery_kP, recoilPosRecovery_kI, recoilPosRecovery_kD);

        recoilPID_x.Init();
        recoilPID_y.Init();
        recoilPID_z.Init();
        InitHitmarkers();

        if (User is Player) lookPoint = (User as Player).LookPoint;
    }

    /*
     * Called once per frame
     */
    public override void Update()
    {
        base.Update();
        UpdateLookPointWeights();
        UpdateCastBasedUI();
    }

    /*
     * Update world UI elements which are printed on the point at which the weapon is aimed. For example,
     * the player's reticle and hitmarkers. These are scaled based on the distance to the player to ensure the player sees the 
     * UI as a consistent size.
     */
    private void UpdateCastBasedUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(barrelPoint.transform.position, barrelPoint.transform.forward, out hit))
        {
            baseHitmarker.transform.position = hit.point;
            killHitmarker.transform.position = hit.point;
            reticle.transform.position = hit.point;
        }
        else
        {
            killHitmarker.transform.localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
            baseHitmarker.transform.localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
            reticle.transform.localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
        }
    }

    /*
     * Initialize hitmarkers
     */
    private void InitHitmarkers()
    {
        baseHitmarker = Instantiate(Resources.Load("Hitmarker Base"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
        baseHitmarker.transform.SetParent(BarrelPoint.transform);
        baseHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z);

        killHitmarker = Instantiate(Resources.Load("Hitmarker Kill"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
        killHitmarker.transform.SetParent(BarrelPoint.transform);
        killHitmarker.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);

        reticle = Instantiate(Resources.Load("Reticle"), BarrelPoint.transform.position, BarrelPoint.transform.rotation) as GameObject;
        reticle.transform.SetParent(BarrelPoint.transform);
        reticle.GetComponent<RectTransform>().localPosition = new Vector3(hitmarkerPosition.x, hitmarkerPosition.y, hitmarkerPosition.z - 0.1f);
    }

    /*
     * Update hitmarker transforms and get their components.
     */
    private void UpdateHitmarkersAndReticle()
    {
        if (User is Player) {
            RectTransform rTransformBase = baseHitmarker.GetComponent<RectTransform>(); // base hitmarker
            rTransformBase.eulerAngles = new Vector3(rTransformBase.eulerAngles.x, rTransformBase.eulerAngles.y, 0);

            RectTransform rTransformKill = killHitmarker.GetComponent<RectTransform>(); // kill hitmarker
            rTransformKill.eulerAngles = new Vector3(rTransformKill.eulerAngles.x, rTransformKill.eulerAngles.y, 0);

            if (aimedDownSights && (User as Player).PlayerCamera.FirstPersonMode) reticle.GetComponent<Reticle>().HideVisiblity();
            else reticle.GetComponent<Reticle>().EnableVisiblity();
        }
    }

    /*
     * Update the transform of this item. Move the item around based on additional movement and rotational vectors. For example, based on weapon recoil
     * the weapon needs to be shifted and rotated. 
     */
    public override void UpdateTransforms()
    {
        this.transform.LookAt(lookPoint);
        lookPointTargetRotation = transform.localEulerAngles;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);

        if (aimedDownSights)
        {
            ItemSway *= aimSwayMultiplier;
            adsWeight += (1 - adsWeight) * Mathf.Min(ads_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
        }
        else { adsWeight += (0 - adsWeight) * Mathf.Min(ads_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1); }

        if (ActiveSight)
        {
            PositionOffset = new Vector3(
                (gunRecoilDisplacement.x * positionCoefficients.x) + ActiveSight.PositionalOffset.x + ItemShift.x + (adsWeight * adsPosition.x),
                (gunRecoilDisplacement.y * positionCoefficients.y) + ActiveSight.PositionalOffset.y + ItemShift.y - (weaponDrawRaise * weaponDrawRaiseDownCoeff) + (adsWeight * adsPosition.y),
                gunRecoilDisplacement.z + ActiveSight.PositionalOffset.z + weaponDrawBack + (adsWeight * adsPosition.z)
            );
        }
        else
        {
            PositionOffset = new Vector3(
                (gunRecoilDisplacement.x * positionCoefficients.x) + ItemShift.x + (adsWeight * adsPosition.x),
                (gunRecoilDisplacement.y * positionCoefficients.y) + ItemShift.y - (weaponDrawRaise * weaponDrawRaiseDownCoeff) + (adsWeight * adsPosition.y),
                gunRecoilDisplacement.z + weaponDrawBack + (adsWeight * adsPosition.z)
            );
        }
        RotationOffset = new Vector3(ItemSwing_y + gunRecoilDisplacement.y + ItemSway.x + weaponDrawRaise + (adsWeight * adsRotation.x), (adsWeight * adsRotation.y), ItemSwing_x + gunRecoilDisplacement.x + ItemSway.y + (adsWeight * adsRotation.z));

        if (!aimedDownSights)
        {
            ItemSwing_x += (0 - ItemSwing_x) * Mathf.Min(ItemRecovery_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
            ItemSwing_y += (0 - ItemSwing_y) * Mathf.Min(ItemRecovery_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
        }
        else
        {
            ItemSwing_x += (0 - ItemSwing_x) * Mathf.Min(itemRecovery_kP_ads * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
            ItemSwing_y += (0 - ItemSwing_y) * Mathf.Min(itemRecovery_kP_ads * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
        }

        this.transform.localPosition = new Vector3(0, 0, 0);
        this.transform.Translate(PositionOffset, ForwardTransform);
        
        // interpolate the weapon to the point the camera is pointed
        if (User is Player) 
        {
            this.transform.localEulerAngles = new Vector3 (
                Mathf.LerpAngle(this.transform.localEulerAngles.x, lookPointRotation.x, lookPointWeight),
                Mathf.LerpAngle(this.transform.localEulerAngles.y, lookPointRotation.y, lookPointWeight),
                Mathf.LerpAngle(this.transform.localEulerAngles.z, lookPointRotation.z, lookPointWeight)
            );
            this.transform.Rotate(rotationOffsets * lookPointWeight);
            this.transform.localEulerAngles += ConvertVectorDirections(RotationOffset);
        }

        UpdateHitmarkersAndReticle();
        UpdateRecoil();
    }

    /*
     * Interpolate the weight of the look point. This controls how much effect the look point has. 
     * The look point is a transform which is placed at the point in which the camera is pointed at.
     */
    public void UpdateLookPointWeights()
    {

        if (User is Player)
        {
            if (Sprinting || (User as Player).PlayerCamera.FreeLook || (User as Player).PlayerCamera.FirstPersonMode || WeaponADSObstruction)
            {
                lookPointWeight += (0 - lookPointWeight) * Mathf.Min(lookAtPointTransition_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
            }
            else
            {
                lookPointWeight += (1 - lookPointWeight) * Mathf.Min(lookAtPointTransition_kP * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant, 1);
            }
            
            lookPointRotation = new Vector3(
                Mathf.LerpAngle(lookPointRotation.x, lookPointTargetRotation.x, lookAtPointShift_kP),
                Mathf.LerpAngle(lookPointRotation.y, lookPointTargetRotation.y, lookAtPointShift_kP),
                Mathf.LerpAngle(lookPointRotation.z, lookPointTargetRotation.z, lookAtPointShift_kP)
            );
        }
    }

    /*
     * Shift the displacement of the item based on the velocity of the weapon. 
     */
    public void UpdateRecoil()
    {
        targetGunRecoilVelocity = new Vector3(recoilPID_x.GetOutput(-gunRecoilDisplacement.x), recoilPID_y.GetOutput(-gunRecoilDisplacement.y), recoilPID_z.GetOutput(-gunRecoilDisplacement.z));
        // the transform of the firearm.
        GunRecoilVelocity += new Vector3(
            (targetGunRecoilVelocity.x - GunRecoilVelocity.x) * Mathf.Min(recoilVelocity_kP, 1),
            (targetGunRecoilVelocity.y - GunRecoilVelocity.y) * Mathf.Min(recoilVelocity_kP, 1),
            (targetGunRecoilVelocity.z - GunRecoilVelocity.z) * Mathf.Min(recoilVelocity_kP, 1)
        );

        // the transform of the player. This is where the player's screen is rotated upwards
        AimRecoilVelocity += new Vector3(
            (0 - AimRecoilVelocity.x) * Mathf.Min(recoilVelocity_kP, 1),
            (0 - AimRecoilVelocity.y) * Mathf.Min(recoilVelocity_kP, 1),
            (0 - AimRecoilVelocity.z) * Mathf.Min(recoilVelocity_kP, 1)
        ) * Time.deltaTime * GameManagement.DeltaTimeFrameSpeedConstant;

        gunRecoilDisplacement += GunRecoilVelocity;
    }

    /*
     * The main use of the item - Run subroutines for shooting and unchamber the weapon
     * 
     * @param input - whether the use input is enabled (left click)
     */
    public override bool PrimaryUseItem(bool input)
    {
        if (input) StopRepetitiveReload = true;
        if (input && Chambered && !Sprinting && itemEnabled && !WeaponFireObstruction)
        {
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

    /*
     * Aim the weapon or scope in
     * 
     * @param input - whether the use input is enabled (right click) 
     */
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

    /*
     * Process of shooting the weapon, spawn a projectile, muzzleflash, audio and effects
     * 
     * @param spreadAmount - the angle at which the projectile should deviate from its original angle
     */
    public virtual void Shoot(float spreadAmount)
    {
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            if (Projectile)
            {
                Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
                spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, useLimbMultipliers, projectileLifeTime);
                spawnedProjectile.Item = this;
                spawnedProjectile.transform.Rotate(new Vector3(Random.Range(-spreadAmount / 2, spreadAmount / 2), Random.Range(-spreadAmount / 2, spreadAmount / 2), 0));
                spawnedProjectile.Init();
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

    /*
     * Process of shooting the weapon, spawn a projectile, muzzleflash, audio and effects
     */
    public virtual void Shoot()
    {
        print("shoot C");
        for (int projectilesFired = 0; projectilesFired < NumberOfProjectiles; projectilesFired++)
        {
            if (Projectile)
            {
                Projectile spawnedProjectile = Instantiate(Projectile, BarrelPoint.position, BarrelPoint.transform.rotation);
                spawnedProjectile.SetProjectileStats(ProjectileSpeed, ProjectileDamage, ProjectileGravity, useLimbMultipliers, projectileLifeTime);
                spawnedProjectile.Init();
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

    /*
     * Produce a casing particle effect
     */
    protected void ProduceCasing()
    {
        if (casing)
        {
            GameObject newCasing = Instantiate(casing, casingPoint.transform.position, casingPoint.transform.rotation);
            if (GameObject.Find("Temp")) newCasing.transform.parent = GameObject.Find("Temp").transform;
        }
    }

    /*
     * Set the weapon's chambered state to true
     */
    public virtual void ChamberWeapon()
    {
        if (Ammo > 0 && !Reloading)
        {
            Chambered = true;          
        }
    }

    /*
     * Deplete the ammo amount 
     * 
     * @param amount - the number of rounds to deplete
     */
    public void DepleteAmmo(int amount)
    {
        Ammo -= amount;
    }

    /*
     * Initialize weapon reload
     */
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

    /*
     * @return weapon the weapon is on full ammo. Player cannot reload weapon when they are on max ammo
     */
    public bool CanReload()
    {
        return ammo < maxAmmo;
    }

    /*
     * Called when reload is complete
     */
    public void CompleteReload()
    {

        Reloading = false;
        Ammo = MaxAmmo;
    }

    /*
     * Called for reloads for weapons that are reloaded one round at a time. For example, a shotgun which is rounded through a tube
     */
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

    /*
     * Called by the projectile when they hit something. Calls hitmarkers to run animations
     * 
     * @param killShot - whether this shot has killed an opponent
     */
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

    /*
     * Get the sight which is currently active
     */
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

    /*
     * Set this item as local, meaning that it is being used by the player. For example, this would be false
     * if an AI player was using a firearm.
     */
    public void SetAsLocalPlayer()
    {
        SetActiveSight();
        if (activeSight) ActiveSight.EnableCamera();
    }

    /*
     * Update the shift component of the item which is created by player movement and inertia.
     */
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

    /*
     * Set the sight to be active
     * 
     * @param sight - the number sight to use
     */
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

    /*
     * @return - the sight attachment to return
     */
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

    /*
     * How much to draw the weapon back. If the player is too close to a wall, the player will pull the firearm towards the,
     * 
     * @param distance - the distance to the obstacle in front
     */
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
