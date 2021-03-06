﻿namespace UniGame.ViewSystem.Backgrounds
{
    using System.Collections;
    using UniGame.UiSystem.Runtime;
    using UniGame.UiSystem.Runtime.Backgrounds.Abstract;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UnityEngine;

    public class DefaultFadeBackgroundView : UiCanvasGroupView<IBackgroundViewModel>, IBackgroundView
    {
        [SerializeField, Range(0.0f, 1.0f)]
        public float duration = 0.3f;

        
        protected override IEnumerator OnHidingProgress(ILifeTime progressLifeTime)
        {
            yield return AnimateFade(1, 0, duration);
        }

        protected override IEnumerator OnShowProgress(ILifeTime progressLifeTime)
        {
            yield return AnimateFade(0, 1, duration);
        }

        private IEnumerator AnimateFade(float fromAlpha, float toAlpha, float time)
        {
            if (time <= 0)
            {
                canvasGroup.alpha = toAlpha;
                yield break;
            }
            
            var currentAlpha = fromAlpha;
            var timePassed = 0f;
            canvasGroup.alpha = fromAlpha;

            while (timePassed < time)
            {
                var stage = timePassed / time;
                currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, stage);
                canvasGroup.alpha = currentAlpha;
                timePassed += Time.deltaTime;
                yield return null;
            }


        }

    }
}