using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip slash;
    public AudioClip attack;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip hit;
    public AudioClip bossSlash;
    public AudioClip bossAttack;
    public AudioClip bossJumpAttack;

    AudioSource audio;

    public static SoundEffect instance;

    void Awake()
    {
        audio = GetComponent<AudioSource>();

        if (SoundEffect.instance == null)
            SoundEffect.instance = this;
    }

    void Update()
    {
        audio.volume =  GameManager.instance.SESlider.value / 2;
    }

    public void SlashSound()
    {
        audio.clip = slash;
        audio.Play();
    }

    public void AttackSound()
    {
        audio.PlayOneShot(attack);
    }

    public void JumpSound()
    {
        audio.PlayOneShot(jump);
    }

    public void SlideSound()
    {
        audio.PlayOneShot(slide);
    }

    public void HitSound()
    {
        audio.PlayOneShot(hit, audio.volume * 3f);
    }

    public void BossSlash()
    {
        audio.clip = bossSlash;
        audio.Play();
    }

    public void BossAttack()
    {
        audio.PlayOneShot(bossAttack);
    }

    public void BossJumpAttack()
    {
        audio.PlayOneShot(bossJumpAttack, audio.volume * 0.95f);
    }
}
