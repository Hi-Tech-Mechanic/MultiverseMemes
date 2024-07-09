using UnityEngine;

public class TriggerListener : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //AudioEffects.PlayButtonClickEffect();
        Debug.Log("ENTER");
    }

    //private void OnTriggerEnter2D()
    //{
    //    //AudioEffects.PlayButtonClickEffect();
    //    Debug.Log("ENTER");
    //}
}
