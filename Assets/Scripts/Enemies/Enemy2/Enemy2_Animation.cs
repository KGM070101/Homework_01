using DG.Tweening;
using UnityEngine;

public partial class Enemy2
{
    [SerializeField]
    private Enemy2_Weapon enemy2_Weapon;

    private Sequence seq;

    private void WeaponBounce()
    {
        seq = DOTween.Sequence().
             Append(enemy2_Weapon.transform.DOScaleY(originalWeaponSize.y * 0.6f, 0.05f)).
             Append(enemy2_Weapon.transform.DOScaleY(originalWeaponSize.y * 1.3f, 0.05f)).
             Append(enemy2_Weapon.transform.DOScaleY(originalWeaponSize.y * 1.0f, 0.05f));

        seq = DOTween.Sequence().
             Append(enemy2_Weapon.transform.DOScaleX(originalWeaponSize.x * 1.5f, 0.075f)).
             Append(enemy2_Weapon.transform.DOScaleX(originalWeaponSize.x * 1.0f, 0.075f));
    }
}
