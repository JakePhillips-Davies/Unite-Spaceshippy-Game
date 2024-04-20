using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapIsActive : MonoBehaviour
{
    public void SwapActiveStatus(){

        gameObject.SetActive( !gameObject.activeSelf );

    }
}
