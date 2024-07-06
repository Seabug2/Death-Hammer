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
            //게임 매니저가 화면을 고정시킨다.
            Screen.SetResolution(Screen.height/19.5f*9, Screen.height, true);
#endif

            //json 파일에 무기 리스트를 저장함
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
            // JSON 문자열을 파일에 저장
            File.WriteAllText(filePath, jsonData);

            Debug.Log("데이터가 저장되었습니다: " + jsonData);
            Debug.Log(filePath);
        }
    }

    public void SetEquipList(int[] num)
    {
        string filePath = Application.persistentDataPath + "/equipList.json";
        string jsonData = JsonUtility.ToJson(new Serialization<int>(num));
        //string jsonData = JsonUtility.ToJson(num);
        
        // JSON 문자열을 파일에 저장
        File.WriteAllText(filePath, jsonData);
        Debug.Log("데이터가 저장되었습니다: " + jsonData);
        Debug.Log(filePath);
    }

    public void SetWeaponNameList()
    {
        string filePath = Application.persistentDataPath + "/WeaponsNames.json";
        //리소스 폴더 안에 있는 파일들을 전부 불러온다.
        Weapon[] weaponPrefabs = Resources.LoadAll<Weapon>("Weapons");
        //그 수 만큼의 이름 배열을 준비한다.
        string[] weaponPaths = new string[weaponPrefabs.Length];

        print(weaponPrefabs.Length);

        for (int i = 0; i < weaponPrefabs.Length; i++)
        {
            Weapon w = weaponPrefabs[i];
            string path = "Weapons/" + w.gameObject.name; //무기의 경로를 준비하고
            weaponPaths[w.ID] = path; //그 무기의 ID 순서에 맞춰 저장
            Debug.Log("Saved path: " + path);
        }

        // JSON 데이터로 변환
        string jsonData = JsonUtility.ToJson(new Serialization<string>(weaponPaths));

        // JSON 데이터를 파일로 저장
        File.WriteAllText(filePath, jsonData);

        Debug.Log("프리팹 경로가 저장되었습니다: " + jsonData);
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