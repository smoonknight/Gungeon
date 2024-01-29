using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Gungeon/CharacterData", order = 0)]
public class CharacterData : ScriptableObject
{
    public new string name;
    public HealthAndMana maximumPoint;
    public HealthAndMana currentPoint;
    public SkillCharacter[] skillCharacters;

    public void AddHealth(int add) => SetHealth(currentPoint.health + add);
    public void SetHealth(int value) => currentPoint.health = Mathf.Clamp(value, 0, maximumPoint.health);

    public void ResetCurrentPoint() {
        currentPoint = maximumPoint;
    }


}

[System.Serializable]
public struct HealthAndMana
{
    public int health;
    public int mana;
}

public enum SkillCharacter
{
    quickDraw,
    stoneSmash,
}