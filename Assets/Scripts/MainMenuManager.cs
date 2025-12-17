using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Transform buttonList;
    private GameObject[] buttonObjs;
    private Button[] buttons;

    void Start()
    {
        buttonObjs = new GameObject[buttonList.childCount];
        buttons = new Button[buttonList.childCount];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonObjs[i] = buttonList.GetChild(i).gameObject;
            buttons[i] = buttonList.GetChild(i).GetComponent<Button>();
        }
    }

    void Update()
    {
        buttonObjs[0].SetActive(StoredGame());
        buttonObjs[2].SetActive(false);
        buttons[3].interactable = false;
    }

    bool StoredGame()
    {
        if (DataPersistenceManager.Instance != null)
        {
            return File.Exists(DataPersistenceManager.Instance.data.FilePath);
        }
        return false;
    }
}
