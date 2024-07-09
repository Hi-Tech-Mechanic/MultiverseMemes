public class GetEnemyDamage : DataStructure
{
    public void GetAttack()
    {
        Heroes.AttackEnemy();
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject.transform.GetChild(0).gameObject);
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }
}