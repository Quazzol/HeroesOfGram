using System;
using UnityEngine;

public class Hero : MonoBehaviour
{
#region "Events"
    public event Action Attacked;
    public event Action<float> DamageTaken;
    public event Action Defeated;
#endregion

#region "Properties"
    public BattleSides Side => _side;
    public HeroInfo Info => _info;
    public string Name => _stats.Name;
    public float Health => _health;
    public float AttackPower => _attackPower;
    public Color HeroColor => _stats.HeroColor;
    public int Experience => _info.Experience;
    public int Level => (_info.Experience / ExperienceNeededPerLevel) + 1;
    public bool IsAlive => _health > float.Epsilon;
#endregion

#region "Members"
    private BattleSides _side;
    private HeroInfo _info;
    private HeroStats _stats;
    private HeroUIController _uiController;
    private HeroAnimationController _animationController;
    private float _health;
    private float _maxHealth;
    private float _attackPower;
    private static float IncreasePercentPerLevel = 1.1f;
    private static int ExperienceNeededPerLevel = 5;
#endregion

#region "MonoBehaviour Methods"
    private void Awake()
    {
        _animationController = GetComponentInChildren<HeroAnimationController>();
        _uiController = GetComponentInChildren<HeroUIController>();
    }

    private void Start()
    {
        _animationController.AttackAnimationEnded += OnAttackAnimationEnded;
    }

    private void OnDisable()
    {
        _animationController.AttackAnimationEnded -= OnAttackAnimationEnded;
    }
#endregion

#region "Public Methods"
    public void Initialize(HeroInfo info, HeroStats stats, BattleSides side)
    {
        _stats = stats;
        _info = info;
        _side = side;
        
        GetComponentInChildren<MeshRenderer>().material.color = _stats.HeroColor;
        CalculateAttributes();
    }

    public void Attack(Hero targetHero)
    {
        targetHero.TakeDamage(_attackPower);
        _animationController.Attack(targetHero.transform.position);
    }

    public void TakeDamage(float damageTaken)
    {
        _health = (_health - damageTaken).ZeroIfNegative();
        _uiController.UpdateHealthBar(HealthPercent());
        _uiController.ShowDamageTaken(damageTaken);

        if (_health < float.Epsilon)
        {
            Defeated?.Invoke();
            _animationController.Dead();
            return;
        }

        DamageTaken?.Invoke(damageTaken);
        _animationController.Damaged();
    }

    public void LoadState(HeroStatus status)
    {
        _health = status.Health;
        _uiController.UpdateHealthBar(HealthPercent());

        if (_health < float.Epsilon)
        {
            _animationController.Dead();
            return;
        }
    }

    public void IncreaseXP(int gainedExperience)
    {
        _uiController.ShowXpGained();
        _info.Experience += gainedExperience;
        CalculateAttributes();
    }
#endregion

#region "Private Methods"
    private float HealthPercent()
    {
        if (_maxHealth < float.Epsilon)
        {
            Debug.Log("Divider cannot be less than or equal to zero!");
            return 0;
        }
        return _health / _maxHealth;
    }
    private void CalculateAttributes()
    {
        _maxHealth = _stats.BaseHealth * Mathf.Pow(IncreasePercentPerLevel, (Level - 1));
        _health = _maxHealth;
        _attackPower = _stats.BaseAttackPower * Mathf.Pow(IncreasePercentPerLevel, (Level - 1));
    }

    private void OnAttackAnimationEnded()
    {
        Attacked?.Invoke();
    }
#endregion
}
