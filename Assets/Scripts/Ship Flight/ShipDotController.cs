using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotController : MonoBehaviour
{
    //---             ---//
    private Vector2 screenCenter, dotDistance;
    private float rotation;
    public int lookSpeed;
    public GameObject ship;
    //---             ---//
    void FixedUpdate()
    {
        MoveDot();
        RotateShip();
    }
    
    private void Awake() {
        Input.mousePosition.Set(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
    }
    private void OnEnable() {
        Input.mousePosition.Set(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
    }

    void MoveDot()
    {
        screenCenter.y = Screen.height * 0.5f;
        screenCenter.x = Screen.width * 0.5f;

        dotDistance.y = (Input.mousePosition.y - screenCenter.y) / screenCenter.y;
        dotDistance.x = -(Input.mousePosition.x - screenCenter.x) / screenCenter.y;

        dotDistance = Vector2.ClampMagnitude(dotDistance, 1f);

        transform.localPosition = new Vector3(dotDistance.x*2f, 0f, dotDistance.y*2f);
    }

    void RotateShip()
    {
        rotation = Mathf.Lerp(rotation, Input.GetAxisRaw("Rotational"), 0.1f);
        ship.GetComponent<Rigidbody>().AddRelativeTorque(dotDistance.y * lookSpeed, -dotDistance.x * lookSpeed, rotation * lookSpeed);
    }
}
