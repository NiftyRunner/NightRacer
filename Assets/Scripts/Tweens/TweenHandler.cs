using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class TweenHandler : MonoBehaviour
{
    [SerializeField] private List<IndividualTween> tweens;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool sequential = true;

    void Start()
    {
        if (playOnStart) PlaySequence();
    }

    public void PlaySequence()
    {
        Sequence sequence = DOTween.Sequence();

        foreach(var tween in tweens)
        {
            if (sequential)
                sequence.Append(tween.PlayForward());
            else
                sequence.Join(tween.PlayForward());
        }

        sequence.Play();
    }

    public void PlaySequenceReverse()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (var tween in tweens)
        {
            if (sequential)
                sequence.Append(tween.PlayBackward());
            else
                sequence.Join(tween.PlayBackward());
        }

        sequence.Play();
    }
}
