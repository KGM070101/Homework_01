using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FireMode
{
    Burst,
    Shotgun,
    Arrow
}
public partial class Player
{
    [Header("- Control")]
    [SerializeField]
    private FireMode fireMode;

    [SerializeField]
    private int shotgunBulletCount = 9;

    [SerializeField]
    private float shotgunSpreadAngle = 40.0f;

    [SerializeField]
    private float dashDuration=0.2f;

    [SerializeField]
    private float dashCoolTime=2.0f;

    [SerializeField]
    private float maxUltStack = 60.0f;

    private bool isMoving;
    private bool isReloading = false;
    private bool isReloading_Arrow = false;
    private bool isDashing = false;
    private bool canDash;
    private bool canFire;    
    private bool canUlt;    
    private bool isCharging;
    private bool inputLocked = false;

    public bool isDead = false;
    public bool isInUltState = false;
    public bool isInBurstFireUltState = false;
    public bool isInShotgunUltState = false;
    public bool isInArrowUltState = false;

    private Vector2 moveDir;
    private Vector2 mousePosition;
    private Vector2 PlayerOriginalFacingDir;    

    private float moveSpeed;
    private float fireCoolTimer = 0.5f;
    private float fireCoolTime = 0.5f;
    private float fireCoolTImeReduction;
    private float reloadTimer;
    private float arrowReloadTImer;
    private float ammo = 99;   
    private float ultMaxDuration = 20.0f;
    private float ultTimer;
    private float arrow_MaxChargeTime = 3f;
    private float arrow_ChargeStartTime;   
    private float arrow_ChargeTime;
    private float defensiveBonus = 0;
    private float moveSpeedMultiply = 1.0f;

    public float UltStack = 0;
    public float arrow_ChargeRation;
    public float maxHP;
    public float hp;
    public float power;

    private int trigger = 1;
    private int arrowReloadTrigger = 0;
    private int arrows = 10;
    private int bonusBulletCount;
    private int maxBonusBulletCount = 5;

    private Coroutine coroutine;   
   
