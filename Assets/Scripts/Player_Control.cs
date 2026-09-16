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
    public bool isDead=false;
    private bool canUlt;
    public bool isInUltState = false;
    public bool isInBurstFireUltState = false;
    public bool isInShotgunUltState = false;
    public bool isInArrowUltState = false;
    private bool isCharging;
    

    private Vector2 moveDir;
    private Vector2 mousePosition;
    private Vector2 PlayerOriginalFacingDir;    

    private float moveSpeed;
    private float fireCoolTimer = 0.5f;
    private float reloadTimer;
    private float arrowReloadTImer;
    private float ammo = 99;
    private int arrows = 10;
    public float UltStack = 0;
    private float ultMaxDuration=15.0f;
    private float ultTimer;
    private float maxHP;
    private float hp;
    public float power;
    private float arrow_MaxChargeTime = 3f;
    private float arrow_ChargeStartTime;
    public float arrow_ChargeRation;
    private float arrow_ChargeTime;
    private int trigger = 1;
    private int arrowReloadTrigger = 0;

    private Coroutine coroutine;
    private void Awake_BindInput()
    {
        
    }

    protected override void FixedUpdate()
    {
        if(isDead==false)
        {
            rigidbody2D.linearVelocity = 
                new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
        }
        
        //transform.Rotate(facingDir);                       
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if(collision.gameObject.CompareTag("BulletForEnemy2"))
        {
            enemy2 = FindFirstObjectByType<Enemy2>();
            TakeDamage(enemy2.power);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }

    public void Heal(float healAmount)
    {
        hp += healAmount;
    }
    
    public void Dead()
    {
        if(hp<=0||enemy_Spawner.enemyCount>=50)
        {
            hp = 0;
            isDead=true;
            gameObject.SetActive(false);
            Time.timeScale = 0;
        }
    }

    private void LookPointer()
    {
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
                        coroutine = StartCoroutine(CoBurstFire_Ult());
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
        
        //arrow_ChargeStartTime = Time.time;       
    }

    private void Arrow_ReleaseCharge()
    {
        if(!isCharging)
        {
            return;
        }

        isCharging = false;
       
        //float chargeTime = Time.time - arrow_ChargeStartTime;
        //arrow_ChargeRation = Mathf.Clamp01(chargeTime / arrow_MaxChargeTime);

        Arrow_Shoot();        
        arrow_ChargeTime = 0;
        arrowChargingBar.fillAmount = 0;
        //arrow_ChargeRation = 0;
    }

    private IEnumerator Co_BurstFire()
    {
        BulletForPlayer bullet =
            Instantiate(bulletPrefab, firepoint.position, Quaternion.identity, bulletBox);

        bullet.Shoot(PlayerOriginalFacingDir);
        ammo -= 1;
        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bullet1 =
            Instantiate(bulletPrefab, firepoint.position, Quaternion.identity, bulletBox);

        bullet1.Shoot(PlayerOriginalFacingDir);
        ammo -= 1;
        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bullet2 =
            Instantiate(bulletPrefab, firepoint.position, Quaternion.identity, bulletBox);

        bullet2.Shoot(PlayerOriginalFacingDir);
        ammo -= 1;
        WeaponBounceVer1();
    }

    private IEnumerator CoBurstFire_Ult()
    {
        BulletForPlayer bulletLeft =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft.Shoot(PlayerOriginalFacingDir);
        bulletRight.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bulletLeft1 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight1 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft1.Shoot(PlayerOriginalFacingDir);
        bulletRight1.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bulletLeft2 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight2 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft2.Shoot(PlayerOriginalFacingDir);
        bulletRight2.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bulletLeft3 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight3 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft3.Shoot(PlayerOriginalFacingDir);
        bulletRight3.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bulletLeft4 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight4 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft4.Shoot(PlayerOriginalFacingDir);
        bulletRight4.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();

        yield return new WaitForSeconds(0.1f);

        BulletForPlayer bulletLeft5 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_1.position, Quaternion.identity, bulletBox);
        BulletForPlayer bulletRight5 =
            Instantiate(bulletPrefab, firepoint_BurstUlt_2.position, Quaternion.identity, bulletBox);

        bulletLeft5.Shoot(PlayerOriginalFacingDir);
        bulletRight5.Shoot(PlayerOriginalFacingDir);

        WeaponBounceVer1();        
    }

    private void Shotgun()
    {
        //스크립트 제어
        {
            shotgunSpreadAngle = 60.0f;
            shotgunBulletCount = 9;
        }        

        Vector2 baseDirection = PlayerOriginalFacingDir.normalized;

        float startAngle = -shotgunSpreadAngle * 0.5f;
        float angleStep =
            shotgunSpreadAngle / (shotgunBulletCount - 1);

        for (int i = 0; i < shotgunBulletCount; i++)
        {
            float currentAngle =
                startAngle + (angleStep * i);

            Vector2 bulletDirection =
                Quaternion.AngleAxis(
                    currentAngle,
                    Vector3.forward
                ) * baseDirection;

            BulletForPlayer bullet = Instantiate(
                bulletPrefab,
                firepoint.position,
                Quaternion.identity,
                bulletBox
            );
            WeaponBounceVer2();

            bullet.durationTime = 0.5f;
            bullet.Shoot(bulletDirection);
        }

        //cameraShaking.ShakeCamera(0.1f, 0.1f);
        ammo -= 9;
    }

    private void Shotgun_Ult()
    {
        //스크립트 제어
        {
            shotgunSpreadAngle = 60.0f;
            shotgunBulletCount = 18;
        }

        Vector2 baseDirection = PlayerOriginalFacingDir.normalized;

        float startAngle = -shotgunSpreadAngle * 0.5f;
        float angleStep =
            shotgunSpreadAngle / (shotgunBulletCount - 1);

        for (int i = 0; i < shotgunBulletCount; i++)
        {
            float currentAngle =
                startAngle + (angleStep * i);

            Vector2 bulletDirection =
                Quaternion.AngleAxis(
                    currentAngle,
                    Vector3.forward
                ) * baseDirection;

            BulletForPlayer bullet = Instantiate(
                bulletPrefab,
                firepoint.position,
                Quaternion.identity,
                bulletBox
            );
            WeaponBounceVer2();

            bullet.durationTime = 0.5f;
            bullet.Shoot(bulletDirection);
        }

        ammo -= 18;
    }

    private void Arrow_Shoot()
    {
        Arrow arrow =
            Instantiate(arrowPrefab, firepoint.position, Quaternion.identity, bulletBox);

        //if(isInUltState)
        //{
        //    arrow.transform.localScale = arrow.transform.localScale * 3 * arrow_ChargeRation;
        //    if (arrow.transform.localScale.x <= 1.0f)
        //    {
        //        arrow.transform.localScale = new Vector2(1.0f, 1.0f);
        //    }
        //}        

        arrow.Shoot(PlayerOriginalFacingDir, 60 * arrow_ChargeRation);

        arrows--;
    }

    private void Arrow_Shoot_Ult()
    {
        Arrow arrow =
            Instantiate(arrowPrefab, firepoint.position, Quaternion.identity, bulletBox);

        arrow.Shoot(PlayerOriginalFacingDir, 60 * arrow_ChargeRation);
    }
}
