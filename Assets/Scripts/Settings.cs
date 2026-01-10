using UnityEngine;

public class Settings : MonoBehaviour, IDataPersistence<PlayerSettings>
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

    public void LoadData(PlayerSettings data)
    {

    }

    public void SaveData(ref PlayerSettings data)
    {

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
