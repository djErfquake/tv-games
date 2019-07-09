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

        public TextMeshProUGUI text;

        public ParticleSystem particles;
        public Gradient correctColor, incorrectColor;


        private void Start()
        {
            particles.Stop();
            image.rectTransform.localScale = Vector2.zero;
        }



        public void Show(string guess, bool correct, Color playerColor)
        {
            image.rectTransform.localScale = Vector2.zero;

            image.color = playerColor;

            ParticleSystem.MainModule particleMain = particles.main;
            particleMain.startColor = correct ? correctColor : incorrectColor;

            text.text = guess;

            image.rectTransform.DOScale(1f, 0.8f).SetEase(Ease.OutElastic);
            particles.Play();
        }

        public void Hide()
        {
            particles.Stop();
            image.rectTransform.DOScale(0f, 0.4f).SetEase(Ease.InBack);
        }

    }
}
