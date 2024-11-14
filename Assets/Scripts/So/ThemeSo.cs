using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ThemeSo", order = 1)]
public class ThemeSo : ScriptableObject
{
    public Color themeBackGroundColor;
    public Color themeButtonColor;
    public Color themeArrowColor;
    public Color[] themeNumBgColors;
    public Color[] themeNumFontColors;
    public Color[] themeBgUnitColors;
}