// #define NO_BGM

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    const float SECOUND_FADE_IN = 2f;
    const float SECOUND_FADE_OUT = 1.5f;

    public static AudioManager Script()
    {
        return GameObject.Find(Define.ObjName.Global).GetComponent<AudioManager>();
    }

    [SerializeField]
    AudioSource audioSource = null;

    Coroutine fading;

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);

        audioSource.clip = null;
        audioSource.loop = true;
        fading = null;
    }

    void BGMPlay(string name)
    {
        audioSource.clip = ResourceManager.Instance.LoadBGM(name) as AudioClip;

        switch (name)
        {
            case Define.Path.BGM.Title:
                audioSource.time = 14.5f;
                break;
            case Define.Path.BGM.Battle:
                audioSource.time = 17.5f;
                break;
        }

#if NO_BGM
#else
        audioSource.Play(1);
#endif
    }

    public void BGMCrossFade(string clipName)
    {
        if (audioSource.isPlaying)
            _BGMCrossFade(clipName);
        else 
            BGMFadeIn(clipName);
    }

    void BGMFadeIn(string clipName)
    {
        this.BGMPlay(clipName);

        fading = StartCoroutine(Coroutine_.Action.CustomLerpOld(SECOUND_FADE_IN, setVolume));
    }

    void _BGMCrossFade(string clipName)
    {
        if (fading != null)
        {
            StopCoroutine(fading);
        }

        fading = StartCoroutine(Coroutine_.Action.CrossFading(
            // Fade 中でもその時点での音量から fade out
            Coroutine_.Action.CustomLerpReverse(SECOUND_FADE_OUT, (float f) =>
            {
                this.LerpVolume(0f, this.audioSource.volume, f);
            }),
            Coroutine_.Action.CustomLerpOld(SECOUND_FADE_IN, this.setVolume),
            () => this.BGMPlay(clipName)));
    }

    void setVolume(float f)
    {
        this.audioSource.volume = f;
    }

    void LerpVolume (float start, float end, float f)
    {
        this.audioSource.volume = Mathf.Lerp(start, end, f);
    }
}
