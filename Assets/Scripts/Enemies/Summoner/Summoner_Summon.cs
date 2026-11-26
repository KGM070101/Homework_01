using UnityEngine;

public class Summoner_Summon : Summoner_States
{
    [SerializeField] private float minRadius;
    [SerializeField] private float maxRadius;
    [SerializeField] private float healRadius;
    [SerializeField] private Transform enemyBox;
    

    public Summoner_Summon(Summoner summoner, Summoner_FSM fsm) : base(summoner, fsm)
    {

    }
}
