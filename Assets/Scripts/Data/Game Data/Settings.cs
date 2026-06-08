using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Settings : MonoBehaviour, IDataPersistence
{
    private Cardbox Box => Cardbox.Instance;

    /* =================================== */
    [field: Header("Field Values"), SerializeField, Range(0f, 1f)]
    public float Volume { get; set; }

    [HideInInspector] public BackgroundColours.BackgroundColour selectedColour;
    [field: FormerlySerializedAs("<BGColours>k__BackingField"), SerializeField]
    public BackgroundColours bgColours { get; set; }
    [FormerlySerializedAs("_BGMaterial")] 
    [SerializeField] private Material _bgMaterial;

    [field: SerializeField]
    public bool HighContrast { get; set; }
    
    // --- UI ---
    [Header("UI Elements")]
    [SerializeField] private GameObject _volumeSlider;
    private Slider _slider;

    [FormerlySerializedAs("_BGCDropdown")]
    [SerializeField] private GameObject _bgcDropdown;
    private Dropdown _dropdown;

    [SerializeField] private GameObject _contrastButton;
    private Toggle _toggle;

    [SerializeField] private Image[] _cards;
    [SerializeField] private CardDefinition[] _cardSprites;

    private void Reset()
    {
        Volume = 1f;
        HighContrast = false;
    }

    private void Update()
    {
        for (int i = 0; i < _cards.Length; i++)
            _cards[i].sprite = _toggle.isOn ? _cardSprites[i].highContrast : _cardSprites[i].lowContrast;
    }

    public void LoadData(GameData data)
    {
        Volume = data.volume;
        HighContrast = data.highContrast;
        _slider.value = data.volume;

        SetNewBgColour(data.backgroundId);
        for (int i = 0; i < _dropdown.options.Count; i++)
        {
            if (_dropdown.options[i].text == data.backgroundId)
            {
                _dropdown.value = i;
                break;
            }
        }
        
        ActivePlayer.SetSettings(this);
    }

    public void SaveData(ref GameData data)
    {
        data.volume = _slider.value;
        data.backgroundId = _dropdown.options[_dropdown.value].text;
        data.highContrast = _toggle.isOn;
    }

    private void SetBgMaterial()
    {
        _bgMaterial.SetColor("_PrimaryColour", selectedColour.primary);
        _bgMaterial.SetColor("_SecondaryColour", selectedColour.secondary);
    }

    public void SetNewBgColour(BackgroundColours.BackgroundColour newColour)
    {
        selectedColour = newColour;
        SetBgMaterial();
    }

    public void SetNewBgColour(string newColour)
    {
        selectedColour = bgColours.Get(newColour);
        SetBgMaterial();
    }

    public void SetNewBgColour(int newColour)
    {
        selectedColour = bgColours.Get(newColour);
        SetBgMaterial();
    }

    public void ToggleCardContrast()
    {
        if (Box == null) return;

        Box.isHighContrastMode = HighContrast;
        for (int i = 0; i < Box.cardSet.cards.Count; i++)
        {
            Box.SetCardContrast(Box.cards[i].GetComponent<CardObject>(), Box.cardSet.cards[i]);
        }
        foreach (var joker in Box.jokerCards)
        {
            Box.SetJokerContrast(joker.GetComponent<CardObject>());
        }
    }
}
