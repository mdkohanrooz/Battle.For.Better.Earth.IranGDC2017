using UnityEngine;
using System.Collections;

public class UpgradeBoxController : MonoBehaviour
{
    float CaptureTime = 0;
    Transform CapturePlayer;
    [SerializeField]
    Transform Counter;

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(co_Updater());
	}
	
	// Update is called once per frame
	IEnumerator co_Updater ()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.1f);
            Counter.localEulerAngles = new Vector3(0, 0, CaptureTime * 35);
            Collider[] cols = Physics.OverlapSphere(transform.position, 6, 256);
            if(cols.Length == 0)
                CapturePlayer = null;
            else if(cols.Length == 1)
                CapturePlayer = cols[0].transform;
            else if(cols.Length == 2)
                CapturePlayer = null;
            if(CapturePlayer != null)
            {
                if(CapturePlayer.tag == "P1")
                    CaptureTime += 0.1f;
                else
                    CaptureTime -= 0.1f;

                if(CaptureTime > 2)
                {
                    CaptureTime = 2;
                    CapturePlayer.SendMessage("ReceiveUpgrade", Random.Range(0, 4));    // Upgrade index
                    StartCoroutine(co_Destroy());
                    yield break;
                }
                if(CaptureTime < -2)
                {
                    CaptureTime = -2;
                    CapturePlayer.SendMessage("ReceiveUpgrade", Random.Range(0, 4));    // Upgrade index
                    StartCoroutine(co_Destroy());
                    yield break;
                }
            }
            else
            {
                if(CaptureTime < -0.01f)
                    CaptureTime += 0.1f;
                if(CaptureTime > 0.01f)
                    CaptureTime -= 0.1f;
            }
        }
	}

    // Destroy 
    IEnumerator co_Destroy()
    {
        Instantiate(ResourceCenter.instance.BoxDissapearParticle, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
