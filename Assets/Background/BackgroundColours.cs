using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BG Colour", menuName = "Background Colour Pallette")]
public class BackgroundColours : ScriptableObject
{
    [Serializable]
    public struct BackgroundColour
    {
        public string name;
        public Color primary;
        public Color secondary;
    }

    [SerializeField] private List<BackgroundColour> _colours = new();

    public BackgroundColour Get(string name)
    {
        foreach (var colour in _colours)
            if (colour.name == name)
                return colour;

        throw new NullReferenceException($"Could not find a colour with the name: {name}");
    }

    public string[] GetNameList()
    {
        return _colours.ConvertAll(c => c.name).ToArray();
    }

    public void Remove(string name)
    {
        _colours.RemoveAll(c => c.name == name);
    }

    public void RemoveAll()
    {
        while (_colours.Count > 0)
            _colours.Clear();
    }
}
