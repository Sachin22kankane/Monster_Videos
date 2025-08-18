using System;
using TMPro;
using UnityEngine;

public class FloatingText : PoolableObject
{
    [SerializeField] private TMP_Text scoreText;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void ShowText(int score)
    {
        gameObject.SetActive(true);
        scoreText.text = "+" + score.ToString();
        anim.Play();
    }

    public void DisableText()
    {
        gameObject.SetActive(false);
        pool?.Release(this);
    }
}
