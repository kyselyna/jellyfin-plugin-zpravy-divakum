namespace Jellyfin.Plugin.ZpravyDivakum.Services;

/// <summary>Czech wording of a duration in the accusative ("za 1 minutu", "za 3 minuty", "za 30 sekund").</summary>
public static class CzechTime
{
    public static string Format(int seconds)
    {
        if (seconds >= 60 && seconds % 60 == 0)
        {
            var m = seconds / 60;
            return $"{m} {Plural(m, "minutu", "minuty", "minut")}";
        }

        if (seconds > 60)
        {
            var m = seconds / 60;
            var s = seconds % 60;
            return $"{m} {Plural(m, "minutu", "minuty", "minut")} a {s} {Plural(s, "sekundu", "sekundy", "sekund")}";
        }

        return $"{seconds} {Plural(seconds, "sekundu", "sekundy", "sekund")}";
    }

    private static string Plural(int n, string one, string few, string many) =>
        n == 1 ? one : n is >= 2 and <= 4 ? few : many;
}
