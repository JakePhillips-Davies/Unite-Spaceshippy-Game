using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FluidContainer : MonoBehaviour
{
    public enum FluidType
    {
        fuel,
        coolant
    }
    [SerializeField] private float fluidAmount;
    [Header("Attributes")]
    [SerializeField] private float fluidMax;
    [SerializeField] private float flowRateBase;
    private float flowRate;
    [SerializeField] private FluidType type;
    [SerializeField] private List<FluidOutput> outputs;
    private int activeOutputs;


    public float GetFluidMax(){
        return fluidMax;
    }
    public float GetFluidAmount(){
        return fluidAmount;
    }
    public void SetFluidAmount(float setAmount){
        fluidAmount = setAmount;
    }

    public FluidType GetFluidType(){
        return type;
    }


    private void FixedUpdate() {

        if(fluidAmount < 0.5f) return;

        activeOutputs = 0;
        foreach (FluidOutput output in outputs)
        {
            if((ReferenceEquals(output.GetOutputsTo(), null) == false) && output.enabled) activeOutputs++;
        }

        if(activeOutputs == 0) return;

        flowRate = (fluidAmount / fluidMax) * (flowRateBase / activeOutputs);

        foreach (FluidOutput output in outputs)
        {
            if((ReferenceEquals(output.GetOutputsTo(), null) == false) && output.enabled) Flow(output.GetOutputsTo());
        }
    }

    private void Flow(FluidContainer output){

        float amountToFlow = fluidAmount / (activeOutputs + 1);
        if(amountToFlow > flowRate) amountToFlow = flowRate;

        if( (output.GetFluidAmount() + amountToFlow) >= output.GetFluidMax() ) {
            amountToFlow = output.GetFluidMax() - output.GetFluidAmount();
            output.SetFluidAmount(output.GetFluidMax());
        }
        else {
            output.SetFluidAmount(output.GetFluidAmount() + amountToFlow);
        }

        this.fluidAmount -= amountToFlow;

        activeOutputs--; 

    }

    
}
