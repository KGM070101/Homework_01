using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public partial class Player
{
    private DG.Tweening.Sequence seq;

    private void WeaponBounceVer1()
    {
        seq = DOTween.Sequence().
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y*0.6f,0.05f)).
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y * 1.3f, 0.05f)).
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y * 1.0f, 0.05f));

        seq = DOTween.Sequence().
             Append(Weapon.transform.DOScaleX(OriginalWeaponSize.x * 1.5f, 0.075f)).
             Append(Weapon.transform.DOScaleX(OriginalWeaponSize.x * 1.0f, 0.075f));       
    }

    private void WeaponBounceVer2()
    {
        seq = DOTween.Sequence().
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y * 0.4f, 0.05f)).
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y * 1.1f, 0.05f)).
             Append(Weapon.transform.DOScaleY(OriginalWeaponSize.y * 1.0f, 0.05f));

        seq = DOTween.Sequence().
             Append(Weapon.transform.DOScaleX(OriginalWeaponSize.x * 2.0f, 0.075f)).
             Append(Weapon.transform.DOScaleX(OriginalWeaponSize.x * 1.0f, 0.075f));
    }

    private void WheelAnimation()
    {
        seq = DOTween.Sequence().
            Append(LeftWheel.transform.DOScaleX(OriginalWheelSize.x * 1.1f, 0.01f)).
            Append(LeftWheel.transform.DOScaleX(OriginalWheelSize.x * 1.0f, 0.01f));

        seq = DOTween.Sequence().
            Append(RightWheel.transform.DOScaleX(OriginalWheelSize.x * 1.1f, 0.01f)).
            Append(RightWheel.transform.DOScaleX(OriginalWheelSize.x * 1.0f, 0.01f));        
    }

    private IEnumerator Co_WheelAnimationStart()
    {
        while(isMoving)
        {
            WheelAnimation();

            yield return new WaitForSeconds(0.05f);
        }

        coroutine = null;
    }
}
