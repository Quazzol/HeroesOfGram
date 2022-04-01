using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroFactory : MonoBehaviour
{
    [SerializeField]
    private List<HeroStats> _definedHeroes;
    [SerializeField]
    private Hero _heroPrefab;

    public Hero CreateHero(HeroInfo info, BattleSides side)
    {
        CheckDefinedHeroes();

        var stat = _definedHeroes.FirstOrDefault(q => q.Name.Equals(info.Name));
        if (stat == null)
            throw new System.Exception("Cannot find specified hero!");
        
        return CreateHero(info, stat, side);
    }

    public HeroInfo GetRandomHeroInfoWithExclusion(List<HeroInfo> heroesToExlude)
    {
        CheckDefinedHeroes();
        
        if (heroesToExlude == null)
            heroesToExlude = new List<HeroInfo>();
            
        var includedHeroes = _definedHeroes.Where(q => !heroesToExlude.Select(p => p.Name).Contains(q.Name)).ToList();
        var stat = includedHeroes[Random.Range(0, includedHeroes.Count)];
        return new HeroInfo() { Name = stat.Name };
    }

    public Hero CreateHero(string heroName, BattleSides side)
    {
        CheckDefinedHeroes();
        var stat = _definedHeroes.First(q => q.Name.Equals(heroName));

        return CreateHero(new HeroInfo() { Name = stat.Name}, stat, side);
    }

    public Hero CreateRandomHero(BattleSides side)
    {
        CheckDefinedHeroes();
        var stat = _definedHeroes[Random.Range(0, _definedHeroes.Count)];
        
        return CreateHero(new HeroInfo() { Name = stat.Name}, stat, side);
    }

    private Hero CreateHero(HeroInfo info, HeroStats stat, BattleSides side)
    {
        var hero = Instantiate<Hero>(_heroPrefab, Vector3.zero, Quaternion.identity);
        hero.Initialize(info, stat, side);
        return hero;
    }

    private void CheckDefinedHeroes()
    {
        if (_definedHeroes == null || _definedHeroes.Count == 0)
            throw new System.Exception("No hero defined in factory!");
    }
}