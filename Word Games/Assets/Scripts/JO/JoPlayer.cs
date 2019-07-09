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
        public TextMeshProUGUI bigNameText, smallNameText, wordText;

        private Vector2 originalPosition;

        public bool hasBeenGuesser = false;
        public Vector2 guessPosition = new Vector2(0, 400);
        public Vector2 guesserSize = Vector2.one * 1.5f;

        public bool isGuesser = false;

        public string guessedWord = string.Empty;


        public void Setup(string name, PlayerColor color, Vector2 position)
        {
            gameObject.name = name;
            bigNameText.text = name;
            smallNameText.text = name;
            wordText.text = string.Empty;
            image.color = color.mainColor;
            originalPosition = position;
            image.rectTransform.anchoredPosition = originalPosition;
            image.rectTransform.localScale = Vector2.zero;

            this.color = color;

            smallNameText.gameObject.SetActive(false);
            wordText.gameObject.SetActive(false);
        }


        public Vector2 GetPosition()
        {
            return image.rectTransform.anchoredPosition;
        }


        private Tween revealTween;

        public void Show(float delay)
        {
            revealTween.Kill();
            image.rectTransform.anchoredPosition = originalPosition;
            image.rectTransform.localScale = Vector2.zero;
            revealTween = image.rectTransform.DOScale(Random.Range(0.8f, 1.2f), 0.7f).SetEase(Ease.OutBack).SetDelay(delay);
        }

        public void Hide(System.Action onComplete = null)
        {
            revealTween.Kill();
            revealTween = image.rectTransform.DOScale(0f, 0.7f).SetEase(Ease.InBack).OnComplete(() => {
                onComplete?.Invoke();
            });
        }

        public void OffScreen(float delay, System.Action onComplete)
        {
            revealTween.Kill();
            float y = originalPosition.y - 1080f;
            revealTween = image.rectTransform.DOAnchorPosY(y, 2f).SetEase(Ease.InQuad).SetDelay(delay).OnComplete(() => {
                onComplete?.Invoke();
            });
        }

        public void OnScreen(float delay)
        {
            revealTween.Kill();
            revealTween = image.rectTransform.DOAnchorPosY(originalPosition.y, 2f).SetEase(Ease.OutQuad).SetDelay(delay);
        }


        public void SetAsGuesser()
        {
            isGuesser = true;
            hasBeenGuesser = true;
            guessedWord = "<GUESSER>";
            
            revealTween.Kill();
            image.rectTransform.DOScale(guesserSize, 1.2f);
            image.rectTransform.DOAnchorPos(guessPosition, 1.2f);
        }

        public void SetWord(string word)
        {
            guessedWord = word;
            bigNameText.gameObject.SetActive(false);
            smallNameText.gameObject.SetActive(true);
            wordText.gameObject.SetActive(true);
            wordText.text = word;
        }

        public void Reset()
        {
            isGuesser = false;
            guessedWord = string.Empty;

            bigNameText.gameObject.SetActive(true);
            smallNameText.gameObject.SetActive(false);
            wordText.gameObject.SetActive(false);
            wordText.text = string.Empty;
        }
    }
}

