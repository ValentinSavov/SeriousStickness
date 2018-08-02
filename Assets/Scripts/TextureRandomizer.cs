using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRandomizer : MonoBehaviour
{
    public List<Sprite> sprites;
    public bool randomizeScale = true;
    public bool randomizeRotation = true;
    int spriteIndex;
    SpriteRenderer spriteRenderer;

	void Start()
    {
        spriteIndex = (int)Random.Range(0, sprites.Count);
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[spriteIndex];
        if(randomizeScale) transform.localScale *= Random.Range(0.6f, 1.1f);
        if(randomizeRotation) transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
