using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;
using System.Text.Json.Serialization;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace shaedyTracers;

public class shaedyTracersConfig : BasePluginConfig
{
    [JsonPropertyName("beam_color")]
    public string BeamColor { get; set; } = "0, 255, 0, 255"; // R, G, B, Alpha

    [JsonPropertyName("beam_width")]
    public float BeamWidth { get; set; } = 2.0f;

    [JsonPropertyName("beam_life")]
    public float BeamLife { get; set; } = 2.0f; // seconds
}

public class shaedyTracers : BasePlugin, IPluginConfig<shaedyTracersConfig>
{
    public override string ModuleName => "shaedy Tracers";
    public override string ModuleVersion => "1.0";
    public override string ModuleAuthor => "shaedy";

    public shaedyTracersConfig Config { get; set; } = new();

    public void OnConfigParsed(shaedyTracersConfig config)
    {
        Config = config;
    }

    public override void Load(bool hotReload)
    {
        Console.WriteLine("[shaedyTracers] Loaded.");
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var victim = @event.Userid;

        if (attacker == null || victim == null || !attacker.IsValid || !victim.IsValid)
            return HookResult.Continue;

        // Skip self-kills
        if (attacker == victim) return HookResult.Continue;

        var startPos = attacker.PlayerPawn.Value?.AbsOrigin;
        var eyeOffset = attacker.PlayerPawn.Value?.ViewOffset;
        if (startPos == null || eyeOffset == null) return HookResult.Continue;

        Vector from = new Vector(startPos.X, startPos.Y, startPos.Z + eyeOffset.Z);

        var vicPos = victim.PlayerPawn.Value?.AbsOrigin;
        if (vicPos == null) return HookResult.Continue;

        // +35 for roughly torso height
        Vector to = new Vector(vicPos.X, vicPos.Y, vicPos.Z + 35.0f);

        DrawBeam(from, to);

        return HookResult.Continue;
    }

    private void DrawBeam(Vector start, Vector end)
    {
        var colors = Config.BeamColor.Split(',');
        int r = 255, g = 255, b = 255, a = 255;
        if (colors.Length >= 3)
        {
            int.TryParse(colors[0].Trim(), out r);
            int.TryParse(colors[1].Trim(), out g);
            int.TryParse(colors[2].Trim(), out b);
            if (colors.Length > 3) int.TryParse(colors[3].Trim(), out a);
        }

        var beam = Utilities.CreateEntityByName<CBeam>("beam");
        if (beam == null) return;

        beam.Render = Color.FromArgb(a, r, g, b);
        beam.Width = Config.BeamWidth;

        beam.Teleport(start, new QAngle(0, 0, 0), new Vector(0, 0, 0));
        beam.EndPos.X = end.X;
        beam.EndPos.Y = end.Y;
        beam.EndPos.Z = end.Z;

        beam.DispatchSpawn();

        AddTimer(Config.BeamLife, () =>
        {
            if (beam != null && beam.IsValid)
                beam.Remove();
        });
    }
}