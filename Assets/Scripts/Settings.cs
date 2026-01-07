using UnityEngine;

public class Settings : MonoBehaviour, IDataPersistence<PlayerSettings>
{
    [Range(0f, 1f)]
    public float volume;

    private BackgroundColours.BackgroundColour _selectedColour;
    public BackgroundColours BGColours;

    public bool highContrast;

    private void Reset()
    {
        volume = 1f;
        highContrast = false;
    }

    public void LoadData(PlayerSettings data)
    {

    }

    public void SaveData(ref PlayerSettings data)
    {

    }

    public void SetNewBGColour(BackgroundColours.BackgroundColour newColour)
    {
        _selectedColour = newColour;
    }
}
