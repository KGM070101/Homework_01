using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SkillCardManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;
    [SerializeField] private TextMeshProUGUI[] levelTexts;
    [SerializeField] private List<SkillCardData> statCards;
    [SerializeField] private float cardWaitTime = 0.5f;

    private Dictionary<SkillCardData, int> cardLevel = new();
    private Dictionary<SkillCardData, SkillRunTIme> ownedSkills = new();

    private Player player;
    private int pending;
    private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        player.OnLevelUp += HandleLevelUp;
    }

    private void OnDisable()
    {
        player.OnLevelUp -= HandleLevelUp;
    }

    private void HandleLevelUp(int level)
    {
        pending++;
        TryOpen();
    }

    private void TryOpen()
    {
        if (isOpen || pending <= 0)
            return;

        List<SkillCardData> pool = new List<SkillCardData>();
        foreach (SkillCardData card in statCards)
        {
            if (!IsMaxLevel(card))
                pool.Add(card);
        }

        if (pool.Count == 0)
            return;

        pending--;
        isOpen = true;
        GameManager.CardOpenChanged(isOpen);
        Time.timeScale = 0.0f;
        panel.SetActive(true);

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].gameObject.SetActive(true);

        int count = Mathf.Min(buttons.Length, pool.Count);

        for (int i = 0; i < count; i++)
        {
            int index = UnityEngine.Random.Range(0, pool.Count);
            SkillCardData skillCard = pool[index];            
            pool.RemoveAt(index);

            nameTexts[i].text = skillCard.cardName;
            descriptionTexts[i].text = skillCard.description;

            int current = GetLevel(skillCard);
           
            if (current == 0)
                levelTexts[i].text = "NEW";
            else if(current+1>=skillCard.maxLevel)
                levelTexts[i].text= $"lv.{current}¡ælv.Max";
            else
                levelTexts[i].text= $"lv.{current}¡ælv.{current + 1}";

            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => Select(skillCard));
        }

        for (int i = count; i < buttons.Length; i++)
            buttons[i].gameObject.SetActive(false);       
    }

    private void Select(SkillCardData skillCard)
    {
        if(skillCard.type==CardType.Stat)
        {
            if (skillCard.statKind == StatKind.HomingCount)
                Arrow.AddHomingBonus((int)skillCard.statValue);
            else
                player.AddStatBonus(skillCard.statKind, skillCard.statValue);

            cardLevel[skillCard] = GetLevel(skillCard) + 1;            
        }
        else
        {
            if(ownedSkills.TryGetValue(skillCard,out SkillRunTIme owned))
            {
                owned.LevelUp();
            }
            else
            {
                SkillRunTIme skill = Instantiate(skillCard.skillPrefab);
                skill.Configure(player);
                ownedSkills[skillCard] = skill;
            }
                
        }

        isOpen = false;
        GameManager.CardOpenChanged(isOpen);
        panel.SetActive(false);
        StartCoroutine(Co_WaitOpenTime());
    }

    private IEnumerator Co_WaitOpenTime()
    {
        if (pending > 0)
            yield return new WaitForSecondsRealtime(cardWaitTime);

        TryOpen();

        if (!isOpen)
            Time.timeScale = 1.0f;
    }

    private int GetLevel(SkillCardData skillCard)
    {
        cardLevel.TryGetValue(skillCard, out int level);

        return level;
    }

    private bool IsMaxLevel(SkillCardData skillCard)
    {
        return GetLevel(skillCard) >= skillCard.maxLevel;
    }
}
