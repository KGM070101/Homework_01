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
        //player.OnLevelUp+=
    }

    private void OnDisable()
    {
        //player.OnLevelUp-=
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
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => Select(skillCard));
        }
    }

    private void Select(SkillCardData skillCard)
    {
        player.AddStatBonus(skillCard.statKind, skillCard.statValue);

        isOpen = false;
        panel.SetActive(false);
        Time.timeScale = 1.0f;

        TryOpen();
    }
}
