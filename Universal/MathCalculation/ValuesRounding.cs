using System;

public class ValuesRounding
{
    [NonSerialized] private readonly static double[] _degree = { Math.Pow(10, 18), Math.Pow(10, 15), Math.Pow(10, 12), Math.Pow(10, 9), Math.Pow(10, 6), Math.Pow(10, 3), Math.Pow(10, 0) };
    [NonSerialized] private readonly static string[] _abbreviation = { "Quin", "Quad", "T", "B", "M", "K", "" };

    public static string FormattingValue(string prefix, string postfix, double value)
    {
        int tmpIndex = 0;

        for (int i = 0; i < _degree.Length; i++)
        {
            tmpIndex = i;

            if (value < (_degree[i] * 1000) && value >= (_degree[i] * 100))
            {
                value = Math.Round(value / _degree[i]);
                break;
            }
            else if (value < (_degree[i] * 100) && value >= (_degree[i] * 10))
            {
                value = Math.Round(value / _degree[i], 1);
                break;
            }
            else if (value < (_degree[i] * 10) && value >= _degree[i])
            {
                value = Math.Round((value / _degree[i]), 2);
                break;
            }
            else value = Math.Round(value, 2);
        }
        return $"{prefix}{value}{_abbreviation[tmpIndex]}{postfix}";
    }

    public static string ExtendedAccuracyFormattingValue(string prefix, string postfix, double value)
    {
        int tmpIndex = 0;

        for (int i = 0; i < _degree.Length; i++)
        {
            tmpIndex = i;

            if (value < (_degree[i] * 1000) && value >= (_degree[i] * 100))
            {
                value = Math.Round(value / _degree[i], 2);
                break;
            }
            else if (value < (_degree[i] * 100) && value >= (_degree[i] * 10))
            {
                value = Math.Round(value / _degree[i], 3);
                break;
            }
            else if (value < (_degree[i] * 10) && value >= _degree[i])
            {
                value = Math.Round((value / _degree[i]), 3);
                break;
            }
            else value = Math.Round(value);
        }
        return $"{prefix}{value}{_abbreviation[tmpIndex]}{postfix}";
    }

    public static string UltraAccuracyFormattingValue(string prefix, string postfix, double value)
    {
        int tmpIndex = 0;

        for (int i = 0; i < _degree.Length; i++)
        {
            tmpIndex = i;

            if (value < (_degree[i] * 1000) && value >= (_degree[i] * 100))
            {
                value = Math.Round(value / _degree[i], 2);
                break;
            }
            else if (value < (_degree[i] * 100) && value >= (_degree[i] * 10))
            {
                value = Math.Round(value / _degree[i], 3);
                break;
            }
            else if (value < (_degree[i] * 10) && value >= _degree[i])
            {
                value = Math.Round((value / _degree[i]), 3);
                break;
            }
            else value = Math.Round(value, 2);
        }
        return $"{prefix}{value}{_abbreviation[tmpIndex]}{postfix}";
    }

    public static string GetFormattedTime(float seconds)
    {
        if (((int)seconds % 60) < 10)
            return $"{(int)seconds / 60}:0{(int)seconds % 60}";
        else return $"{(int)seconds / 60}:{(int)seconds % 60}";
    }

    public static float GetMinutes(int seconds)
    {
        return seconds / 60f;
    }

    public static string GetFormattedLongTime(int seconds)
    {
        int hour = 0;
        int minute = 0;
        string hourMessage;
        string minuteMessage;
        string secondMessage;

        if (seconds >= 3600)
        {
            hour = seconds / 3600;
            seconds -= hour * 3600;
        }

        if (seconds >= 60)
        {
            minute = seconds / 60;
            seconds -= minute * 60;
        }

        if (hour < 10)
            hourMessage = $"0{hour}";
        else hourMessage = $"{hour}";

        if (minute < 10)
            minuteMessage = $"0{minute}";
        else minuteMessage = $"{minute}";

        if (seconds < 10)
            secondMessage = $"0{seconds}";
        else secondMessage = $"{seconds}";


        return $"{hourMessage}:{minuteMessage}:{secondMessage}";
    }
}