using System.Collections.Generic;
using UnityEngine;

public class EngineInfoPanel : MonoBehaviour
{
    [SerializeField] private GameObject infoPrefab;
    [SerializeField] private GameObject shipInterior;
    [SerializeField] private List<FuelInjector> fuelInjectors;
    
    private void Start() {
        fuelInjectors = new List<FuelInjector>();
        DestroyPrinted();
    }
    public void PrintInfo() {
        shipInterior.GetComponentsInChildren<FuelInjector>(fuelInjectors);

        foreach (FuelInjector inj in fuelInjectors)
        {
            EngineInfo panel = Instantiate(infoPrefab, transform).GetComponent<EngineInfo>();

            panel.SetName(inj.GetNozzle().gameObject.name);

            panel.SetFuelContainer(inj.GetFuelContainer());

            panel.SetEngineBell(inj.GetNozzle());
        }
    }
    public void DestroyPrinted() {
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }
    }
}
