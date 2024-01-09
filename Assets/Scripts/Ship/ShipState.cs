using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class ShipState : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Sprite noneSprite;

    [SerializeField]
    private Sprite fewSprite;

    [SerializeField]
    private Sprite halfSprite;

    [SerializeField]
    private Sprite fullSprite;

    public int containerCount = 3;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (containerCount < 0) containerCount = 0;
        switch (containerCount)
        {
            case 0:
                spriteRenderer.sprite = noneSprite;
                break;
            case 1:
                spriteRenderer.sprite = fewSprite;
                break;
            case 2:
                spriteRenderer.sprite = halfSprite;
                break;
            default:
                spriteRenderer.sprite = fullSprite;
                break;
        }
    }
}
