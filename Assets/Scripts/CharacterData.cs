using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("HP")]
    [SerializeField]
    private float maxHp=100f;
    public float MaxHp => maxHp;


    [Header("MOVE_SPEED")]
    [SerializeField]
    private float moveSpeed = 5f;
    public float MoveSpeed => moveSpeed;

    [Header("DASH_SPEED")]
    [SerializeField]
    private float dashSpeed = 20f;
    public float DashSpeed => dashSpeed;


    [Header("POWER")]
    [SerializeField]
    private float power = 10f;
    public float Power => power;

    [Header("ATTACK_SPEED")]
    [SerializeField]
    private float attackSpeed = 5f;
    public float AttackSpeed => attackSpeed;

    [Header("STOP_DISTANCE")]
    [SerializeField]
    private float stopDistance = 2.0f;
    public float StopDistance => stopDistance;
}
