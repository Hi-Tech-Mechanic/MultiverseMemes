using System;
using System.Collections;
using UnityEngine;

public class EnemyAnimations : DataStructure
{
    #region Nonserialized
    [NonSerialized] private const float _closeEnemyPosition_x = -1100;
    [NonSerialized] private const float _createEnemyPosition_x = 1100;
    [NonSerialized] private const float _standardEnemyPosition_x = 0;
    [NonSerialized] private const float _timeToSwipe = 0.3f;
    #endregion

    public void DiscloseSwipeEnemy(string enemy_state)
    {
        if (enemy_state == "dead")
        { StartCoroutine(SwipeAndDestroy()); }
        else StartCoroutine(CreateAndSwipe()); 

        IEnumerator SwipeAndDestroy()
        {            
            float timer = 0;
            float current_x_positon = EnemiesSystem.EnemyObject.transform.localPosition.x;
            float current_y_positon = EnemiesSystem.EnemyObject.transform.localPosition.y;
            Vector2 base_position = new (current_x_positon, current_y_positon);
            Vector2 target_position = new (_closeEnemyPosition_x, current_y_positon);

            while (timer < _timeToSwipe)
            {
                EnemiesSystem.EnemyObject.transform.localPosition = Vector2.Lerp(base_position, target_position, timer / _timeToSwipe);
                timer += Time.deltaTime;
                yield return null;
            }
            EnemiesSystem.EnemyObject.transform.localPosition = target_position;
        }

        IEnumerator CreateAndSwipe()
        {
            float timer = 0;
            float current_y_positon = EnemiesSystem.EnemyObject.transform.localPosition.y;
            Vector2 base_position = new (_createEnemyPosition_x, current_y_positon);
            Vector2 target_position = new (_standardEnemyPosition_x, current_y_positon);

            while (timer < _timeToSwipe)
            {
                EnemiesSystem.EnemyObject.transform.localPosition = Vector2.Lerp(base_position, target_position, timer / _timeToSwipe);
                timer += Time.deltaTime;
                yield return null;
            }
            EnemiesSystem.EnemyObject.transform.localPosition = target_position;
        }
    }   
}