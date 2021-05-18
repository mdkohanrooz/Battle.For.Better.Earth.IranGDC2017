using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    Vector3 Speed;
    bool Destroyed = false;
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(target != null)
        {
            transform.LookAt(target);
            transform.Translate(Speed * Time.fixedDeltaTime);
        }
	}

    // Launch the missile
    void LaunchMissile(Transform Target)
    {
        target = Target;
    }

    void OnCollisionEnter(Collision col)
    {
        if(Destroyed)
            return;
        Instantiate(ResourceCenter.instance.ExplosionPrefab, transform.position, Quaternion.identity);
        AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[4], Camera.main.transform.position);
        Destroyed = true;
        col.gameObject.SendMessage("ReceiveAttack", transform);
        Invoke("DestroyIt", 0.1f);
    }
    void DestroyIt()
    {
        Destroy(gameObject);
    }
}
