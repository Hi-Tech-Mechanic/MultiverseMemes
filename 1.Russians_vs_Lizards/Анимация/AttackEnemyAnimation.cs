using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class AttackEnemyAnimation : DataStructure
{
    //private Vector3 random_vector;
    //private Vector3 saved_vector;
    //private const int min_shift = 5;
    //private const int max_shift = 25;
    //private const float shift_time = 0.05f;

    //private void Start()
    //{
    //    saved_vector = EnemiesSystem.EnemyObject.transform.localPosition;
    //}

    //public void EnemyShake()
    //{
    //    if (EnemiesSystem.EnemyObject != null)
    //    {
    //        saved_vector = EnemiesSystem.EnemyObject.transform.localPosition;

    //        random_vector = new Vector3 
    //            (Random.Range(min_shift + saved_vector.x, max_shift + saved_vector.x),
    //            Random.Range(min_shift + saved_vector.y, max_shift + saved_vector.y), 0);

    //        StartCoroutine(shift_position());
    //    }

    //    IEnumerator shift_position()
    //    {
    //        float timer = 0;

    //        while (timer < shift_time)
    //        {
    //            if (EnemiesSystem.EnemyObject != null) 
    //            { 
    //                EnemiesSystem.EnemyObject.transform.localPosition = 
    //                    Vector3.Lerp(saved_vector, random_vector, timer / shift_time);
    //            }
    //                timer += Time.deltaTime;
    //                yield return null;
    //        }
    //        if (EnemiesSystem.EnemyObject != null)
    //            EnemiesSystem.EnemyObject.transform.localPosition = random_vector;

    //        while (timer < shift_time)
    //        {
    //            if (EnemiesSystem.EnemyObject != null)
    //            { 
    //                EnemiesSystem.EnemyObject.transform.localPosition =
    //                Vector3.Lerp(random_vector, saved_vector, timer / shift_time);
    //            }
    //            timer += Time.deltaTime;
    //            yield return null;
    //        }
    //        if (EnemiesSystem.EnemyObject != null)
    //            EnemiesSystem.EnemyObject.transform.localPosition = saved_vector;
    //    }
    //}
}