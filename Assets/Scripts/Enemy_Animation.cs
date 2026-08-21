using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class Enemy
{
    [SerializeField]
    private Enemy_LeftArm enemy_LeftArm;

    [SerializeField]
    private Enemy_RightArm enemy_RightArm;

    private Sequence seq;
    private void Enemy_Punch()
    {
        seq = DOTween.Sequence().
            Append(leftArm.transform.DOLocalMoveX(originalLeftArmPos.x + 0.5f, 0.2f)).SetEase(Ease.OutCubic).
            Append(leftArm.transform.DOLocalMoveX(originalLeftArmPos.x - 0.2f, 0.2f)).SetEase(Ease.OutCubic).
            Append(leftArm.transform.DOLocalMoveX(originalLeftArmPos.x, 0.2f));

        seq = DOTween.Sequence().
            Append(RightArm.transform.DOLocalMoveX(originalRightArmPos.x - 0.2f, 0.2f)).SetEase(Ease.OutCubic).
            Append(RightArm.transform.DOLocalMoveX(originalRightArmPos.x + 0.5f, 0.2f)).SetEase(Ease.OutCubic).
            Append(RightArm.transform.DOLocalMoveX(originalRightArmPos.x, 0.2f));
    }
}
