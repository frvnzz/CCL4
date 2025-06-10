using UnityEngine;

public class PlayerMoveTest : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    private Vector3 targetPosition;
    public LayerMask groundLayer;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                targetPosition = hit.point;
                targetPosition.y = transform.position.y;
            }
        }

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }
}
