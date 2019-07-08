using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

namespace JO
{
    public class Logo : MonoBehaviour
    {
        public Image logoImage;
        public ParticleSystem logoParticles;
        public TextMeshProUGUI instructionsText;

        private Tween revealTween, hoverTween;


        private void Awake()
        {
            logoImage.rectTransform.localScale = Vector2.one * 0.85f;
            logoParticles.Stop();
        }

        public void Show(string gameUrl)
        {
            instructionsText.text = "Go to " + gameUrl + " to join the game!";

            if (revealTween != null) { revealTween.Kill(); revealTween = null; }
            if (hoverTween != null) { hoverTween.Kill(); hoverTween = null; }

            logoImage.rectTransform.localScale = Vector2.zero;
            revealTween = logoImage.rectTransform.DOScale(Vector2.one * 0.85f, 0.8f).SetEase(Ease.OutBack).OnComplete(() => {
                logoParticles.Play();
                hoverTween = logoImage.rectTransform.DOScale(Vector2.one, 5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            });

            instructionsText.gameObject.SetActive(true);
        }

        public void Hide(System.Action onComplete = null)
        {
            if (revealTween != null) { revealTween.Kill(); revealTween = null; }
            if (hoverTween != null) { hoverTween.Kill(); hoverTween = null; }

            logoParticles.Stop();
            revealTween = logoImage.rectTransform.DOScale(Vector2.zero, 0.4f).OnComplete(() => {
                onComplete?.Invoke();
            });

            instructionsText.gameObject.SetActive(false);
        }


        [ButtonMethod]
        public void TestShow() { Show("TESTING"); }

        [ButtonMethod]
        public void TestHide() { Hide(); }
    }
    
}

