using System.Collections;
using UnityEngine;
using System;
using static Skills;
using static ListOfEffects;
using Random = UnityEngine.Random;

public class SkillsMethods : DataStructure
{
    #region SkillsMechaniks
    public void BallClamp()
    {
        int skill_index = (int)SkillEnum.BallClamp;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._BallClamp.WillCost;
        float reload_time = Skills._BallClamp.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            if (EnemiesSystem.EnemyObject != null)
            {
                ListOfEffects.CreateDebuffEffect((int)DebuffEnum.Stun, Skills._BallClamp.StunDuration);
                EnemiesSystem.GetSkillDamage(Skills._BallClamp.Damage);
                EnemiesSystem.enemy.ReceivedStun = Skills._BallClamp.StunDuration;
            }
        }
    }

    public void ShootingGlance()
    {
        int skill_index = (int)SkillEnum.ShootingGlance;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._ShootingGlance.WillCost;
        float reload_time = Skills._ShootingGlance.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;
        float duration = Skills._ShootingGlance.Duration;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            if (EnemiesSystem.EnemyObject != null)
            {
                ListOfEffects.CreateDebuffEffect((int)DebuffEnum.PsiAttack, duration);
                EnemiesSystem.GetSkillDamage(Skills._ShootingGlance.Damage);
                StartCoroutine(Battle.PeriodicDamage(Skills._ShootingGlance.PeriodicDamage, duration));
            }
        }
    }

    public void BlackSauna()
    {
        int skill_index = (int)SkillEnum.BlackSauna;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._BlackSauna.WillCost;
        float reload_time = Skills._BlackSauna.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;
        float duration = Skills._BlackSauna.Duration;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            if (EnemiesSystem.EnemyObject != null)
            {
                ListOfEffects.CreateDebuffEffect((int)DebuffEnum.Burning, duration);
                ListOfEffects.CreateDebuffEffect((int)DebuffEnum.DecreaseHealthRegeneration, duration);
                StartCoroutine(Battle.PeriodicDamage(Skills._BlackSauna.PeriodicDamage, duration));
                StartCoroutine(DecreaseEnemyHealthRegeneration());
            }
        }

        IEnumerator DecreaseEnemyHealthRegeneration()
        {
            float timer = Skills._BlackSauna.Duration;
            float decrease = EnemiesSystem.enemy.HealthRegeneration
                * Skills._BlackSauna.DecreasedHealthRegeneration;

            EnemiesSystem.enemy.HealthRegeneration -= decrease;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            EnemiesSystem.enemy.HealthRegeneration += decrease;
        }
    }

    public void ArmorBreak()
    {
        int skill_index = (int)SkillEnum.ArmorBreak;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._ArmorBreak.WillCost;
        float reload_time = Skills._ArmorBreak.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            if (EnemiesSystem.EnemyObject != null)
            {
                ListOfEffects.CreateDebuffEffect((int)DebuffEnum.ArmorBreak, Skills._ArmorBreak.Duration);
                EnemiesSystem.GetSkillDamage(Skills._ArmorBreak.Damage);
                StartCoroutine(ArmorDebuf());
            }
        }

        IEnumerator ArmorDebuf()
        {
            float timer = Skills._ArmorBreak.Duration;
            float decrease = EnemiesSystem.enemy.Armor
                * Skills._ArmorBreak.DecreasedArmor;

            EnemiesSystem.enemy.Armor -= decrease;
            while (timer > 0 && EnemiesSystem.enemy.IsAlive == true)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            EnemiesSystem.enemy.Armor += decrease;
        }
    }

    public void ZeusAnger()
    {
        int skill_index = (int)SkillEnum.ZeusAnger;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._ZeusAnger.WillCost;
        float reload_time = Skills._ZeusAnger.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            StartCoroutine(CreateLightning());
        }

        IEnumerator CreateLightning()
        {
            int attack_count = Skills._ZeusAnger.AttackCount;

            while (attack_count > 0)
            {
                int a = Random.Range(0, Skills.LightningEffectsBuffer.Length);
                AudioEffects.PlayOneShotEffect(Skills.LightningEffectsBuffer[a]);
                if (EnemiesSystem.BossIsAlive)
                {
                    EnemiesSystem.GetPureDamage((EnemiesSystem.enemy.ActualHealth * Skills._ZeusAnger.PercentDamage) / 2);
                }
                else
                {
                    EnemiesSystem.GetPureDamage(EnemiesSystem.enemy.MaxHealth * Skills._ZeusAnger.PercentDamage);
                }
                attack_count--;
                yield return new WaitForSeconds(Skills._ZeusAnger.TimeBetweenAttacks);
            }
        }
    }

    public void HelpPerun()
    {
        int skill_index = (int)SkillEnum.HelpPerun;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._HelpPerun.WillCost;
        float reload_time = Skills._HelpPerun.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            AudioEffects.PlayOneShotEffect(Skills.PerunIntroduction);
            StartCoroutine(CreateLightning());
        }

        IEnumerator CreateLightning()
        {
            int attack_count = Skills._HelpPerun.AttackCount;

            while (attack_count > 0)
            {
                int a = Random.Range(0, Skills.LightningEffectsBuffer.Length);
                AudioEffects.PlayOneShotEffect(Skills.LightningEffectsBuffer[a]);
                if (EnemiesSystem.BossIsAlive)
                {
                    EnemiesSystem.GetPureDamage(EnemiesSystem.enemy.ActualHealth * Skills.HelpPerun.DamagePercentFromBossActualHealth);
                }
                else
                {
                    EnemiesSystem.GetPureDamage(EnemiesSystem.enemy.MaxHealth);
                }
                attack_count--;
                yield return new WaitForSeconds(1);
            }
        }
    }

    public void PrayerToTheGods()
    {
        int skill_index = (int)SkillEnum.PrayerToTheGods;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._PrayerToTheGods.WillCost;
        float reload_time = Skills._PrayerToTheGods.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;
        float duration = Skills._PrayerToTheGods.Duration;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            ListOfEffects.CreateBuffEffect((int)BuffEnum.Health, duration);
            ListOfEffects.CreateBuffEffect((int)BuffEnum.Damage, duration);
            ListOfEffects.CreateBuffEffect((int)BuffEnum.Armor, duration);
            StartCoroutine(AdditivePercentToStats());
        }

        IEnumerator AdditivePercentToStats()
        {
            float timer = duration;
            float additive_percent = Skills._PrayerToTheGods.PercentToStats;
            int heroIndex = Heroes.CurrentHero.Index;

            ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 0] = Heroes.CurrentHero.Damage * additive_percent;
            ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 1] = Heroes.CurrentHero.MaxHealth * additive_percent;
            ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 2] = Heroes.CurrentHero.Armor * additive_percent;
            Heroes.hero[heroIndex].DamageBase += ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 0];
            Heroes.hero[heroIndex].MaxHealthBase += ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 1];
            Heroes.hero[heroIndex].ArmorBase += ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 2];

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                ListOfEffects.CachedBuffTime_PrayerToTheGods[heroIndex] = timer;
                if (ListOfEffects.CachedBuffTime_PrayerToTheGods[heroIndex] < 0)
                    ListOfEffects.CachedBuffTime_PrayerToTheGods[heroIndex] = 0;
                yield return null;
            }

            Heroes.hero[heroIndex].DamageBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 0];
            Heroes.hero[heroIndex].MaxHealthBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 1];
            Heroes.hero[heroIndex].ArmorBase -= ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, 2];

            for (int i = 0; i < 3; i++)
            {
                ListOfEffects.CachedAdditiveStats_PrayerToTheGods[heroIndex, i] = 0;
            }
        }
    }

    public void GeneralFee()
    {
        int skill_index = (int)SkillEnum.GeneralFee;
        int place_index = GetPlaceSkillIndex(skill_index);
        float will_cost = Skills._GeneralFee.WillCost;
        float reload_time = Skills._GeneralFee.ReloadTime;
        float reload_amount = Skills.ReloadSkillImage[place_index].fillAmount;
        float duration = Skills._GeneralFee.Duration;

        if (CheckActualWill(reload_amount, will_cost, place_index))
        {
            SkillReload(reload_time, place_index);
            WillSubtraction(will_cost);

            AudioEffects.PlayOneShotEffect(Skills.SkillNamesEffects[skill_index]);
            ListOfEffects.CreateBuffEffect((int)BuffEnum.Attributes, duration);
            StartCoroutine(AdditivePercentToStats());
        }

        IEnumerator AdditivePercentToStats()
        {
            float timer = duration;
            float additive_percent = Skills._GeneralFee.PercentToStats;
            int heroIndex = Heroes.CurrentHero.Index;

            for (int i = 0; i < Heroes.HeroCount; i++)
            {
                if (Heroes.hero[i].Bought == true)
                {
                    ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 0] += Heroes.hero[i].Strength * additive_percent;
                    ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 1] += Heroes.hero[i].Dexterity * additive_percent;
                    ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 2] += Heroes.hero[i].Intellect * additive_percent;
                }
            }

            Heroes.hero[heroIndex].StrengthBase += ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 0];
            Heroes.hero[heroIndex].DexterityBase += ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 1];
            Heroes.hero[heroIndex].IntellectBase += ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 2];

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                ListOfEffects.CachedBuffTime_GeneralFee[heroIndex] = timer;
                if (ListOfEffects.CachedBuffTime_GeneralFee[heroIndex] < 0)
                    ListOfEffects.CachedBuffTime_GeneralFee[heroIndex] = 0;
                yield return null;
            }

            Heroes.hero[heroIndex].StrengthBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 0];
            Heroes.hero[heroIndex].DexterityBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 1];
            Heroes.hero[heroIndex].IntellectBase -= ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, 2];

            for (int i = 0; i < 3; i++)
            {
                ListOfEffects.CachedAdditiveStats_GeneralFee[heroIndex, i] = 0;
            }
        }
    }

    private bool CheckActualWill(float reload_amount, float will_cost, int place_index)
    {
        if (reload_amount <= 0 && Heroes.CurrentHero.ActualWill >= will_cost)
            return true;
        else BattleHero.SkillsAnimator[place_index].SetTrigger("Active");

        if (Heroes.CurrentHero.ActualWill < will_cost) BattleHero.WillSliderAnimator.SetTrigger("Active");

        return false;
    }

    public void SkillReload(float reloadTime, int placeIndex)
    {
        if (Skills.SavedReloadTimeForSelectedSkill[placeIndex] <= 0)
            Skills.SavedReloadTimeForSelectedSkill[placeIndex] = reloadTime;

        float timer = reloadTime - Skills.SavedReloadTimeForSelectedSkill[placeIndex];
        float fillAmount = 1 - (timer / reloadTime);

        Skills.ReloadSkillImage[placeIndex].gameObject.SetActive(true);
        Skills.ReloadValueText[placeIndex].gameObject.SetActive(true);
        StartCoroutine(ReloadCorotune());

        IEnumerator ReloadCorotune()
        {
            while (fillAmount > 0)
            {
                timer += Time.deltaTime;
                Skills.SavedReloadTimeForSelectedSkill[placeIndex] -= Time.deltaTime;
                fillAmount = 1 - (timer / reloadTime);
                Skills.ReloadSkillImage[placeIndex].fillAmount = fillAmount;
                UpdateSkillText();
                yield return null;
            }
            Skills.ReloadSkillImage[placeIndex].gameObject.SetActive(false);
            Skills.ReloadValueText[placeIndex].gameObject.SetActive(false);
        }

        void UpdateSkillText()
        {  Skills.ReloadValueText[placeIndex].text = $"{Math.Round((reloadTime - timer), 1)}ñ."; }
    }

    private void WillSubtraction(float cost)
    {  Heroes.CurrentHero.ActualWill -= cost; }

    public int GetPlaceSkillIndex(int skill_index)
    {
        for (int i = 0; i < PlasesForSkillsCount; i++)
        {
            if (Skills.SelectedSkillIndex[i] == skill_index)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion

    #region ButtonEvents
    public void GetPlaceIndex(int placeIndex)
    { Skills.ReturnedPlaseIndex = placeIndex; }

    public void SwitchSelectedSkill(int skillIndex)
    {
        Skills.SwitchSelectedSkill(skillIndex);
    }

    public void DeleteSkills()
    {
        Skills.DeleteSkills();
    }

    public void PurchaseSkill(int skill_index)
    {
        Skills.PurchaseSkill(skill_index);
    }

    public void PurchasePlaceForSkill(int place_index)
    {
        Skills.PurchasePlaceForSkill(place_index);
    }
    #endregion
}
