public class HeroesMethods : DataStructure
{
    #region EventsForButtons
    public void BuyHero(int Index)
    {
        Heroes.BuyHero(Index);
    }

    public void ChooseHero(int Index)
    {
        Heroes.ChooseHero(Index);
    }

    public void AddStrength(int heroIndex)
    {
        if (Heroes.hero[heroIndex].PumpingPoints > 0)
        {
            Heroes.hero[heroIndex].StrengthFromPumpingPoints += Heroes.AdditiveStatFromPumpingPoint;
            Heroes.hero[heroIndex].PumpingPoints--;
            CheckPumpingPoints(heroIndex);
        }
    }

    public void AddDexterity(int heroIndex)
    {
        if (Heroes.hero[heroIndex].PumpingPoints > 0)
        { 
            Heroes.hero[heroIndex].DexterityFromPumpingPoints += Heroes.AdditiveStatFromPumpingPoint;
            Heroes.hero[heroIndex].PumpingPoints--;
            CheckPumpingPoints(heroIndex);
        }
    }

    public void AddIntellect(int heroIndex)
    {
        if (Heroes.hero[heroIndex].PumpingPoints > 0)
        { 
            Heroes.hero[heroIndex].IntellectFromPumpingPoints += Heroes.AdditiveStatFromPumpingPoint;
            Heroes.hero[heroIndex].PumpingPoints--;
            CheckPumpingPoints(heroIndex);
        }
    }

    public void CheckPumpingPoints(int heroIndex)
    {
        if (Heroes.hero[heroIndex].PumpingPoints == 0)
        {
            Heroes.hero[heroIndex].PumpingPointsText.gameObject.SetActive(false);
            Heroes.hero[heroIndex].StrengthPumpingPointButton.gameObject.SetActive(false);
            Heroes.hero[heroIndex].DexterityPumpingPointButton.gameObject.SetActive(false);
            Heroes.hero[heroIndex].IntellectPumpingPointButton.gameObject.SetActive(false);
        }
        else
        {
            if (Heroes.hero[heroIndex].PumpingPointsText.gameObject.activeInHierarchy == false)
            {
                Heroes.hero[heroIndex].PumpingPointsText.gameObject.SetActive(true);
                Heroes.hero[heroIndex].StrengthPumpingPointButton.gameObject.SetActive(true);
                Heroes.hero[heroIndex].DexterityPumpingPointButton.gameObject.SetActive(true);
                Heroes.hero[heroIndex].IntellectPumpingPointButton.gameObject.SetActive(true);
            }
        }
    }
    #endregion
}