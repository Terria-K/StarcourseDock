using CutebaltCore;
using Nanoray.PluginManager;
using Nickel;
using Nickel.ModSettings;
using Shockah.CustomRunOptions;

namespace Teuria.StarcourseDock;

internal sealed partial class DoubleCardRunOption : 
    ICustomRunOptionsApi.ICustomRunOption, 
    IRegisterable
{
    public static ICustomRunOptionsApi CustomRunOptionsAPI { get; set; } = null!;
	public static readonly Lazy<List<Artifact>> Artifacts = new(() => [
        new DoubleCard()
    ]);

    public static void Register(IPluginPackage<IModManifest> package, IModHelper helper)
    {
        helper.ModRegistry.AwaitApi<ICustomRunOptionsApi>("Shockah.CustomRunOptions", x =>
        {
            CustomRunOptionsAPI = x;
            CustomRunOptionsAPI.RegisterCustomRunOption(new DoubleCardRunOption());
        });
    }

    public IReadOnlyList<ICustomRunOptionsApi.INewRunOptionsElement> GetNewRunOptionsElements(G g, RunConfig config)
    {
        return Artifacts.Value
            .Where(x => config.IsOptionSelected(x.Key()))
            .Select(CustomRunOptionsAPI.MakeArtifactNewRunOptionsElement)
            .ToList();
    }

    public IModSettingsApi.IModSetting MakeCustomRunSettings(NewRunOptions baseRoute, G g, RunConfig config)
    {
        var api = ModEntry.Instance.ModSettingsAPI;

        return api.MakeList([
            api.MakePadding(
                api.MakeText(
                    () => "<c=white>STARCOURSE DOCK</c>"
                ).SetFont(DB.thicket),
                8, 4
            ),
            api.MakeList([
                CustomRunOptionsAPI.MakeIconAffixModSetting(api.MakeCheckbox(
                    Artifacts.Value[0].GetLocName,
                    () => config.IsOptionSelected(Artifacts.Value[0].Key()),
                    (_, _, value) => config.SelectOption(Artifacts.Value[0].Key(), value)
                )).SetTooltips(Artifacts.Value[0].GetTooltips)
                .SetLeftIcon(CustomRunOptionsAPI.MakeIconAffixModSettingIconSetting().SetIcon(Sprites.artifacts_DoubleDeck.Sprite)),
            ])
        ]);
    }
}

internal sealed partial class PopulateRun : IPatchable
{
    [OnPostfix<State>(nameof(State.PopulateRun))]
    private static void State_PopulateRun_Postfix(State __instance)
    {
        if (!DoubleCardRunOption.CustomRunOptionsAPI.IsStartingNormalRun)
        {
            return;
        }

        foreach (var artifact in DoubleCardRunOption.Artifacts.Value)
        {
            if (__instance.runConfig.IsOptionSelected(artifact.Key()))
            {
                __instance.SendArtifactToChar(Mutil.DeepCopy(artifact));
            }
        }
    }   
}


file static class RunConfigExt
{
    extension(RunConfig config)
    {
        public bool IsOptionSelected(string key)
            => ModEntry.Instance.Helper.ModData.TryGetModData<List<string>>(config, "OptionsSelected", out var dailyModifierKeys) && dailyModifierKeys.Contains(key);

        public void SelectOption(string key, bool isSelected)
        {
            var dailyModifierKeys = ModEntry.Instance.Helper.ModData.ObtainModData<List<string>>(config, "OptionsSelected");
            dailyModifierKeys.Remove(key);

            if (!isSelected)
            {
                return;
            }
                
            dailyModifierKeys.Add(key);
        }
    }
}