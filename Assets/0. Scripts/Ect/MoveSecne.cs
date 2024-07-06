using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveSecne : MonoBehaviour
{
    public void NextScene(string _sceneNamde)
    {
        SceneManager.LoadScene(_sceneNamde);
    }
}
