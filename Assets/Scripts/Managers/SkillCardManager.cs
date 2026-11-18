using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCardManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private TextMeshProUGUI[] descriptionTexts;
    [SerializeField] private List<SkillCardData> statCards;

    private Player player;
    private int pending;
    private bool isOpen;

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

        pending--;
        isOpen = true;
        Time.timeScale = 0.0f;
        panel.SetActive(true);

        List<SkillCardData> pool = new List<SkillCardData>(statCards);
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = Random.Range(0, pool.Count);
            SkillCardData skillCard = pool[index];
            pool.RemoveAt(index);

            nameTexts[i].text = skillCard.cardName;
            descriptionTexts[i].text = skillCard.description;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => Select(skillCard));
        }
    }

    private void Select(SkillCardData skillCard)
    {
        if (skillCard.statKind == StatKind.HomingCount)        
            Arrow.AddHomingBonus((int)skillCard.statValue);        
        else        
            player.AddStatBonus(skillCard.statKind, skillCard.statValue);

        isOpen = false;
        panel.SetActive(false);
        Time.timeScale = 1.0f;

        TryOpen();
    }
}
