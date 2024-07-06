using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TitleManager : MonoBehaviour
{
    // 유저가 선택한 장비리스트
    [SerializeField]
    List<int> selectedNumList;

    // 타이틀 창이 로드되면 인벤토리를 준비한다.
    //
    // 1. 리소스 폴더의 무기 아이콘을 icons에 띄운다.
    // 2. json 파일로부터 현재 유저가 장착 중인 무기를 확인하여 icons에 장착 표시(mark)를 활성화한다.
    [SerializeField]
    ButtonActiveInTitle[] buttons;

    // 옵션 버튼을 눌러 인벤토리 창을 띄우면,
    // 무기를 장착하고 해제를 할 수 있다.

    //Awake에서는 게임매니저의 작업이 선행 되어야 하므로 사용하지 않음
    private void Start()
    {
        UISetting();
    }

    //시작화면에서 장착 중인 장비를 보여주는 기본 세팅
    private void UISetting()
    {
        selectedNumList = new List<int>();

        //먼저 리소스 경로의 weapons 라는 폴더 안의 모든 weapon 프리팹을 가져 옵니다.
        Weapon[] weapons = Resources.LoadAll<Weapon>("Weapons");

        //리소스를 불러와요...
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[weapons[i].ID].Init(weapons[i].ID, weapons[i].IconImage, selectedNumList);
        }
        
        //json으로 저장한 플레이어 데이터를 가져옵니다.
        string filePath = Application.persistentDataPath + "/equipList.json";
        string json = File.ReadAllText(filePath);
        int[] equipList = JsonUtility.FromJson<Serialization<int>>(json).items;

        for (int i = 0; i < equipList.Length; i++)
        {
            buttons[equipList[i]].isSelected = true;
            buttons[equipList[i]].myMark.SetActive(true);
            selectedNumList.Add(equipList[i]);
        }

        //현재 선택중인 번호 리스트가 저장됩니다.
        print(selectedNumList);
    }

    [SerializeField]
    string inGameScene;
    [SerializeField]
    Button[] actionButtons;
    [SerializeField]
    Animator FadeInOut;

    public void ReadyToStart()
    {
        if (selectedNumList.Count.Equals(4))
        {

            StartCoroutine(GoToTheNext_co());
        }
        else
        {
            ShowWarning();
        }
    }
    
    IEnumerator GoToTheNext_co()
    {
        inventory.SetActive(false);
        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].enabled = false;
        }

        GameManager.instance.SetEquipList(selectedNumList.ToArray());
        FadeInOut.SetTrigger("Fade Out");
        yield return null;
        yield return new WaitWhile(()=> FadeInOut.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);

        SceneManager.LoadScene(inGameScene);
    }

    public Warning warning;
    public void ShowWarning()
    {
        warning.gameObject.SetActive(true);
        warning.Messege(selectedNumList.Count);
    }

    public GameObject inventory;
    public void ShowInventory()
    {
        if (!inventory.activeSelf)
        {
            inventory.SetActive(true);
        }
        else
        {
            if (selectedNumList.Count.Equals(4))
            {
                inventory.SetActive(false);
            }
            else
            {
                ShowWarning();
            }
        }
    }

}
