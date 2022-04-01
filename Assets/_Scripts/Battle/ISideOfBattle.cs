using System;
using System.Collections.Generic;

public interface ISideOfBattle
{
    public event System.Action AttackStarted;
    public event System.Action AttackDone;

    public void SetHeroFactory(HeroFactory factory);
    public bool HasAliveHeroes();
    public List<Hero> GetAliveHeroes();
    public void CreateBattleHeroes();
    public void LoadBattleHeroes(List<HeroStatus> statuses);
    public int BattleHeroCount();
    public Hero GetBattleHero(int index);
    public void SetAttackableTargets(Func<List<Hero>> targets);
    public void NotifyForAttack();
    public void BattleEnded(bool isVictor);
}