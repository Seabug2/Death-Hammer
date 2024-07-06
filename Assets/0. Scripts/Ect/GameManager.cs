using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

#if UNITY_STANDALONE_WIN
            //���� �Ŵ����� ȭ���� ������Ų��.
            Screen.SetResolution(Screen.height/19.5f*9, Screen.height, true);
#endif

            //json ���Ͽ� ���� ����Ʈ�� ������
            SetWeaponNameList();
            EquipListCheck();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    CameraControl mainCamCtrl;
    public CameraControl MainCamCtrl
    {
        get
        {
            if (!mainCamCtrl)
            {
                Camera.main.TryGetComponent<CameraControl>(out mainCamCtrl);
            }
            return mainCamCtrl;
        }
    }

    Player player;
    public Player Player
    {
        get {
            if (!player)
            {
                GameObject.FindWithTag("Player").TryGetComponent<Player>(out player);
            }
            return player;
        }
    }

    BGMManager bgmManager;
    public BGMManager BGMManager
    {
        get {
            if (!bgmManager)
            {
                TryGetComponent<BGMManager>(out bgmManager);
            }
            return bgmManager;
        }
    }

    void EquipListCheck()
    {
        string filePath = Application.persistentDataPath + "/equipList.json";

        if (!File.Exists(filePath))
        {
            int[] num = new int[4] { 0, 1, 2, 3 };

            string jsonData = JsonUtility.ToJson(new Serialization<int>(num));
            //string jsonData = JsonUtility.ToJson(num);
            // JSON ���ڿ��� ���Ͽ� ����
            File.WriteAllText(filePath, jsonData);

            Debug.Log("�����Ͱ� ����Ǿ����ϴ�: " + jsonData);
            Debug.Log(filePath);
        }
    }

    public void SetEquipList(int[] num)
    {
        string filePath = Application.persistentDataPath + "/equipList.json";
        string jsonData = JsonUtility.ToJson(new Serialization<int>(num));
        //string jsonData = JsonUtility.ToJson(num);
        
        // JSON ���ڿ��� ���Ͽ� ����
        File.WriteAllText(filePath, jsonData);
        Debug.Log("�����Ͱ� ����Ǿ����ϴ�: " + jsonData);
        Debug.Log(filePath);
    }

    public void SetWeaponNameList()
    {
        string filePath = Application.persistentDataPath + "/WeaponsNames.json";
        //���ҽ� ���� �ȿ� �ִ� ���ϵ��� ���� �ҷ��´�.
        Weapon[] weaponPrefabs = Resources.LoadAll<Weapon>("Weapons");
        //�� �� ��ŭ�� �̸� �迭�� �غ��Ѵ�.
        string[] weaponPaths = new string[weaponPrefabs.Length];

        print(weaponPrefabs.Length);

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            Weapon w = weaponPrefabs[i];
            string path = "Weapons/" + w.gameObject.name; //������ ��θ� �غ��ϰ�
            weaponPaths[w.ID] = path; //�� ������ ID ������ ���� ����
            Debug.Log("Saved path: " + path);
        }

        // JSON �����ͷ� ��ȯ
        string jsonData = JsonUtility.ToJson(new Serialization<string>(weaponPaths));

        // JSON �����͸� ���Ϸ� ����
        File.WriteAllText(filePath, jsonData);

        Debug.Log("������ ��ΰ� ����Ǿ����ϴ�: " + jsonData);
        Debug.Log("File saved to: " + filePath);
    }
}

[System.Serializable]
public class Serialization<T>
{
    public T[] items;
    public Serialization(T[] items)
    {
        this.items = items;
    }
}