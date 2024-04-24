using UnityEngine;

public class EngineBell : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float specificImpulse;
    [SerializeField] private Rigidbody ship;
    [SerializeField] private float thrust;

    private void Awake() {
        FindShip();    
    }

    private void FindShip() {
        Transform searching = transform;
        while (searching.parent != null)
        {
            searching = searching.parent;

            if(searching.GetComponent<DragBox>() != null) {
                ship = searching.GetComponent<DragBox>().GetShipRB();
            }
            
        }   
    }

    public void ApplyThrust(float flowRate) {
        thrust = flowRate * specificImpulse;

        ship.AddForce(-transform.forward * thrust, ForceMode.Force);
    }
}
