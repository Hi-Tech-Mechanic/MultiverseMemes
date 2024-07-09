using System.Collections;
using UnityEngine;

public class StartLaunchAnimation : MonoBehaviour
{
    [SerializeField] private MusicOptions _musicOptions;
    [SerializeField] private Animator[] _heroAnimators;
    private GameObject[] _gameObject;
    public static bool StartEffectComplete = false;

    private void Start()
    {
        _gameObject = new GameObject[_heroAnimators.Length];

        for (int i = 0; i < _heroAnimators.Length; i++)
        {
            _gameObject[i] = _heroAnimators[i].gameObject;
        }

        _musicOptions.GetMusicPlayer().clip = _musicOptions.GetMusicBuffer()[1];
        _musicOptions.GetMusicPlayer().time = 16;
        _musicOptions.GetMusicPlayer().Play();
        StartCoroutine(PeriodicShowHeroes());
    }

    IEnumerator PeriodicShowHeroes()
    {
        int i = 0;
        foreach (Animator animator in _heroAnimators)
        {
            _gameObject[i].SetActive(true);
            animator.SetTrigger("Active");
            i++;
            yield return new WaitForSeconds(0.35f);
        }

        while (_musicOptions.GetMusicPlayer().volume > 0.1f)
        {
            Debug.Log("ffffffffff");
            _musicOptions.GetMusicPlayer().volume -= 0.001f;
            yield return new WaitForSeconds(0.02f);
        }

        StartEffectComplete = true;
        _musicOptions.GetMusicPlayer().time = 0;
        _musicOptions.Init();
    }
}
