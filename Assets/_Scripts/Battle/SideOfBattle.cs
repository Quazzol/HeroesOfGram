using System;
using System.Collections.Generic;
using System.Linq;

public abstract class SideOfBattle : ISideOfBattle
{
    public abstract event System.Action AttackStarted;
    public event System.Action AttackDone;

    public BattleSides Side => _side;

    protected BattleSides _side;
    protected HeroFactory _heroFactory;
    protected List<Hero> _heroesToBattle;
    protected Func<List<Hero>> _targets;

    public SideOfBattle(BattleSides side)
    {
        _side = side;
        _heroesToBattle = new List<Hero>();
    }

    public void SetHeroFactory(HeroFactory factory)
    {
        _heroFactory = factory;
    }

    public bool HasAliveHeroes()
    {
        return GetAliveHeroes().Count > 0;
    }

    public List<Hero> GetAliveHeroes()
    {
        return _heroesToBattle.Where(q => q.IsAlive).ToList();
    }

    public abstract void CreateBattleHeroes();
    public abstract void LoadBattleHeroes(List<HeroStatus> statuses);
    
    public int BattleHeroCount()
    {
        return _heroesToBattle.Count;
    }

    public Hero GetBattleHero(int index)
    {
        if (index < 0 || index >= _heroesToBattle.Count)
            return null;
        return _heroesToBattle[index];
    }

    public void SetAttackableTargets(Func<List<Hero>> targets)
    {
        _targets = targets;
    }

    public abstract void NotifyForAttack();
    public abstract void BattleEnded(bool isVictor);

    protected void AddBattleHero(Hero hero)
    {
        if (hero == null)
            throw new System.Exception("Hero cannot be null!");

        _heroesToBattle.Add(hero);
        hero.Attacked += OnHeroAttacked;
    }

    private void OnHeroAttacked()
    {
        AttackDone?.Invoke();
    }
}