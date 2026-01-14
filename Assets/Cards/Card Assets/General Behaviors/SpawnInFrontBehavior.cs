using UnityEngine;
using Cards.Core.Behaviors;
using Cards.Core.BehaviorTags;
using Cards.Environments;
using Characters;
using Cards;
using Cards.Core;
using Cards.PhysicalProperties;

[CreateAssetMenu(menuName = "Behaviors/SpawnInFront")]
public class SpawnInFrontBehavior : Behavior, IBehaviorUseListener
{
    public PhysicalObject prefab;
    public float distance;
    public string abilityName, description;
    public bool Use(Card card, CardEnv env, Agent agent)
    {
        var p = Instantiate(prefab,  agent.transform.position + agent.transform.forward * distance, agent.transform.rotation);
        p.card = card;
        return true;
    }
    
    public override string GetDescription(Card card)
    {
        return $"<b>(Active) {abilityName}</b>: {description}.";
    }
}