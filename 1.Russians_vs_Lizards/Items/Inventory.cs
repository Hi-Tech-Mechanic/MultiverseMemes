using System.Collections;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject InventoryWindow;
    private bool _isOpen = false;

    public void OpenBackpack()
    {
        StartCoroutine(open(_isOpen));
        _isOpen = !_isOpen;

        IEnumerator open(bool state)
        {
            float timer = 0;
            float time = 0.5f;
            Vector2 open_pos = InventoryWindow.transform.localPosition;
            open_pos.y = -265;
            Vector2 close_pos = InventoryWindow.transform.localPosition;
            close_pos.y = -545;

            if (!state)
            {
                while (timer < time)
                {
                    timer += Time.deltaTime;
                    InventoryWindow.transform.localPosition = Vector2.Lerp(close_pos, open_pos, timer / time);
                    yield return null;
                }
                InventoryWindow.transform.localPosition = open_pos;
            }
            else
            {
                while (timer < time)
                {
                    timer += Time.deltaTime;
                    InventoryWindow.transform.localPosition = Vector2.Lerp(open_pos, close_pos, timer / time);
                    yield return null;
                }
                InventoryWindow.transform.localPosition = close_pos;
            }
        }
    }
}
