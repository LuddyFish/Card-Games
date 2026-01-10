using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Transform buttonList;
    private GameObject[] _buttonObjs;
    private Button[] _buttons;

    void Start()
    {
        _buttonObjs = new GameObject[buttonList.childCount];
        _buttons = new Button[buttonList.childCount];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttonObjs[i] = buttonList.GetChild(i).gameObject;
            _buttons[i] = buttonList.GetChild(i).GetComponent<Button>();
        }
    }

    void Update()
    {
        _buttonObjs[0].SetActive(StoredGame());
        _buttonObjs[2].SetActive(false);
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
