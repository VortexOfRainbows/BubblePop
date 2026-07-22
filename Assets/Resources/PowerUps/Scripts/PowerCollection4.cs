using UnityEngine;
public class Corrode : PowerUp
{
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {
        p.CorrodeDamage += 0.2f * Stack;
    }
}
public class Contaminate : PowerUp
{
    public override void Init() => Weighting = Common;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}
public class Combust : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}
public class Concoct : PowerUp
{
    public override void Init() => Weighting = Uncommon;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}
public class OilSpill : PowerUp
{
    public override void Init() =>  Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}
public class Gasoline : PowerUp
{
    public override void Init() => Weighting = Rare;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}
public class BlackDiamond : PowerUp
{
    public override void Init() => Weighting = SuperRare;
    public override void HeldEffect(Player p)
    {
        //p.ClusterBomb += Stack;
    }
}