// ResourceCenter script starts from here
using UnityEngine;
using System.Collections;

public class ResourceCenter : MonoBehaviour
{
    public static ResourceCenter instance;

    public GameObject MissilePrefab;
    public GameObject ExplosionPrefab;
    public GameObject BoxDissapearParticle;
    public AudioClip[] audioClips;
    public GameObject OnePlusBlue;
    public GameObject OnePlusRed;
    public GameObject ItemInfoAutoCycle;
    public GameObject ItemInfoBackPack;
    public GameObject ItemInfoSpeed;
    public GameObject ItemInfoRocket;
    void Awake()
    {
        instance = this;
    }
}
// ResourceCenter script ends at here