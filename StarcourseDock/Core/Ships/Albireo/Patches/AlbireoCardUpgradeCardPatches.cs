using System.Reflection;
using HarmonyLib;

namespace Teuria.StarcourseDock;

[HarmonyPatch]
internal sealed class AlbireoCardUpgradeCardPatches
{
    public static MethodBase TargetMethod()
    {
        MethodInfo? cardUpgrade_Render_info = null!;

        foreach (var method in typeof(CardUpgrade).GetMethods())
        {
            if (method.Name.Contains("<Render>"))
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 1)
                {
                    var p = parameters[0];
                    if (p.ParameterType == typeof(Upgrade))
                    {
                        cardUpgrade_Render_info = method;
                    }
                }
            }
        }

        return cardUpgrade_Render_info;
    }

    private static void Postfix(Upgrade upgradePath, in Card __result)
    {
        if (__result.TryGetLinkedCard(out Card? linkedCard))
        {
            bool validUpgrade = false;
            Card card = Mutil.DeepCopy(linkedCard);
            var meta = card.GetMeta().upgradesTo.AsSpan();
            for (int i = 0; i < meta.Length; i++)
            {
                if (meta[i] == upgradePath)
                {
                    validUpgrade = true;
                    break;
                }
            }

            if (validUpgrade)
            {
                card.upgrade = upgradePath;
            }

            __result.LinkedCard = card;
        }
    }
}
