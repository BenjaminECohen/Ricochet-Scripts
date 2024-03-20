using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaChange : MonoBehaviour
{
    [SerializeField] private Renderer theModel;

    private void Start()
    {

        Color color = theModel.material.color;
        color.a = 0.5f; 
        theModel.material.color = color;
    }  
}
