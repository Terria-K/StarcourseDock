using FSPRO;
using HarmonyLib;
using Newtonsoft.Json;
using Riptide;
using Teuria.OnlineCoOp.TheClient;
using Teuria.OnlineCoOp.TheServer;

namespace Teuria.OnlineCoOp;

[HarmonyPatch]
public static class NewRunOptionsPatches
{
	private static readonly UK CoOpOptionsUk = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();
	private static readonly UK CoOpServerOptionsUk = ModEntry.Instance.Helper.Utilities.ObtainEnumCase<UK>();

    [HarmonyPatch(typeof(NewRunOptions), nameof(NewRunOptions.StartRun))]
    [HarmonyPrefix]
    public static void NewRunOptions_StartRun_Prefix(G g, ref uint? seed)
    {

    }

    [HarmonyPatch(typeof(NewRunOptions), nameof(NewRunOptions.DifficultyOptions))]
    [HarmonyPrefix]
    public static void NewRunOptions_DifficultyOptions_Prefix(NewRunOptions __instance, G g, RunConfig runConfig)
    {
        SharedArt.ButtonText(
            g,
            new Vec(404, 88 + 95 + 31),
            CoOpOptionsUk,
            "CO-OP",
            onMouseDown: new MouseDownHandler(() =>
            {
                ClientManager.Instance.Connect("127.0.0.1:7778");

                while (ClientManager.Instance.Client!.IsConnecting)
                {
                    ClientManager.Instance.Update();
                    Thread.Sleep(10);
                }

                var message = Message.Create(MessageSendMode.Reliable, PacketType.GetSeed);
                ClientManager.Instance.Send(message);
                uint? seed = null;

                while (seed is null)
                {
                    seed = ClientMessages.seed;
                    ClientManager.Instance.Update();
                    Thread.Sleep(10);
                }

                __instance.StartRun(g, seed);
            })
        );

        SharedArt.ButtonText(
            g,
            new Vec(404, 88 + 95 + 31 + 31),
            CoOpServerOptionsUk,
            "CO-OP SERVER",
            onMouseDown: new MouseDownHandler(() =>
            {
                ServerManager.Instance.Create(7778);
                ServerManager.Instance.Start();

                ClientManager.Instance.Connect("127.0.0.1:7778");

                while (ClientManager.Instance.Client!.IsConnecting)
                {
                    ClientManager.Instance.Update();
                    Thread.Sleep(10);
                }

                var message = Message.Create(MessageSendMode.Reliable, PacketType.GetSeed);
                ClientManager.Instance.Send(message);
                uint? seed = null;

                while (seed is null)
                {
                    seed = ClientMessages.seed;
                    ClientManager.Instance.Update();
                    Thread.Sleep(10);
                }

                __instance.StartRun(g, seed);
            })
        );
    }
}

file class MouseDownHandler(Action act) : OnMouseDown
{
    public void OnMouseDown(G g, Box b)
    {
        Audio.Play(Event.Click);
        act();
    }
}
