using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Config", order = 2)]
public class Config : ScriptableObject
{
    public ThemeSo[] themes;
    public int themeIndex;
}