    protected override void FixedUpdate()
    {
        if (!isDead)
        {
            rigidbody2D.linearVelocity =
                new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        }
        else
            rigidbody2D.linearVelocity = Vector2.zero;
        
        //transform.Rotate(facingDir);                       
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);        
    }

    public void TakeDamage(float damage)
    {
        float reduction = Mathf.Min(defensiveBonus, 0.5f);

        hp -= (damage*(1.0f-reduction));
        IndicatePlayerHp();

        if (hp <= 0)
            Dead();
    }

    public void Heal(float healAmount)
    {
        if (hp >= maxHP)
            return;

        hp = Mathf.Min(hp + healAmount, maxHP);
        IndicatePlayerHp();

        if (hp >= maxHP)
            hp = maxHP;
    }
    
    public void Dead()
    {                
        isDead=true;
        boxCollider2D.enabled = false;

        Destroy(body);
    }

    private void LookPointer()
    {
        if (isDead)
            return;

        if (inputLocked)
            return;

        Vector3 mouseWorldPosition = 
            mainCamera.ScreenToWorldPoint
            (new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));

        Vector2 facingDir = mouseWorldPosition - transform.position;

        if(facingDir.sqrMagnitude<0.1f)
        {
            return;
        }

        PlayerOriginalFacingDir = facingDir.normalized;

        float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (inputLocked)
            return;

        if(canFire==true)
        {
            if(isReloading_Arrow==false)
            {
                if (fireMode == FireMode.Arrow)
                {
                    if (isInUltState == false)
                    {
                        isInBurstFireUltState = false;
                        isInShotgunUltState = false;
                        isInArrowUltState = false;
                    }
                    else
                    {
                        isInBurstFireUltState = false;
                        isInShotgunUltState = false;
                        isInArrowUltState = true;
                    }

                    Arrow_Input(context);

                    return;
                }
            }
            
            if (isReloading==false)
            {                
                if (!context.performed)
                {
                    return;
                }

                if (fireMode==FireMode.Burst)
                {
                    if(isInUltState==false)
                    {
                        isInBurstFireUltState = false;
                        isInArrowUltState = false;
                        isInShotgunUltState = false;
                        coroutine = StartCoroutine(Co_BurstFire());
                    }
                    else
                    {
                        isInBurstFireUltState = true;
                        isInArrowUltState = false;
                        isInShotgunUltState = false;
                        coroutine = StartCoroutine(Co_BurstFire_Ult());
                    }
                }
                else if(fireMode==FireMode.Shotgun)
                {
                    if(isInUltState==false)
                    {
                        isInBurstFireUltState = false;
                        isInArrowUltState = false;
                        isInShotgunUltState = false;
                        Shotgun();
                    }
                    else
                    {
                        isInBurstFireUltState = false;
                        isInArrowUltState = false;
                        isInShotgunUltState = true;
                        Shotgun_Ult();                        
                    }
                }
                
            }            
            fireCoolTimer = 0;
        }//if(isReloading == true)                
    }//if(canFire == true)

    public void OnUlt(InputAction.CallbackContext context)
    {
        if(canUlt)
        {
            isInUltState = true;            
            UltStack = 0;
        }
        else
        {
            if(!isInUltState)
            {
                IndicateUltAnnouncement();
            }            
        }            
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (inputLocked)
            return;

        if(context.performed)
        {
            moveDir = context.ReadValue<Vector2>();
            isMoving = true;
        }
        else if(context.canceled)
        {
            moveDir = Vector2.zero;
            isMoving = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (isDead)
            return;

        if (inputLocked)
            return;

        if(canDash==true)
        {
            isDashing = true;
            canDash = false;
            dashDuration = 0.2f;
            dashCoolTime = 2.0f;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        ammo = 0;        
        IndicateReloadingAmmoBar();
    }

    public void OnFireModeChange(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (isReloading_Arrow||isReloading)
            return;

        if (isCharging)
            return;

        trigger++;

        if(trigger==1)
        {
            fireMode = FireMode.Burst;
            coroutine = StartCoroutine(Co_WaitIndicateAnnouncement());
        }
        if(trigger==2)
        {
            fireMode = FireMode.Shotgun;
            coroutine = StartCoroutine(Co_WaitIndicateAnnouncement());            
        }
        if (trigger == 3)
        {
            fireMode = FireMode.Arrow;
            coroutine = StartCoroutine(Co_WaitIndicateAnnouncement());            
            trigger = 0;
        }
    }

    private void Arrow_Input(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Arrow_StartCharging();
        }
        else if(context.canceled)
        {
            Arrow_ReleaseCharge();           
        }
    }

    private void Arrow_StartCharging()
    {
        if(isCharging)
        {
            return;
        }

        isCharging = true;
    }

    private void Arrow_ReleaseCharge()
    {
        if(!isCharging)
        {
            return;
        }

        isCharging = false;
               
        Arrow_Shoot();        
        arrow_ChargeTime = 0;
        arrowChargingBar.fillAmount = 0;        
    }

    private void SpawnBullet()
    {
        FireBullet();
        WeaponBounceVer1();
        ammo -= 1;
        IndicateAmmo();
    }

    private void SpawnBullet_Ult()
    {
        PoolObject pool_0 = poolManager.Get(3);
        PoolObject pool_1 = poolManager.Get(3);

        pool_0.transform.SetParent(bulletBox);
        pool_0.transform.position = firepoint_BurstUlt_1.position;
        pool_0.transform.rotation = Quaternion.identity;

        pool_1.transform.SetParent(bulletBox);
        pool_1.transform.position = firepoint_BurstUlt_2.position;
        pool_1.transform.rotation = Quaternion.identity;

        BulletForPlayer bullet_0 = pool_0.GetComponent<BulletForPlayer>();
        BulletForPlayer bullet_1 = pool_1.GetComponent<BulletForPlayer>();

        bullet_0.Shoot(PlayerOriginalFacingDir, power);
        bullet_1.Shoot(PlayerOriginalFacingDir, power);
        WeaponBounceVer1();
    }

    private void FireBullet(Vector2? direction = null, float lifeTime = -1.0f)
    {
        Vector2 dir = direction ?? PlayerOriginalFacingDir;

        PoolObject pool = poolManager.Get(3);

        pool.transform.SetParent(bulletBox);
        pool.transform.position = firepoint.position;
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<BulletForPlayer>().Shoot(dir, power, lifeTime);
    }

    private IEnumerator Co_BurstFire()
    {        
        for (int i = 0; i < 3 + bonusBulletCount; i++)
        {
            if (ammo <= 0)
                yield break;

            SpawnBullet();

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator Co_BurstFire_Ult()
    {
        for (int i = 0; i < 6 + bonusBulletCount; i++)
        {
            SpawnBullet_Ult();

            yield return new WaitForSeconds(0.05f);
        }
    }                    

    private void Shotgun()
    {
        //총알 부채꼴각 / 탄수 스크립트 제어
        {
            shotgunSpreadAngle = 60.0f;
            shotgunBulletCount = 9 + bonusBulletCount;
        }        

        Vector2 baseDirection = PlayerOriginalFacingDir.normalized;

        float startAngle = -shotgunSpreadAngle * 0.5f;        

        int ammoCount = Mathf.Min(shotgunBulletCount, (int)ammo);

        float angleStep = 
            shotgunSpreadAngle / Mathf.Max(ammoCount - 1.0f, 1.0f);

        if (ammoCount <= 0)
            return;

        if (ammoCount == 1)
            startAngle = 0;
        
        for (int i = 0; i < ammoCount; i++)
        {
            float currentAngle =
                startAngle + (angleStep * i);

            Vector2 bulletDirection =
                Quaternion.AngleAxis(
                    currentAngle,
                    Vector3.forward
                ) * baseDirection;

            FireBullet(bulletDirection, 0.25f);               
        }

        ammo -= ammoCount;
        WeaponBounceVer2();
        IndicateAmmo();
    }

    private void Shotgun_Ult()
    {
        //총알 부채꼴 / 탄수 스크립트 제어
        {
            shotgunSpreadAngle = 60.0f;
            shotgunBulletCount = 18 + bonusBulletCount;
        }

        Vector2 baseDirection = PlayerOriginalFacingDir.normalized;

        float startAngle = -shotgunSpreadAngle * 0.5f;

        int ammoCount = Mathf.Min(shotgunBulletCount, (int)ammo);

        float angleStep =
            shotgunSpreadAngle / Mathf.Max(ammoCount - 1.0f, 1.0f);

        if (ammoCount <= 0)
            return;

        if (ammoCount == 1)
            startAngle = 0;

        for (int i = 0; i < ammoCount; i++)
        {
            float currentAngle =
                startAngle + (angleStep * i);

            Vector2 bulletDirection =
                Quaternion.AngleAxis(
                    currentAngle,
                    Vector3.forward
                ) * baseDirection;

            FireBullet(bulletDirection, 0.25f);
        }

        ammo -= ammoCount;
        WeaponBounceVer2();
        IndicateAmmo();
    }

    private void Arrow_Shoot()
    {
        PoolObject pool = poolManager.Get(5);

        pool.transform.SetParent(bulletBox);
        pool.transform.position = firepoint.position;
        pool.transform.rotation = Quaternion.identity;

        pool.GetComponent<Arrow>()?.
            Shoot(PlayerOriginalFacingDir, 60 * arrow_ChargeRation, arrow_ChargeRation);

        arrows--;        
    }   
}
