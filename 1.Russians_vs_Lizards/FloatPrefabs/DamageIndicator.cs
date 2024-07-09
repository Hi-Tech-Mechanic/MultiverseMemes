using UnityEngine;

public class DamageIndicate : MonoBehaviour
{
    private Vector2 _randomVector;

    void Start()
    {
        _randomVector = new Vector2(Random.Range(-350, 350), Random.Range(gameObject.transform.localPosition.y, 450));
    }

    void Update()
    {
        gameObject.transform.Translate(_randomVector * Time.deltaTime);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
