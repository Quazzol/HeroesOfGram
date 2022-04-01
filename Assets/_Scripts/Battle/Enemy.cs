using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : SideOfBattle
{
    public override event System.Action AttackStarted;

    private const int ENEMY_HERO_COUNT = 1;

    public Enemy(BattleSides side) : base(side)
    {
    }

    public override void CreateBattleHeroes()
    {
        for (int i = 0; i < ENEMY_HERO_COUNT; i++)
        {
            AddBattleHero(_heroFactory.CreateRandomHero(Side));
        }
    }

    public override void LoadBattleHeroes(List<HeroStatus> statuses)
    {
        foreach (var status in statuses)
        {
            var hero = _heroFactory.CreateHero(status.Name, Side);
            hero.LoadState(status);
            AddBattleHero(hero);
        }
    }

    public override void NotifyForAttack()
    {
        var aliveTargets = _targets();
        if (aliveTargets.Count == 0)
        {
            throw new Exception("No alive hero to target");
        }

        GetRandomHero()?.Attack(aliveTargets[UnityEngine.Random.Range(0, aliveTargets.Count)]);
        AttackStarted?.Invoke();
    }

    public override void BattleEnded(bool isVictor)
    {
        // do nothing
    }

    private Hero GetRandomHero()
    {
        var aliveHeroes = _heroesToBattle.Where(q => q.IsAlive).ToList();
        if (aliveHeroes.Count == 0)
        {
            Debug.Log("No alive hero to attack");
            return null;
        }

        return aliveHeroes[UnityEngine.Random.Range(0, aliveHeroes.Count)];
    }
}