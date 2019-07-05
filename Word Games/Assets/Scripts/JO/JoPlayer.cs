using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JO
{
    public class JoPlayer : Player
    {
        public Image image;
        public TextMeshProUGUI nameText;

        private Vector2 originalPosition;

        public bool hasBeenGuesser = false;
        public Vector2 guessPosition = new Vector2(0, 400);
        public Vector2 guesserSize = Vector2.one * 1.5f;

        public string guessedWord = string.Empty;


        public void Setup(string name, PlayerColor color, Vector2 position)
        {
            gameObject.name = name;
            nameText.text = name;
            image.color = color.mainColor;
            originalPosition = position;
            image.rectTransform.anchoredPosition = originalPosition;
            image.rectTransform.localScale = Vector2.zero;
        }


        public Vector2 GetPosition()
        {
            return image.rectTransform.anchoredPosition;
        }


        private Tween revealTween;

        public void Show()
        {
            revealTween.Kill();
            revealTween = image.rectTransform.DOScale(Random.Range(0.8f, 1.2f), 0.7f).SetEase(Ease.OutBack);
        }

        public void Hide(System.Action onComplete = null)
        {
            revealTween.Kill();
            revealTween = image.rectTransform.DOScale(0f, 0.7f).SetEase(Ease.InBack).OnComplete(() => {
                onComplete?.Invoke();
            });
        }

        public void OffScreen(float delay)
        {
            revealTween.Kill();
            float y = originalPosition.y - 1080f;
            revealTween = image.rectTransform.DOAnchorPosY(y, 2f).SetEase(Ease.InQuad).SetDelay(delay);
        }

        public void OnScreen(float delay)
        {
            revealTween.Kill();
            revealTween = image.rectTransform.DOAnchorPosY(originalPosition.y, 2f).SetEase(Ease.OutQuad).SetDelay(delay);
        }


        public void SetAsGuesser()
        {
            hasBeenGuesser = true;
            guessedWord = "<GUESSER>";
            
            revealTween.Kill();
            image.rectTransform.DOScale(guesserSize, 1.2f);
            image.rectTransform.DOAnchorPos(guessPosition, 1.2f);
        }
    }
}

