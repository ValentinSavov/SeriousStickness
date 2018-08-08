using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlatformColliderSizeUpdate : MonoBehaviour
{
    void Update()
    {
        Debug.Log("Editor causes this Update");
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        collider.size = renderer.size;
    }
}
