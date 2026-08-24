using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public partial class Player : Character, InputSystem_Actions.IPlayerActions
{
    [Header("- Player")]
    [SerializeField]
    private BulletForPlayer bulletPrefab;

    [SerializeField]
    private Transform firepoint;

    [SerializeField]
    private Transform firepoint_BurstUlt_1;

    [SerializeField]
    private Transform firepoint_BurstUlt_2;

    [SerializeField]
    private GameObject Weapon;

    [SerializeField]
    private Transform bulletBox;

    [SerializeField]
    private GameObject LeftWheel;

    [SerializeField]
    private GameObject RightWheel;

    private Enemy2 enemy2;
    private Enemy_Spawner enemy_Spawner;
    private CameraShaking cameraShaking;

    private Vector2 OriginalWeaponSize; //무기 발사할때 애니메이션 로직에 사용
    private Vector2 OriginalWheelSize; //플레이어 이동할때 바퀴 애니메이션 로직에 사용


    private InputSystem_Actions action;
    private Camera mainCamera;    
    protected override void Awake()
    {
        //dataFile = "Assets / CharactersDatas / Player";
        base.Awake();

        enemy_Spawner = FindFirstObjectByType<Enemy_Spawner>();
        cameraShaking = FindFirstObjectByType<CameraShaking>();

        action = new InputSystem_Actions();
        action.Player.Enable();
        action.Player.SetCallbacks(this);

        mainCamera = Camera.main;
        OriginalWeaponSize = Weapon.transform.localScale;
        OriginalWheelSize = 
            LeftWheel.transform.localScale = RightWheel.transform.localScale;

        reloadTimer = data[0].ReloadTIme;
        reloadTime = data[0].ReloadTIme;

        maxHP = data[0].MaxHp;
        hp = data[0].MaxHp;
        power = data[0].Power;
    }

    protected override void Start()
    {
        base.Start();       
    }

    protected override void Update()
    {
        base.Update();

        if(isDead==false)
        {
            //플레이어 시선 마우스 제어 로직
            {
                LookPointer();
            }
            
            //플레이어 바퀴 애니메이션 재생
            {
                if (isMoving == true && coroutine == null)
                {
                    coroutine = StartCoroutine(Co_WheelAnimationStart());
                }
            }            

            //플레이어 최대체력 상한선 유지
            {
                if (hp >= maxHP)
                {
                    hp = maxHP;
                }
            }

            //플레이어 최대레벨 상한선 유지
            {
                if(level>=10)
                {
                    level = 10;
                }
            }

            //총알발사 간격 타이머
            {
                fireCoolTimer += Time.deltaTime;

                if(isInBurstFireUltState==false)
                {
                    if (fireCoolTimer >= 0.5f)
                    {
                        fireCoolTimer = 0.5f;
                        canFire = true;
                    }
                    else
                    {
                        canFire = false;
                    }                    
                }
                else
                {
                    if (fireCoolTimer >= 1.0f) // ==>버스트모드 궁극기 상태에서 총알수가 두배가 돼서,
                                               // 타이밍 맞추려고 쿨타임도 2배
                    {
                        fireCoolTimer = 1.0f;
                        canFire = true;
                    }
                    else
                    {
                        canFire = false;
                    }
                }                
            }

            //재장전 타이머
            {
                if (ammo <= 0)
                {
                    ammo = 0;
                    reloadTimer -= Time.deltaTime;
                    isReloading = true;

                    if (reloadTimer <= 0)
                    {
                        isReloading = false;
                        ammo += 99;
                        reloadTimer = data[0].ReloadTIme;
                    }
                }
            }

            //화면 UI 표시
            {
                IndicateAmmo();
                IndicateAmmoBar();
                IndicateFireMode();
                IndicatePlayerHp();
                IndicatePlayerHpBar();
                IndicateXpBar();
                IndicateLevel();
                IndicateEnemyCount();
                if (isReloading)
                {
                    IndicateReloadingAmmoBar();
                }

                if(isInUltState==true)
                {
                    IndicateUltDuration();
                }
                else
                {
                    IndicateUltBar();
                }

                if(canUlt==true)
                {
                    if(isUltBarBlinking==false)
                    {
                        isUltBarBlinking = true;

                        coroutine = StartCoroutine(Co_UltBarBlink());
                    }
                    
                }
                else
                {
                    if(isUltBarBlinking==true)
                    {
                        isUltBarBlinking = false;

                        StopCoroutine(coroutine);

                        ultBar.DOKill();
                        ultBar.color = originalUltBarColor;
                    }
                }
            }

            //대쉬 타이머
            {
                dashCoolTime -= Time.deltaTime;
                if (dashCoolTime <= 0)
                {
                    canDash = true;
                }
            }

            //대쉬 지속시간
            {
                dashDuration -= Time.deltaTime;
                if (dashDuration <= 0)
                {
                    isDashing = false;
                }
            }

            //플레이어 속도 제어
            {
                if (isDashing == false)
                {
                    if(level==1)
                    {
                        moveSpeed = data[0].MoveSpeed;
                    }
                    else if(level==2)
                    {
                        moveSpeed = data[0].MoveSpeed*1.05f;
                    }
                    else if(level==3)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.1f;
                    }
                    else if(level==4)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.15f;
                    }
                    else if(level==5)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.2f;
                    }
                    else if (level == 6)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.25f;
                    }
                    else if (level == 7)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.3f;
                    }
                    else if (level == 8)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.35f;
                    }
                    else if (level == 9)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.4f;
                    }
                    else if (level == 10)
                    {
                        moveSpeed = data[0].MoveSpeed * 1.45f;
                    }
                }                
                else
                {
                    moveSpeed = data[0].DashSpeed;
                }
            }

            //플레이어 궁극기 사용 가능 여부 판단
            {
                if(UltStack/maxUltStack>=1)
                {
                    canUlt = true;                    
                }
                else
                {
                    canUlt = false;
                }
            }

            //플레이어 궁극기 타이머
            {
                if(isInUltState==true)
                {
                    ultTimer += Time.deltaTime;
                    if(ultTimer>=ultMaxDuration)
                    {
                        ultTimer = ultMaxDuration;
                        isInUltState = false;       
                        if(isInUltState==false)
                        {
                            ultTimer = 0;
                            isInBurstFireUltState = false;
                            isInShotgunUltState = false;
                        }
                    }
                }
            }

            //플레이어 레벨업
            {
                if(xp>=requireXp)
                {
                    if(level<=9)
                    {
                        LevelUp();
                    }                  
                }
            }
            
        }        

        //플레이어 사망
        {
            Dead();
        }

        //Debug.Log(ammo);
        //Debug.Log(requireXp);
        //Debug.Log(power);
    }

    //protected override void Reset()
    //{
    //    base.Reset();
    //}
}
