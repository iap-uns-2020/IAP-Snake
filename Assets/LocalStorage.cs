using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalStorage 
{
    public void Guardar(string clave, string valor)
    {
        PlayerPrefs.SetString(clave,valor);
        PlayerPrefs.Save();
    }

    public string Cargar(string clave)
    {
        return PlayerPrefs.GetString(clave);
    }

    public void BorrarToro()
    {
        PlayerPrefs.DeleteAll();
    }
}
