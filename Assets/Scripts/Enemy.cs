using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class Enemy : Character
{
    [Header("- Enemy")]
    [SerializeField]
    private float hp;

    [SerializeField]
    private Transform player;
      
    private Vector3 originalPos;

    private Coroutine coroutine;
    protected override void Awake()
    {
        base.Awake();

        hp = data[1].MaxHp;
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
            Destroy(collision.gameObject);
        }
    }

    protected override void Update()
    {
        base.Update();

        Debug.Log(hp);
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

        TracePlayer();
    }

    private IEnumerator Co_Dead()
    {         
        if(hp<=0)
        {
            hp = 0;
           
            spriteRenderer.DOFade(0f, 0.5f);

            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }             
    }    
}
