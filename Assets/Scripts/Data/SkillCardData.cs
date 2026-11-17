using UnityEngine;

public enum CardType
{
    Skill,
    Stat
}

public enum StatKind
{
    Power,
    MoveSpeed,
    MaxHp,   
}

[CreateAssetMenu(menuName ="Card/CardData")]
public class SkillCardData : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite icon;
    public CardType type;

    [Header("스탯 카드")]
    public StatKind statKind;
    public float statValue;

    [Header("스킬 카드")]
    public GameObject skillPrefab;
}
