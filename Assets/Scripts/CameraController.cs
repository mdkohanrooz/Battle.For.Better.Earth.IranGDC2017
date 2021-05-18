using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Transform lockTarget;
	
    // Lock at target
    void LockAtTarget(Transform target)
    {
        lockTarget = target;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(lockTarget == null)
        {
            transform.position = Vector3.Lerp(transform.position,
                (PlayerController.Player1.transform.position + PlayerController.Player2.transform.position) / 2 +
                new Vector3(0,
                35 + (Vector3.Distance(PlayerController.Player1.transform.position, PlayerController.Player2.transform.position) / 5),
                -22)
            , 0.1f);
            transform.eulerAngles = new Vector3(60 + Vector3.Distance(PlayerController.Player1.transform.position, PlayerController.Player2.transform.position) / 10, 0, 0);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,
                lockTarget.position +
                new Vector3(0,
                28,
                -17)
            , 0.05f);
            transform.eulerAngles = new Vector3(60, 0, 0);
        }
	}
}
