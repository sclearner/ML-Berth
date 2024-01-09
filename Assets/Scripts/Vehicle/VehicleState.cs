using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleState : MonoBehaviour
{
    public bool isWaiting = false;
    public bool hasContainer = false;
    public bool isRunning = false;
    public bool isBreak = false;
    private float breakPrecision = 1e-10f;
    [SerializeField]
    private Sprite emptyState;
    [SerializeField]
    private Sprite containerState;
    [SerializeField]
    private Sprite breakState;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBreak) { spriteRenderer.sprite = breakState; return; }
        if (hasContainer) { spriteRenderer.sprite = containerState; }
        else
        {
            spriteRenderer.sprite = emptyState;
        }
    }

    private void FixedUpdate()
    {
        if (!isBreak) {
            //isBreak = UnityEngine.Random.Range(0, 1f) < breakPrecision;
            breakPrecision *= 1.0001f;
        }
        else
        {
            Debug.Log($"{name} feels bad!");
            breakPrecision = 1e-10f;
        }
    }
}
