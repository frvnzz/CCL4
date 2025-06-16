using UnityEngine;

public class CamPosition : MonoBehaviour
{
    public Transform cameraPosition;

    // Update is called once per frame
    private void Update()
    {
        // This makes sure that camera follows the player's position
        transform.position = cameraPosition.position;
    }
}
