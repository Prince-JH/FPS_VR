using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GL : Rifle
{
    [SerializeField]
    private float explosionRange;
    [SerializeField]
    private GameObject grenade;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = Instantiate(grenade);
            go.transform.position = new Vector3(447.5704f, 2.092758f, 533.568f);
        }
    }
}
