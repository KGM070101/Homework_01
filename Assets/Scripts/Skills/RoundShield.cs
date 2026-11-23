using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoundShield : SkillRunTIme
{
    [SerializeField] private GameObject shield;
    [SerializeField] private float turnSpeed = 360.0f; //deg
    [SerializeField] private float turnRadius = 3.0f;

    private int shieldCount = 0;
    private int shieldMaxCount = 5;
    private List<GameObject> shields = new();

    [SerializeField] private float startAngle; //rad

    public override void Configure(Player player)
    {
        base.Configure(player);

        AddShieldCount();        
       
        for (int i = 0; i < shieldCount; i++)
        {
            shields[i] = Instantiate(shield, transform);
        }    
    }

    public override void LevelUp()
    {
        base.LevelUp();

        AddShieldCount();
    }

    public void AddShieldCount()
    {
        if (shields.Count >= shieldMaxCount)
            return;

        shields.Add(Instantiate(shield, transform));
    }

    private void Update()
    {
        startAngle += turnSpeed * Time.deltaTime * Mathf.Deg2Rad;
        startAngle = Mathf.Repeat(startAngle, Mathf.PI * 2);

        float angleStep = (360.0f / shields.Count) * Mathf.Deg2Rad;        

        for (int i=0; i<shields.Count; i++)
        {
            shields[i].transform.position = 
                player.transform.position +
                turnRadius * new Vector3(Mathf.Cos(startAngle+angleStep*i), 
                Mathf.Sin(startAngle + angleStep * i), 0);

            shields[i].transform.rotation = 
                Quaternion.Euler(0, 0, (startAngle + angleStep * i) * Mathf.Rad2Deg - 90);
        }
    }
}
