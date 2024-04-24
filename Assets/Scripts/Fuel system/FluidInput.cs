using UnityEngine;

public class FluidInput : MonoBehaviour
{
    [SerializeField] private FluidContainer inputsTo;

    public FluidContainer GetInputsTo(){
        return inputsTo;
    }
}
