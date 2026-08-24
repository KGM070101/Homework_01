using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum FireMode
{
    Burst,
    Shotgun
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
    private bool isDashing = false;
    private bool canDash;
    private bool canFire;
    public bool isDead=false;
    private bool canUlt;
    private bool isInUltState = false;
    public bool isInBurstFireUltState = false;
    public bool isInShotgunUltState = false;
    

    private Vector2 moveDir;
    private Vector2 mousePosition;
    private Vector2 PlayerOriginalFacingDir;    

    private float moveSpeed;
    private float fireCoolTimer = 0.5f;
    private float reloadTimer;
    private float ammo = 99;    
    public float UltStack = 0;
    private float ultMaxDuration=15.0f;
    private float ultTimer;
    private float maxHP;
    private float hp;
    public float power;   
    private int trigger = 1;

    private Coroutine coroutine;
    private void Awake_BindInput()
    {
        
    }

    protected override void FixedUpdate()
    {
        if(isDead==false)
        {
            rigidbody2D.linearVelocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
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
            Destroy(gameObject);
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
        if (!context.performed)
        {
            return;
        }

        if(canFire==true)
        {
            if(isReloading==false)
            {
                if(fireMode==FireMode.Burst)
                {
                    if(isInUltState==false)
                    {
                        isInBurstFireUltState = false;
                        isInShotgunUltState = false;
                        coroutine = StartCoroutine(Co_BurstFire());
                    }
                    else
                    {
                        isInBurstFireUltState = true;
                        isInShotgunUltState = false;
                        coroutine = StartCoroutine(CoBurstFire_Ult());
                    }
                }
                else if(fireMode==FireMode.Shotgun)
                {
                    if(isInUltState==false)
                    {
                        isInBurstFireUltState = false;
                        isInShotgunUltState = false;
                        Shotgun();
                    }
                    else
                    {
                        isInBurstFireUltState = false;
                        isInShotgunUltState = true;
                        Shotgun_Ult();                        
                    }
                }
                
            }            

            fireCoolTimer = 0;
        }                
    }

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
            trigger = 0;
        }
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
}
