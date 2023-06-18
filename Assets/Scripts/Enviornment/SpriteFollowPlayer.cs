using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpriteFollowPlayer : MonoBehaviour
{
    public bool followVertically;

    private Transform t_playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        t_playerTransform = FindObjectOfType<CharacterLook>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v3_playerPosition = t_playerTransform.position;
        if (!followVertically)
        {
            v3_playerPosition.y = transform.position.y;
        }
        transform.LookAt(v3_playerPosition);
    }
}
