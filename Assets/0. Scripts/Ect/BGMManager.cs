using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> BGMs = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> gameStart = new List<AudioClip>();
    [SerializeField]
    List<AudioClip> gameOver = new List<AudioClip>();

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        LoadAudioClips("BGM", BGMs);
        LoadAudioClips("Start Audio", gameStart);
        LoadAudioClips("Game Over Audio", gameOver);
    }

    void LoadAudioClips(string _key, List<AudioClip> _audioClipList)
    {
        Addressables.LoadAssetsAsync<AudioClip>(_key, null).Completed +=
            (AsyncOperationHandle<IList<AudioClip>> obj) =>
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
                _audioClipList.AddRange(obj.Result);
            else
                Debug.LogError("Failed to load AudioClips.");
        };
    }

    public void InGameBGMStart()
    {
        if (BGMLoop != null)
            StopCoroutine(BGMLoop);
        // BGM 재생 시작
        BGMLoop = StartCoroutine(BGM_co());
    }

    Coroutine BGMLoop;

    IEnumerator BGM_co()
    {
        while (gameStart.Count.Equals(0))
        {
            yield return new WaitForFixedUpdate();
        }

        audioSource.clip = gameStart[Random.Range(0, gameStart.Count)];
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);


        while (BGMs.Count > 0)
        {
            AudioClip clip;
            do
            {
                clip = BGMs[Random.Range(0, gameStart.Count)];
            }
            while (audioSource.clip.Equals(clip));

            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    public void BGMStop()
    {
        if (BGMLoop != null)
            StopCoroutine(BGMLoop);
        audioSource.Stop();
    }

    public IEnumerator GameOver_co()
    {
        AudioClip clip = gameOver[Random.Range(0, gameOver.Count)];
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
        yield return new WaitWhile(() => audioSource.isPlaying);
    }
}
