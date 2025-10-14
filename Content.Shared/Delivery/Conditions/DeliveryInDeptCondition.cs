using System.Linq;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityTable.Conditions;


public sealed partial class DeliveryInDeptCondition : EntityTableCondition
{

    [DataField]
    public string Department = "";

    protected override bool EvaluateImplementation(EntityTableSelector root,
        IEntityManager entMan,
        IPrototypeManager proto,
        EntityTableContext ctx)
    {

        var key = "departments";
        if (!ctx.TryGetData(key, out List<DepartmentPrototype>? departments))
        {
            return false;
        }

        if (departments.Any(item => item.ID == Department))
        {
            return true;
        }

        return false;

    }
}
