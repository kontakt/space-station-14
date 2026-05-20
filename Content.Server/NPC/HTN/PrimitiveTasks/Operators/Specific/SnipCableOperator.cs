using Content.Server.Electrocution;
using Content.Server.Power.Components;

namespace Content.Server.NPC.HTN.PrimitiveTasks.Operators.Specific;

/// <summary>
/// Operator that snips a targeted cable and electrocutes the NPC.
/// </summary>
public sealed partial class SnipCableOperator : HTNOperator
{
    [Dependency] private IEntityManager _entityManager = default!;
    private ElectrocutionSystem _electrocution;

    /// <summary>
    /// Key that contains the target entity.
    /// </summary>
    [DataField]
    public string TargetKey = "Target";

    /// <summary>
    /// Initialize the operator, resolving ElectrocutionSystem.
    /// This is called by the HTN system during task management.
    /// </summary>
    /// <param name="sysManager">Simulation EntitySystemManager</param>
    public override void Initialize(IEntitySystemManager sysManager)
    {
        base.Initialize(sysManager);
        _electrocution = sysManager.GetEntitySystem<ElectrocutionSystem>();
    }

    /// <summary>
    /// Attempt to snip the wire.
    /// </summary>
    /// <param name="blackboard">Blackboard for the operator.</param>
    /// <param name="_">This operator does not care about timing.</param>
    /// <returns>Failed if unsuccessful. Finished otherwise.</returns>
    public override HTNOperatorStatus Update(NPCBlackboard blackboard, float _)
    {
        // Ensure a target is selected
        if (!blackboard.TryGetValue<EntityUid>(TargetKey, out var target, _entityManager))
            return HTNOperatorStatus.Failed;

        // Ensure the target is a cable
        if (!_entityManager.TryGetComponent<CableComponent>(target, out var cableComponent))
            return HTNOperatorStatus.Failed;

        var owner = blackboard.GetValue<EntityUid>(NPCBlackboard.Owner);

        // Zap the snipper, drop the cable
        _electrocution.TryDoElectrifiedAct(target, owner);
        _entityManager.SpawnNextToOrDrop(cableComponent.CableDroppedOnCutPrototype, target);
        _entityManager.QueueDeleteEntity(target);

        return HTNOperatorStatus.Finished;
    }
}
