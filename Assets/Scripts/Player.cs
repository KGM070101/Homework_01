using TMPro;
using UnityEditor.U2D.Animation;
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
    private GameObject Weapon;
    [SerializeField]
    private Transform bulletBox;
    [SerializeField]
    private GameObject LeftWheel;
    [SerializeField]
    private GameObject RightWheel;
    

    private Vector2 OriginalWeaponSize; //무기 발사할때 애니메이션 로직에 사용
    private Vector2 OriginalWheelSize; //플레이어 이동할때 바퀴 애니메이션 로직에 사용


    private InputSystem_Actions action;
    private Camera mainCamera;
    protected override void Awake()
    {
        //dataFile = "Assets / CharactersDatas / Player";
        base.Awake();        

        action = new InputSystem_Actions();
        action.Player.Enable();
        action.Player.SetCallbacks(this);

        mainCamera = Camera.main;
        OriginalWeaponSize = Weapon.transform.localScale;
        OriginalWheelSize = 
            LeftWheel.transform.localScale = RightWheel.transform.localScale;
    }

    protected override void Update()
    {
        base.Update();

        LookPointer(); //Player_Control --> 플레이어 시선 마우스 제어 로직
        if(isMoving==true&&coroutine==null)
        {
            coroutine = StartCoroutine(Co_WheelAnimationStart());
        }

        //총알발사 간격 타이머
        {
            fireCoolTimer += Time.deltaTime;

            if (fireCoolTimer >= 0.5f)
                fireCoolTimer = 0.5f;
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
                    reloadTimer = 2f;
                }
            }
        }

        //화면 UI 표시
        {
            IndicateAmmo();
            IndicateAmmoBar();
            IndicateFireMode();
            if (isReloading)
            {
                IndicateReloadingAmmoBar();
            }
        }
        
            

        //Debug.Log(ammo);
    }

    protected override void Reset()
    {
        base.Reset();

        //dataFile = "Assets / CharactersDatas / Player"; 
    }

    
}
