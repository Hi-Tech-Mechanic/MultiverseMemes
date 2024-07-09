using CombinedUniverse;
using UnityEngine;

public class AchievementsDropdown : Achievements
{
    public void DropdownInputMenu(int value)
    {
        switch (value)
        {
            case 0:
                SortingPerType();
                break;
            case 1:
                SortingPerProgress();
                break;
            case 2:
                SortingPerReward();
                break;
            default: break;
        }
    }
}
