using UnityEngine;
using System.Collections;

public class OnePlusText : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    // Use this for initialization
	void Start ()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("DestroyIt", 2);
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(0, Time.deltaTime * 10, 0);
        transform.LookAt(Camera.main.transform);
        Color col = spriteRenderer.color;
        col.a -= Time.deltaTime / 3;
        spriteRenderer.color = col;
	}
    void DestroyIt()
    {
        Destroy(gameObject);
    }
}
