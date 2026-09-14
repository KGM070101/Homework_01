using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Player
{
    [Header("- UI")]
    [SerializeField]
    private GameObject ammoUI;

    [SerializeField]
    private TextMeshProUGUI ammoText;

    [SerializeField]
    private Image ammoBar;

    [SerializeField]
    private GameObject arrowUI;

    [SerializeField]
    private GameObject[] arrowBars;

    [SerializeField]
    private TextMeshProUGUI fireModeText;

    [SerializeField]
    private TextMeshProUGUI fireModeAnnouncementText;

    [SerializeField]
    private Image playerHpBar;

    [SerializeField]
    private TextMeshProUGUI playerHpText;

    [SerializeField]
    private Image ultBar;

    [SerializeField]
    private TextMeshProUGUI ultAnnouncementText;

    [SerializeField]
    private Image xpBar;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI enemyCountText;

    private float maxAmmo = 99;
    private float currentAmmo;
    private float reloadTime;    

    private bool isUltBarBlinking = false;

    private Color originalUltBarColor = new Color(1, 0.5f, 0);
    private Color blinkingUltBarColor = new Color(1, 0.8f, 0);
    private Color invisibleArrowBarColor = new Color(0, 0, 0, 0);
    private Color originalArrowBarColor = new Color(1, 0.5f, 0f, 1f);
    private void IndicateAmmo()
    {
        ammoText.text = "Ammo :" + ammo;
    }

    private void IndicateAmmoBar()
    {
        ammoBar.fillAmount = ammo / maxAmmo;
    }

    private void IndicateReloadingAmmoBar()
    {
        ammoBar.fillAmount = 0;
        ammoBar.fillAmount = 1.0f-(reloadTimer / reloadTime);
        ammoText.text = "Reloading...";
    }

    private void IndicateFireMode()
    {
        fireModeText.text = "FireMode :" + fireMode;
    }

    private void IndicateFireModeAnnoucement()
    {
        fireModeAnnouncementText.text = "FireMode :"+fireMode;

        fireModeAnnouncementText.DOFade(1.0f,0f);
        fireModeAnnouncementText.DOFade(0.0f, 2.0f);
    }

    private void IndicatePlayerHp()
    {
        playerHpText.text = "HP :" + hp;
    }

    private void IndicatePlayerHpBar()
    {
        playerHpBar.fillAmount = hp / maxHP;
    }

    private IEnumerator Co_WaitIndicateAnnouncement()
    {
        yield return new WaitForSeconds(0.2f);

        IndicateFireModeAnnoucement();        
    }    

    private void IndicateUltBar()
    {
        ultBar.fillAmount = UltStack / maxUltStack;
    }

    private void IndicateUltAnnouncement()
    {
        ultAnnouncementText.DOFade(1.0f, 0f);
        ultAnnouncementText.DOFade(0.0f, 1.0f);
    }

    private void IndicateUltDuration()
    {
        ultBar.fillAmount = 1 - (ultTimer / ultMaxDuration);
    }    

    private void UltBarAnimation()
    {
        ultBar.DOKill();

        seq = DOTween.Sequence().
            Append(ultBar.DOColor(blinkingUltBarColor, 0.5f)).
            Append(ultBar.DOColor(originalUltBarColor, 0.5f));
    }

    private IEnumerator Co_UltBarBlink()
    {
        while(canUlt==true)
        {
            UltBarAnimation();

            yield return new WaitForSeconds(1);
        }

        isUltBarBlinking = false;
        coroutine = null;
    }

    private void IndicateXpBar()
    {
        xpBar.fillAmount = xp / requireXp;
    }

    private void IndicateLevel()
    {
        levelText.text = "" + level;
    }

    private void IndicateEnemyCount()
    {
        enemyCountText.text = "" + enemy_Spawner.enemyCount;
    }

    private void IndicateArrowBar()
    {
        for(int i=0; i<arrowBars.Length; i++)
        {
            if (isReloading_Arrow)
            {
                return;
            }

            Image image = arrowBars[i].GetComponent<Image>();

            if(i<arrows)
            {
                image.color = originalArrowBarColor;
            }
            else
            {                               
                image.color = invisibleArrowBarColor;                            
            }           
        }
    }
    
    private IEnumerator Co_IndicateArrowBarReloading()
    {
        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[0].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[1].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[2].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[3].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[4].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[5].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[6].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[7].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[8].GetComponent<Image>().color = originalArrowBarColor;

        yield return new WaitForSeconds(data[0].ReloadTIme / 10);

        arrowBars[9].GetComponent<Image>().color = originalArrowBarColor;

        StopAllCoroutines();
    }

    private void IndicateArrowOrBullet()
    {
        if(fireMode==FireMode.Burst||fireMode==FireMode.Shotgun)
        {
            ammoUI.SetActive(true);
            arrowUI.SetActive(false);
        }
        else
        {
            ammoUI.SetActive(false);
            arrowUI.SetActive(true);
        }
    }
}
