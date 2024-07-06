using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShowResult : MonoBehaviour
{
    public int score;

    [SerializeField]
    TextMeshProUGUI resultScroeUI;

    [SerializeField]
    float countUpTerm, countUpTime;

    [SerializeField]
    GameObject button;

    [SerializeField]
    float buttonTerm;

    private void Start()
    {
        button.SetActive(false);
    }

    public void PlayBounceAudio()
    {
        if(TryGetComponent<AudioSource>(out AudioSource _audio))
        {
            _audio.Play();
        }
    }


    public IEnumerator ShowScore_co()
    {
        Animator anim = GetComponent<Animator>();
        resultScroeUI.text = Mathf.FloorToInt(score).ToString();

        yield return new WaitWhile(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

        //ÆÄÆ¼Å¬

        yield return new WaitForSeconds(buttonTerm);
        button.SetActive(true);
    }

    public Animator fadeInOut;

    public void CloseScene()
    {
        StartCoroutine(CloseScene_co());
    }

    IEnumerator CloseScene_co()
    {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("Next");
        yield return null;
        yield return new WaitWhile(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

        fadeInOut.SetTrigger("Fade Out");
        yield return null;
        yield return new WaitWhile(() => fadeInOut.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

        SceneManager.LoadScene(0);
    }

}
