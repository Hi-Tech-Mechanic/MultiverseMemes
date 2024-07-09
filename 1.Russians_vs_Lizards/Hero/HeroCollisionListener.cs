using UnityEngine;

public class HeroCollisionListener : DataStructure
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float critChance = Random.Range(0f, 1f);

        Battle.ProbabilityOfDifferentVersionsOfAttack(EnemiesSystem.enemy.Damage, (int)Battle.EntityType.Enemy);

        int rnd = Random.Range(0, 2);

        if (rnd == 0)
            AudioEffects.PlayEntitiesAudioEffects(EnemiesSystem.MakeDamageClip, "Play");
        else
            AudioEffects.PlayEntitiesAudioEffects(BattleHero.GetDamageClip, "Play");
    }
}