using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "hearPlayer", story: "[Agent] heard the [player]", category: "Variable Conditions", id: "09935852a1096bfc0c3df0330f488d32")]
public partial class HearPlayerCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<Boolean> isPlayerHeard;


    public Enemy enemyScript;

    public override bool IsTrue()
    {

        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
