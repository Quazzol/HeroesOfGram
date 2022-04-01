using UnityEngine;

[CreateAssetMenu(fileName = "New Hero", menuName = "Hero/Create Data")]
public class HeroStats : ScriptableObject
{
    public string Name = "Random Hero";
    public float BaseHealth = 300f;
    public float BaseAttackPower = 50f;
    public Color HeroColor = Color.white;
}
