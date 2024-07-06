using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class WeaponManager : MonoBehaviour
{
    //1. 플레이어 데이터를 불러온다
    //2. 저장된 무기 4개를 리소스 폴더로 부터 불러와 하위에 둔다.

    [SerializeField]
    Inventory inventory;

    [SerializeField]
    Image[] icons;
    public Weapon GetWeapon(int _num)
    {
        return inventory[_num];
    }
    
    public void WeaponEquip()
    {
        inventory = new Inventory(4);

        string filePath = Application.persistentDataPath + "/equipList.json";
        string jsonData = File.ReadAllText(filePath);
        Serialization<int>  equipList = JsonUtility.FromJson<Serialization<int>>(jsonData);

        filePath = Application.persistentDataPath + "/WeaponsNames.json";
        jsonData = File.ReadAllText(filePath);
        Serialization<string> paths = JsonUtility.FromJson<Serialization<string>>(jsonData);

        for (int i = 0; i < 4; i++)
        {
            Weapon wp = Instantiate(Resources.Load<Weapon>(paths.items[equipList.items[i]]),transform);
            inventory[i] = wp;
            wp.gameObject.SetActive(false);
            icons[i].sprite = inventory[i].IconImage;
            icons[i].SetNativeSize();
        }

        inventory[0].gameObject.SetActive(true);
    }
}

[System.Serializable]
public class Inventory
{
    [SerializeField]
    Weapon[] list;

    // 배열 인덱서 정의
    public Weapon this[int index]
    {
        get
        {
            if (index >= list.Length)
                return list[list.Length - 1];

            else if (index < 0)
                return list[0];

            // 배열 요소에 접근할 때 호출되는 로직
            else
                return list[index];
        }
        set
        {
            if (index >= list.Length)
                list[list.Length - 1] = value;

            else if (index < 0)
                list[0] = value;

            // 배열 요소에 접근할 때 호출되는 로직
            else
                list[index] = value;
        }
    }

    public Inventory(int _weaponsCount)
    {
        list = new Weapon[_weaponsCount];
    }

    public int Length
    {
        get { return list.Length; }
    }
}
