using UnityEngine;

public class HandleToggle : MonoBehaviour
{
    public bool isActive = false;

    public void ToggleActive()
    {
        isActive = !isActive;
        GameManager.instance.LimitEnemySpawns = isActive;
    }
}
