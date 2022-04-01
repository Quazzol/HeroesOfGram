using System;
using System.Collections.Generic;

[Serializable]
public class HeroInfo
{
    public string Name;
    public int Experience;
}

[Serializable]
public class HeroInfoList
{
    public List<HeroInfo> Heroes = new List<HeroInfo>();

    public int Count()
    {
        return Heroes.Count;
    }
}