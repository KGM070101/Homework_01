using Mono.Cecil;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    [Header("- 캐릭터 데이터들")]
    [SerializeField]   
    protected CharacterData[] data;   

    protected new Rigidbody2D rigidbody2D;
    protected BoxCollider2D boxCollider2D;
    protected SpriteRenderer spriteRenderer;

   

    protected virtual void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider2D = GetComponentInChildren<BoxCollider2D>();
        
        //data = Resources.Load<CharacterData>(dataFile);
    }

    protected virtual void Update()
    {

    }

#if UNITY_EDITOR
    protected virtual void Reset()
    {

    }
#endif

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
}


