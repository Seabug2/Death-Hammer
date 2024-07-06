using UnityEngine;

public class DoYouWantToGoTitle : MonoBehaviour
{
    public GameObject popUpUI;

   public void PopUpUI() {

        if(!popUpUI.activeSelf)
            popUpUI.SetActive(true);
        else 
            popUpUI.SetActive(false);
    }
}
