using UnityEngine;

public class FuelInjector : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float maxTypicalFlowRate;
    private float flowRate;
    [SerializeField] private FluidContainer fuelContainer;
    [SerializeField] private FluidContainer coolantContainer;
    [SerializeField] private EngineBell engineNozzle;

    [Space(7)]
    [SerializeField] private float maxTempTolerance;
    private float temp;

    private void Awake() {
        engineNozzle = GetNozzle();
    }

    public EngineBell GetNozzle() {
        return transform.parent.GetComponent<EngineBell>();
    }

    public float GetFuelLevel() {
        return fuelContainer.GetFluidAmount();
    }
    public float GetFuelMax() {
        return fuelContainer.GetFluidMax();
    }

    public FluidContainer GetFuelContainer() {
        return fuelContainer;
    }


    private void FixedUpdate() {

        if(OverHeated()) return;
        
        CalculateFlowRate();

        CombustFuel();

        CalculateHeat();

        CoolInjector();

    }

    private bool OverHeated() {
        return temp > maxTempTolerance;
    }

    private void CalculateFlowRate() {
        float fuelPercentage = GetFuelLevel() / GetFuelMax();

        if( fuelPercentage <= 0 ) flowRate = 0;
        else flowRate = maxTypicalFlowRate * Mathf.Pow(fuelPercentage, 2);
    }

    private void CombustFuel() {
        engineNozzle.ApplyThrust(flowRate);
        fuelContainer.SetFluidAmount(GetFuelLevel() - flowRate);
    }

    private void CalculateHeat() {

    }

    private void CoolInjector() {

    }
}
