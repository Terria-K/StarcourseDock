using System.Reflection;
using HarmonyLib;
using Riptide;
using Teuria.OnlineCoOp.TheClient;
using Teuria.OnlineCoOp.TheServer;

namespace Teuria.OnlineCoOp;

[HarmonyPatch]
public static class StatePatches
{
    public static State state = null!;

    [HarmonyPatch(typeof(State), nameof(State.Update))]
    [HarmonyPostfix]
    public static void State_Update_Postfix()
    {
        ClientManager.Instance.Update();
    }

    [HarmonyPatch(typeof(State), nameof(State.PopulateRun))]
    [HarmonyPrefix]
    public static void State_PopulateRun_Prefix(State __instance, IEnumerable<Deck> chars)
    {
        state = __instance;
        if (ServerManager.Instance.Server is null)
        {
            return;
        }

        var charsSet = (HashSet<Deck>)chars;

        var message = Message.Create(MessageSendMode.Reliable, PacketType.PlayerSelection);
        message.AddInt(charsSet.Count);
        foreach (var c in charsSet)
        {
            message.Add((int)c);
        }

        ClientManager.Instance.Send(message);
    }
}

[HarmonyPatch]
public static class StateHiddenPatches
{
    public static MethodBase TargetMethod()
    {
        return typeof(State).GetNestedTypes(AccessTools.all)
            .SelectMany(t => t.GetMethods(AccessTools.all))
            .First(m => m.Name.StartsWith("<PopulateRun>")
                && m.ReturnType == typeof(Route));
    }

    public static void Postfix()
    {
        if (ClientManager.Instance.Client is null)
        {
            return;
        }

        Deck? deck = null;
        var message = Message.Create(MessageSendMode.Reliable, PacketType.GetDeck);
        ClientManager.Instance.Send(message);

        while (deck is null)
        {
            deck = ClientMessages.deck;
            ClientManager.Instance.Update();
            Thread.Sleep(10);
        }

        StatePatches.state.deck.RemoveAll((c) =>
        {
            var cardDeck = c.GetMeta().deck;
            return cardDeck != deck.Value && cardDeck != Deck.colorless;
        });
    }
}