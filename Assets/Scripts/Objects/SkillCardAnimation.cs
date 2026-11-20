using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCardAnimation : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private int turnCount = 1;
    [SerializeField] private float turnSpeed = 360.0f;
    [SerializeField] private float cardScaleMultiply = 2.0f;

    private Vector2 originalCardScale;
    private Sequence seq;
   
    private bool isturnning = false;
    public bool IsTurnning => isturnning;

    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (isturnning)
            return;
        
        transform.DOScale(originalCardScale * cardScaleMultiply, 0.25f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (isturnning)
            return;

        transform.DOScale(originalCardScale, 0.25f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    private void Awake()
    {
        originalCardScale = transform.localScale;
    }

    private void OnEnable()
    {
        isturnning = true;

        float angle = turnCount * 360.0f;
        float duration = angle / turnSpeed;

        transform.DOKill();
        
        seq = DOTween.Sequence().
                    Append(transform.DOScale(originalCardScale * cardScaleMultiply, duration * 0.5f)).
                    Append(transform.DOScale(originalCardScale, duration * 0.5f)).
                    Insert(0.0f, transform.DORotate(new Vector3(0, angle, 0), duration, RotateMode.FastBeyond360)).
                    SetUpdate(true).
                    SetEase(Ease.OutSine).
                    SetLink(gameObject);

        seq.OnComplete(() =>
        {
            isturnning = false;

            if(isHovering)
                transform.DOScale(originalCardScale * cardScaleMultiply, 0.25f).SetEase(Ease.OutCubic).SetUpdate(true);
        });
    }

    private void OnDisable()
    {
        transform.localScale = originalCardScale;
    }
}
