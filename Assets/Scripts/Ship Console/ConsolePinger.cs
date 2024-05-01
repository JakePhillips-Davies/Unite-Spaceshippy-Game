using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsolePinger : MonoBehaviour
{
    [SerializeField] EngineInfoPanel enginePanel;
    [SerializeField] VelocityReadoutPanel velocityPanel;

    private void OnEnable() {
        enginePanel.PrintInfo();
        velocityPanel.gameObject.SetActive(true);
    }

    private void OnDisable() {
        enginePanel.DestroyPrinted();
        velocityPanel.gameObject.SetActive(false);
    }
}
