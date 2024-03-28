using UnityEngine;

public class DragBox : MonoBehaviour
{
    [SerializeField] Transform ship;

    private void FixedUpdate() {
        transform.SetPositionAndRotation(ship.transform.position, ship.transform.rotation);
    }
}
