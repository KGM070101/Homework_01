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
    private TextMeshProUGUI fireModeText;
    [SerializeField]
    private TextMeshProUGUI fireModeAnnouncementText;
    [SerializeField]
    private Image playerHpBar;
    [SerializeField]
    private TextMeshProUGUI playerHpText;

    private float maxAmmo = 99;
    private float currentAmmo;
    private float reloadTime = 2.0f;
    private float maxHP;
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
        playerHpText.text = "HP :" + maxHP;
    }

    private void IndicatePlayerHpBar()
    {
        playerHpBar.fillAmount = maxHP / data[0].MaxHp;
    }

    private IEnumerator Co_WaitIndicateAnnouncement()
    {
        yield return new WaitForSeconds(0.2f);

        IndicateFireModeAnnoucement();        
    }    
}
