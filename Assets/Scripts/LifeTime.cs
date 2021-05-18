// LifeTime script starts from here
using UnityEngine;
using System.Collections;

internal class LifeTime : MonoBehaviour
{
    public float _LifeTime;

    // Use this for initialization
    void Start()
    {
        Invoke("DestroyIt", _LifeTime);
    }

    // Destroy
    void DestroyIt()
    {
        Destroy(gameObject);
    }
}
// LifeTime script ends at here
