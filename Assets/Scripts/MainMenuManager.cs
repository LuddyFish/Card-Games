using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Transform buttonList;
    private Button[] buttons;

    void Start()
    {
        buttons = new Button[buttonList.childCount];
        for (int i = 0; i < buttons.Length; i++)
            buttons[i] = buttonList.GetChild(i).GetComponent<Button>();
    }

    void Update()
    {
        buttons[1].interactable = StoredGame();
        buttons[2].interactable = false;
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
