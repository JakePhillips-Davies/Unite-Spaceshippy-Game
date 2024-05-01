using System;
using UnityEngine;
using UnityEngine.UI;

public class EngineInfo : MonoBehaviour
{
    [SerializeField] private FluidContainer fuelContainer;
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private EngineBell engineBell;
    [SerializeField] private Text thrustText;
    [SerializeField] private Text engineName;

    public void SetFuelContainer( FluidContainer cont ) {
        fuelContainer = cont;
    }
    public void SetEngineBell( EngineBell bell ) {
        engineBell = bell;
    }
    public void SetName( string name ) {
        engineName.text = name;
    }

    private void FixedUpdate() {
        
        thrustText.text = String.Format("{0:0.0}" + " N", engineBell.GetThrust());

        fuelSlider.value = fuelContainer.GetFluidAmount() / fuelContainer.GetFluidMax();

    }


}
