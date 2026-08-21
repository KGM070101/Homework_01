using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class Enemy : Character
{
    [Header("- Enemy")]
    [SerializeField]
    private float hp;

    [SerializeField]
    private GameObject leftArm;

    [SerializeField]
    private GameObject RightArm;

    [SerializeField]
    private Transform playerPos;

    private Player player;
      
    private Vector3 originalPos;
    private Vector2 originalLeftArmPos;
    private Vector2 originalRightArmPos;

    private DOTween doTween;

    private Coroutine coroutine;
    protected override void Awake()
    {
        base.Awake();

        hp = data[1].MaxHp;
        player = FindFirstObjectByType<Player>();

        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        Physics2D.IgnoreLayerCollision
            (LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("RedLine"));

        originalLeftArmPos = leftArm.transform.localPosition;
        originalRightArmPos = RightArm.transform.localPosition;
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (collision.gameObject.CompareTag("BulletForPlayer"))
        {
            hp -= data[0].Power;
            player.SkillStack += 1;
            Destroy(collision.gameObject);
        }
    }

    protected override void Update()
    {
        base.Update();

        //Debug.Log(hp);
        if (gameObject)
        {
            coroutine = StartCoroutine(Co_Dead());
        }
        else
        {
            return;
        }


        if (hp <= 0)
        {
            originalPos = transform.position;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if(player.isDead==false)
        {
            TracePlayer();
        }
        else
        {
            return;
        }
        
    }

    private IEnumerator Co_Dead()
    {         
        if(hp<=0)
        {
            hp = 0;            
                
            spriteRenderer.DOFade(0f, 0.5f);
            enemy_LeftArm.spriteRenderer.DOFade(0f, 0.5f);
            enemy_RightArm.spriteRenderer.DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.51f);
            Destroy(gameObject);           
        }             
    }    
}
