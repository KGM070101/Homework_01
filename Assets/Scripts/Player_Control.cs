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

    private bool isMoving;
    private bool isReloading = false;
    private bool isDashing = false;
    private bool canDash;
    public bool isDead=false;
    

    private Vector2 moveDir;
    private Vector2 mousePosition;
    private Vector2 PlayerOriginalFacingDir;

    private float moveSpeed;
    private float fireCoolTimer = 0.5f;
    private float reloadTimer = 2;
    private float ammo = 99;    
    public float SkillStack = 0;
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

    public void TakeDamage(float damage)
    {
        maxHP -= damage;
    }

    public void Dead()
    {
        if(maxHP<=0)
        {
            maxHP = 0;
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

        if(fireCoolTimer>=0.5f)
        {
            if(isReloading==false)
            {
                switch (fireMode)
                {
                    case FireMode.Burst:
                        coroutine = StartCoroutine(Co_BurstFire());
                        break;

                    case FireMode.Shotgun:
                        Shotgun();
                        break;
                }
                
            }            

            fireCoolTimer = 0;
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
    private void Shotgun()
    {
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

        ammo -= 9;
    }
}
