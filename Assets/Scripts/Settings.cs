using UnityEngine;

public class Settings : MonoBehaviour, IDataPersistence
{
    [field: SerializeField, Range(0f, 1f)]
    public float Volume { get; set; }

    [HideInInspector] public BackgroundColours.BackgroundColour selectedColour;
    [field: SerializeField]
    public BackgroundColours BGColours { get; set; }
    [SerializeField] private Material _BGMaterial;

    [field: SerializeField]
    public bool HighContrast { get; set; }

    private void Reset()
    {
        Volume = 1f;
        HighContrast = false;
    }

    public void LoadData(GameData data)
    {
        //Volume = data.volume;
        //SetNewBGColour(data.backgroundId);
        //HighContrast = data.highConstrast;
    }

    public void SaveData(ref GameData data)
    {
        //data.volume = Volume;
        //data.backgroundId = selectedColour.name;
        //data.highConstrast = HighContrast;
    }

    private void SetBGMaterial()
    {
        _BGMaterial.SetColor("_PrimaryColour", selectedColour.primary);
        _BGMaterial.SetColor("_SecondaryColour", selectedColour.secondary);
    }

    public void SetNewBGColour(BackgroundColours.BackgroundColour newColour)
    {
        selectedColour = newColour;
        SetBGMaterial();
    }

    public void SetNewBGColour(string newColour)
    {
        selectedColour = BGColours.Get(newColour);
        SetBGMaterial();
    }

    public void SetNewBGColour(int newColour)
    {
        selectedColour = BGColours.Get(newColour);
        SetBGMaterial();
    }
}
