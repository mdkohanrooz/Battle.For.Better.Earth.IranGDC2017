// BaseController script starts from here

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaseController : MonoBehaviour
{
    public double Energy;
    float EnergyConsumption = 1;

    public static BaseController Base_P1;
    public static BaseController Base_P2;

    [SerializeField]
    Text TextP1Energy;
    [SerializeField]
    Text TextP2Energy;
    [SerializeField]
    Animator animator;

	// Use this for initialization
	void Start ()
    {
        if(tag == "P1")
            Base_P1 = this;
        else
            Base_P2 = this;
        StartCoroutine(co_UIUpdater());
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(GameController.instance.isStarted && !GameController.instance.isFinished)
        {
            Energy -= EnergyConsumption * Time.deltaTime;
            if(Energy < 0)
                Energy = 0;
        }
	}

    IEnumerator co_UIUpdater()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            if(tag == "P1")
                TextP1Energy.text = Energy.ToString("F0");
            else if(tag == "P2")
                TextP2Energy.text = Energy.ToString("F0");
        }
    }

    void EmptyTrash(int Trashes)
    {
        AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[3], Camera.main.transform.position);
        Energy += Trashes * 10;
        animator.CrossFade("Collect", 0.1f);
    }
}

// BaseController script ends here