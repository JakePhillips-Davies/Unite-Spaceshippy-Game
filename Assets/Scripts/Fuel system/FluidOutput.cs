using UnityEngine;

public class FluidOutput : MonoBehaviour
{
    [SerializeField] private FluidContainer outputsTo;
    [SerializeField] private FluidContainer outputsFrom;

    public void SetOutputsTo(FluidContainer to){
        outputsTo = to;
    }
    public void SetOutputsFrom(FluidContainer from){
        outputsFrom = from;
    }
    public FluidContainer GetOutputsTo(){
        return outputsTo;
    }
    public FluidContainer GetOutputsFrom(){
        return outputsFrom;
    }
}
