using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Material_Manager : MonoBehaviour
{
    public static Material_Manager materialManager;

    private void Awake()
    {
        if (materialManager == null)
        {
            materialManager = this;
        }
        else if (materialManager != this)
        {
            Destroy(materialManager.gameObject);
            materialManager = this;
        }
    }

    public List<string> woodenMaterials = new List<string>();
}
