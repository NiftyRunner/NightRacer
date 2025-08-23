using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class TweenData
{
    [Header("Start Transform")]
    public Vector3 startPosition;
    public Vector3 startRotation;
    public Vector3 startScale = Vector3.one;

    [Header("End Transform")]
    public Vector3 endPosition;
    public Vector3 endRotation;
    public Vector3 endScale = Vector3.one;

    [Header("Settings")]
    public float duration;
    public Ease easeType = Ease.Linear;
}

public class IndividualTween : MonoBehaviour
{
    [Header("Setting Bools")]
    [SerializeField] private bool keepPositionSame;
    [SerializeField] private bool keepRotationSame;
    [SerializeField] private bool keepScaleSame;
    
    public TweenData tweenData;


    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        if(keepPositionSame) tweenData.startPosition = tweenData.endPosition = rectTransform.localPosition;

        if(keepRotationSame) tweenData.startRotation = tweenData.endRotation = new Vector3(rectTransform.localRotation.x, rectTransform.localRotation.y, rectTransform.localRotation.z);

        if(keepScaleSame) tweenData.startScale = tweenData.endScale = rectTransform.localScale;
    }

    public Tween PlayForward()
    {
        rectTransform.localPosition = tweenData.startPosition;
        rectTransform.localRotation = Quaternion.Euler(tweenData.startRotation);
        rectTransform.localScale = tweenData.startScale;

        return DOTween.Sequence()
            .Append(rectTransform.DOLocalMove(tweenData.endPosition, tweenData.duration).SetEase(tweenData.easeType))
            .Join(rectTransform.DOLocalRotate(tweenData.endRotation, tweenData.duration).SetEase(tweenData.easeType))
            .Join(rectTransform.DOScale(tweenData.endScale, tweenData.duration).SetEase(tweenData.easeType));
    }

    public Tween PlayBackward()
    {
        return DOTween.Sequence()
            .Append(rectTransform.DOLocalMove(tweenData.startPosition, tweenData.duration).SetEase(tweenData.easeType))
            .Join(rectTransform.DOLocalRotate(tweenData.startRotation, tweenData.duration).SetEase(tweenData.easeType))
            .Join(rectTransform.DOScale(tweenData.startScale, tweenData.duration).SetEase(tweenData.easeType));
    }
}
