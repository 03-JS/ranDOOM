using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    public Material[] materials;

    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {

    // }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Demon") && !other.name.Contains("Lost Soul"))
        {
            // Debug.Log("Material name: " + other.gameObject.GetComponentInChildren<SpriteRenderer>().material.name);
            if (other.gameObject.GetComponentInChildren<SpriteRenderer>().material.name == "Sprite Material (Instance)")
            {
                // Debug.Log("Swap material to non-diffuse");
                other.gameObject.GetComponentInChildren<SpriteRenderer>().material = materials[0];
            }
            else
            {
                // Debug.Log("Swap material to diffuse");
                other.gameObject.GetComponentInChildren<SpriteRenderer>().material = materials[1];
            }
        }
    }
}
