using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace JO
{
    public class Answer : MonoBehaviour
    {
        public Image image;

        public TextMeshProUGUI guessText, playerNameText, actualWordText;

        public ParticleSystem particles;
        public Gradient correctColor, incorrectColor;

        public float showTime = 10f;


        private void Start()
        {
            particles.Stop();
            image.rectTransform.localScale = Vector2.zero;
        }



        public void Show(string guess, bool correct, JoPlayer player, string actualAnswer, System.Action onComplete)
        {
            image.rectTransform.localScale = Vector2.zero;

            image.color = player.color.mainColor;

            ParticleSystem.MainModule particleMain = particles.main;
            particleMain.startColor = correct ? correctColor : incorrectColor;

            guessText.text = guess;
            playerNameText.text = $"{player.name} guessed";
            actualWordText.text = $"The word was {actualAnswer}";

            image.rectTransform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
            particles.Play();

            StartCoroutine(ExhibitUtilities.DoActionAfterTime(() => { onComplete.Invoke(); }, showTime));
        }

        public void Hide()
        {
            particles.Stop();
            image.rectTransform.DOScale(0f, 0.4f).SetEase(Ease.InBack);
        }

    }
}
