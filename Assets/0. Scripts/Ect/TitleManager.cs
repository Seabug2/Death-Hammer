using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TitleManager : MonoBehaviour
{
    // ������ ������ ��񸮽�Ʈ
    [SerializeField]
    List<int> selectedNumList;

    // Ÿ��Ʋ â�� �ε�Ǹ� �κ��丮�� �غ��Ѵ�.
    //
    // 1. ���ҽ� ������ ���� �������� icons�� ����.
    // 2. json ���Ϸκ��� ���� ������ ���� ���� ���⸦ Ȯ���Ͽ� icons�� ���� ǥ��(mark)�� Ȱ��ȭ�Ѵ�.
    [SerializeField]
    ButtonActiveInTitle[] buttons;

    // �ɼ� ��ư�� ���� �κ��丮 â�� ����,
    // ���⸦ �����ϰ� ������ �� �� �ִ�.

    //Awake������ ���ӸŴ����� �۾��� ���� �Ǿ�� �ϹǷ� ������� ����
    private void Start()
    {
        UISetting();
    }

    //����ȭ�鿡�� ���� ���� ��� �����ִ� �⺻ ����
    private void UISetting()
    {
        selectedNumList = new List<int>();

        //���� ���ҽ� ����� weapons ��� ���� ���� ��� weapon �������� ���� �ɴϴ�.
        Weapon[] weapons = Resources.LoadAll<Weapon>("Weapons");

        //���ҽ��� �ҷ��Ϳ�...
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[weapons[i].ID].Init(weapons[i].ID, weapons[i].IconImage, selectedNumList);
        }
        
        //json���� ������ �÷��̾� �����͸� �����ɴϴ�.
        string filePath = Application.persistentDataPath + "/equipList.json";
        string json = File.ReadAllText(filePath);
        int[] equipList = JsonUtility.FromJson<Serialization<int>>(json).items;

        for (int i = 0; i < equipList.Length; i++)
        {
            buttons[equipList[i]].isSelected = true;
            buttons[equipList[i]].myMark.SetActive(true);
            selectedNumList.Add(equipList[i]);
        }

        //���� �������� ��ȣ ����Ʈ�� ����˴ϴ�.
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
