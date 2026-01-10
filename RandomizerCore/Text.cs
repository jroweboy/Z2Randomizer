using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NLog;
using Z2Randomizer.RandomizerCore.Overworld;

namespace Z2Randomizer.RandomizerCore;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class Text : IEquatable<Text>
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    protected static readonly IEqualityComparer<byte[]> byteArrayEqualityComparer = new Util.StandardByteArrayEqualityComparer();

    public string RawText { get; private set; }
    public byte[] EncodedText { get; private set; }

    public Text()
    {
        RawText = "I know$nothing";
        EncodedText = Util.ToGameText("I know$nothing", true);
    }

    public Text(byte[] bytes)
    {
        RawText = Util.FromGameText(bytes);
        EncodedText = bytes;
    }

    public Text(string text)
    {
        RawText = text;
        EncodedText = Util.ToGameText(text, true);
    }

    public Text(string text, Collectable collectable)
    {
        RawText = text;
        if (RawText.Contains("%%"))
        {
            RawText = RawText.Replace("%%", collectable.EnglishText());
        }
        else if (RawText.Contains('%'))
        {
            RawText = RawText.Replace("%", collectable.SingleLineText());
        }
        else
        {
            logger.Warn("Invalid collectable in hint generation");
            RawText = "THIS TEXT$IS BROKEN$TELL$ELLENDAR";
        }
        EncodedText = Util.ToGameText(RawText, true);
    }

    public static Dictionary<Collectable, string> GenerateSpellMenuHints(IEnumerable<Location> allLocations, bool fireDash)
    {
        var spellMenu = new Dictionary<Collectable, string>();
        var allspells = Enum.GetValues<Collectable>()
            .Where(c => c.IsSpell() && (fireDash ? c != Collectable.FIRE_SPELL : c != Collectable.DASH_SPELL))
            .Order().ToImmutableList();
        var locs =  allLocations.ToImmutableList();
        // If this is called during room placement, just return dummy data since the locations aren't known yet.
        if (locs.Count == 0)
        {
            foreach (var spell in allspells)
            {
                spellMenu[spell] = "";
            }
            return spellMenu;
        }
        var sariaLocations = locs.First(i => i.ActualTown == Town.SARIA_NORTH).Children;
        var nabooruLocations = locs.First(i => i.ActualTown == Town.NABOORU).Children;
        var newKasutoLocations = locs.First(i => i.ActualTown == Town.NEW_KASUTO).Children;
        foreach (var spell in allspells)
        {
            Location? location = locs.FirstOrDefault(loc => loc.Collectables.Contains(spell));
            if (location == null)
            {
                // Could not find the location for the spell! This probably means they selected to start with this spell
                spellMenu[spell] = "";
                continue;
            }

            string? hint = null;
            if (location.PalaceNumber is >= 1 and <= 6)
            {
                var num = location.PalaceNumber!.Value;
                hint = $"PALACE {num}";
            }
            else
            {
                //For now bagu is not a town for hints. Could change in the future.
                if (location.ActualTown != null && location.ActualTown != Town.BAGU)
                {
                    hint = ((Town)location.ActualTown).HintName();
                } else if (location.ActualTown == Town.BAGU)
                {
                    hint = "BAGU";
                }
                else if (sariaLocations.Contains(location))
                {
                    hint = Town.SARIA_NORTH.HintName();
                } else if (nabooruLocations.Contains(location))
                {
                    hint = Town.NABOORU.HintName();
                } else if (newKasutoLocations.Contains(location))
                {
                    hint = Town.NEW_KASUTO.HintName();
                }
            }
            hint ??= location.Continent switch
            {
                Continent.EAST when location.Name.Contains("CAVE") => "EAST CAVE",
                Continent.EAST when !location.Name.Contains("CAVE") => "EAST ZONE",
                Continent.WEST when location.Name.Contains("CAVE") => "WEST CAVE",
                Continent.WEST when !location.Name.Contains("CAVE") => "WEST ZONE",
                Continent.DM => "DEATH MNT",
                Continent.MAZE => "MAZEISLAND",
                _ => throw new ImpossibleException("Unknown continent generating hints")
            };
            spellMenu.Add(location.Collectables[0], hint);
        }

        return spellMenu;
    }

    private static readonly Dictionary<int, string[]> palaceHints = new()
    {
        [1] = ["horsehead$neighs$with the$%%"],
        [2] = ["helmethead$guards the$%%"],
        [3] = ["rebonack$rides$with the$%%"],
        [4] = ["carock$disappears$with the$%%"],
        [5] = ["gooma sits$on the$%%"],
        [6] = ["barba$slithers$with the$%%"],
    };
    public static Text GenerateHelpfulHint(List<Location> allLocations, Location location, Collectable collectable, bool useTownSpecificHints)
    {
        string? hint = null;
        if (location.PalaceNumber is >= 1 and <= 6)
        {
            var num = location.PalaceNumber!.Value;
            hint = palaceHints[num].First();
        }
        else if (useTownSpecificHints)
        {
            //For now bagu is not a town for hints. Could change in the future.
            if (location.ActualTown != null && location.ActualTown != Town.BAGU)
            {
                hint = $"{((Town)location.ActualTown).HintName()}$has the$%%";
            }
            //Saria table
            if(allLocations.First(i => i.ActualTown == Town.SARIA_NORTH).Children.Contains(location))
            {
                hint = $"{Town.SARIA_NORTH.HintName()}$has the$%%";
            }
            //Nabooru fountain
            if (allLocations.First(i => i.ActualTown == Town.NABOORU).Children.Contains(location))
            {
                hint = $"{Town.NABOORU.HintName()}$has the$%%";
            }
            //Spell Tower / Granny's Basement
            if (allLocations.First(i => i.ActualTown == Town.NEW_KASUTO).Children.Contains(location))
            {
                hint = $"{Town.NEW_KASUTO.HintName()}$has the$%%";
            }
        }
        if(hint == null)
        {
            if (location.Continent == Continent.EAST)
            {
                hint = "go east to$find the$%%";
            }
            else if (location.Continent == Continent.WEST)
            {
                hint = "go west to$find the$%%";
            }
            else if (location.Continent == Continent.DM)
            {
                hint = "death$mountain$holds the$%%";
            }
            else if(location.Continent == Continent.MAZE)
            {
                hint = "in a maze$lies the$%%";
            }
            else
            {
                throw new ImpossibleException("Unknown continent generating hints");
            }
        }

        hint ??= "ERROR$GENERATING$HINT";
        return new Text(hint, collectable);
    }

    public string GetDebuggerDisplay()
    {
        return RawText;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Text);
    }

    public bool Equals(Text? other)
    {
        return other is not null && byteArrayEqualityComparer.Equals(EncodedText, other.EncodedText);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EncodedText);
    }
}
