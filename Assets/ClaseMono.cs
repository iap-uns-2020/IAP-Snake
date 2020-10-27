using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaseMono : MonoBehaviour
{
    private LocalStorage localStorage;

    // Start is called before the first frame update
    void Start()
    {
        localStorage = new LocalStorage();
        localStorage.Guardar("pepito","hola pepito soy un mensaje");
        Debug.Log(localStorage.Cargar("pepito"));

        localStorage.BorrarToro();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
