// PlayerController starts from here
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Player1;
    public static PlayerController Player2;

    [SerializeField]
    ParticleSystem gunFire;
    [SerializeField]
    Transform Gun;
    [SerializeField]
    public Animator animator;
    [SerializeField]
    Slider slider_P1_Slots;
    [SerializeField]
    Slider slider_P2_Slots;
    [SerializeField]
    Vector2 PlayerSpeed;
    Rigidbody RB;
    int StoredTrashes = 0;
    int TrashSlots = 4;
    float attackTime = 0;
    bool isInVacum;
    Transform vacumTarget;
    bool attackReceived;
    bool SelfChargeEnabled;
    bool SpeedUpEnabled;
    bool AdditionSlotEnabled;
    GameObject PlayerMissile;
    [SerializeField]
    Image UISpeedUp_P1, UISpeedUp_P2;
    [SerializeField]
    Image UIAdditionalSlot_P1, UIAdditionalSlot_P2;
    [SerializeField]
    Image UISelfCharge_P1, UISelfCharge_P2;
    [SerializeField]
    Text UISlots_P1, UISlots_P2;

    // Awake
    void Awake()
    {
        if(tag == "P1")
            Player1 = this;
        else if(tag == "P2")
            Player2 = this;
    }
    // Use this for initialization
    void Start ()
    {
        RB = GetComponent<Rigidbody>();
        StartCoroutine(co_UIUpdater());
    }
	// Update is called once per frame
	void FixedUpdate ()
    {
        // if game finished
        if(GameController.instance.isFinished)
            return;
        // if not started
        if(!GameController.instance.isStarted)
            return;
        // if attack received
        if(attackReceived)
            return;
        // When in Vacumm
        animator.SetBool("Collect", isInVacum);
        if(isInVacum)
        {
            if(vacumTarget == null)
            {
                isInVacum = false;
                return;
            }
            Vector3 targetDir = vacumTarget.position - transform.position;
            targetDir = targetDir.normalized;
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg, 0.2f), 0);
            return;
        }

        // VACUMM
        float vacum = Input.GetAxis("Vacum_" + tag);
        if(vacum > 0)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 8);
            foreach(Collider c in cols)
                if(c.tag == "DeadTrash")
                {
                    if(c.GetComponent<Trash_AI>().SlotsNeededToCarry + StoredTrashes > TrashSlots)
                        continue;
                    vacumTarget = c.transform;
                    isInVacum = true;
                    vacumTarget.SendMessage("InVacum", transform, SendMessageOptions.RequireReceiver);
                    AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[2], Camera.main.transform.position);
                    break;
                }
            return;
        }

        // MISSILE
        float missile = Input.GetAxis("SecondaryAttack_" + tag);
        if(missile>0 && PlayerMissile != null)
        {
            AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[7], Camera.main.transform.position);
            PlayerMissile.transform.SetParent(null);
            PlayerMissile.SendMessage("LaunchMissile", tag == "P1" ? Player2.transform : Player1.transform);
            PlayerMissile = null;
        }

        // EJack
        float ejack = Input.GetAxis("EJack_" + tag);

        if(ejack > 0 && StoredTrashes > 0)
        {
            if(tag == "P1")
            {
                if(Vector3.Distance(BaseController.Base_P1.transform.position, transform.position) < 10)
                {
                    animator.SetTrigger("Drop");
                    BaseController.Base_P1.SendMessage("EmptyTrash", StoredTrashes);
                    StoredTrashes = 0;
                }
            }
            else if(tag == "P2")
            {
                if(Vector3.Distance(BaseController.Base_P2.transform.position, transform.position) < 10)
                {
                    animator.SetTrigger("Drop");
                    BaseController.Base_P2.SendMessage("EmptyTrash", StoredTrashes);
                    StoredTrashes = 0;
                }
            }
        }

        // ATTACK
        float attack = Input.GetAxis("Attack_" + tag);
        attackTime -= Time.fixedDeltaTime;
        if(attack > 0 && attackTime <= 0)
        {
            AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[1], Camera.main.transform.position);
            gunFire.Play();
            animator.SetTrigger("Attack");
            RaycastHit hitInfo;
            if(Physics.BoxCast(Gun.position,new Vector3(2,2,0.1f), Gun.forward, out hitInfo, Gun.rotation, 8))
                hitInfo.collider.SendMessage("ReceiveAttack", transform, SendMessageOptions.DontRequireReceiver);
            attackTime = 1;
        }

        // MOVE
        Vector3 Dir = new Vector3(Input.GetAxis("Horizontal_" + tag), 0, Input.GetAxis("Vertical_" + tag));
        Dir = Dir.normalized;
        float jump = Input.GetAxis("Jump_" + tag);
        float boost = Input.GetAxis("Boost_" + tag);
        if(Dir.x != 0 || Dir.z != 0)
        {
            if(boost > 0)
            {
                Dir *= 1.65f;
                if(!animator.IsInTransition(0))
                    animator.SetTrigger("Boost");
            }
            else
                if(!animator.IsInTransition(0))
                animator.SetTrigger("Move");
            RB.velocity = new Vector3(Dir.x * PlayerSpeed.x * Time.fixedDeltaTime, 0, Dir.z * Time.fixedDeltaTime * PlayerSpeed.y);
            transform.eulerAngles = new Vector3(0, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.Atan2(Dir.x, Dir.z) * Mathf.Rad2Deg, 0.2f), 0);
        }
        else
            animator.SetTrigger("Idle");
        
    }
    // When received attack by another player
    void ReceiveAttack(Transform attacker)
    {
        if(attackReceived)
            return;
        animator.SetBool("Damage", true);
        Vector3 dir = (transform.position - attacker.position).normalized;
        dir = new Vector3(dir.x, transform.position.y , dir.z);
        RB.velocity = dir * 50;
        attackReceived = true;
        if(StoredTrashes > 0)
            StoredTrashes -= 2;
        if(StoredTrashes < 0)
            StoredTrashes = 0;
        StartCoroutine(co_AttackRecovery());
    }
    // Receive Upgrade
    void ReceiveUpgrade(int index)
    {
        switch(index)
        {
            case 0:
                if(SelfChargeEnabled)
                    return;
                StartCoroutine(co_upg_SelfCharge());
                Instantiate(ResourceCenter.instance.ItemInfoAutoCycle, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                break;
            case 1:
                if(SpeedUpEnabled)
                    return;
                StartCoroutine(co_upg_SpeedUp());
                Instantiate(ResourceCenter.instance.ItemInfoSpeed, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                break;
            case 2:
                if(AdditionSlotEnabled)
                    return;
                StartCoroutine(co_upg_AddSlot());
                Instantiate(ResourceCenter.instance.ItemInfoBackPack, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                break;
            case 3:
                if(PlayerMissile == null)
                {
                    Instantiate(ResourceCenter.instance.ItemInfoRocket, transform.position + new Vector3(0, 3, 0), Quaternion.identity);
                    PlayerMissile = Instantiate(ResourceCenter.instance.MissilePrefab, transform.position, Quaternion.identity) as GameObject;
                    PlayerMissile.transform.SetParent(transform);
                    PlayerMissile.transform.localEulerAngles = Vector3.zero;
                    PlayerMissile.transform.localPosition = new Vector3(0, 2.5f, 0);
                }
                break;
        }
        AudioSource.PlayClipAtPoint(ResourceCenter.instance.audioClips[5], Camera.main.transform.position);
    }
    IEnumerator co_upg_SelfCharge()
    {
        SelfChargeEnabled = true;
        BaseController Base = tag == "P1" ? BaseController.Base_P1 : BaseController.Base_P2;
        for(int i=0;i<70;i++)
        {
            yield return new WaitForSeconds(0.1f);
            if(StoredTrashes > 0)
            {
                Base.SendMessage("EmptyTrash", StoredTrashes);
                StoredTrashes = 0;
            }
        }
        SelfChargeEnabled = false;
    }
    IEnumerator co_upg_SpeedUp()
    {
        SpeedUpEnabled = true;
        PlayerSpeed *= 1.5f;
        yield return new WaitForSeconds(10);
        PlayerSpeed /= 1.5f;
        SpeedUpEnabled = false;
    }
    IEnumerator co_upg_AddSlot()
    {
        AdditionSlotEnabled = true;
        TrashSlots += 2;
        yield return new WaitForSeconds(10);
        TrashSlots -= 2;
        if(StoredTrashes > TrashSlots)
            StoredTrashes = TrashSlots;
        AdditionSlotEnabled = false;
    }
    // Attack received (recovery)
    IEnumerator co_AttackRecovery()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Damage", false);
        attackReceived = false;
    }
    // UI Updater
    IEnumerator co_UIUpdater()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.2f);
            if(tag == "P1")
            {
                slider_P1_Slots.maxValue = TrashSlots;
                slider_P1_Slots.value = StoredTrashes;
                UISlots_P1.text = StoredTrashes + " / " + TrashSlots;
                // Add slot
                Color col = UIAdditionalSlot_P1.color;
                col.a = AdditionSlotEnabled ? 1 : 0.3f;
                UIAdditionalSlot_P1.color = col;
                // Speed up
                col = UISpeedUp_P1.color;
                col.a = SpeedUpEnabled ? 1 : 0.3f;
                UISpeedUp_P1.color = col;
                // Self Charge
                col = UISelfCharge_P1.color;
                col.a = SelfChargeEnabled ? 1 : 0.3f;
                UISelfCharge_P1.color = col;

            }
            else if(tag == "P2")
            {
                slider_P2_Slots.maxValue = TrashSlots;
                slider_P2_Slots.value = StoredTrashes;
                UISlots_P2.text = StoredTrashes + " / " + TrashSlots;
                // Add slot
                Color col = UIAdditionalSlot_P2.color;
                col.a = AdditionSlotEnabled ? 1 : 0.3f;
                UIAdditionalSlot_P2.color = col;
                // Speed up
                col = UISpeedUp_P2.color;
                col.a = SpeedUpEnabled ? 1 : 0.3f;
                UISpeedUp_P2.color = col;
                // Self Charge
                col = UISelfCharge_P2.color;
                col.a = SelfChargeEnabled ? 1 : 0.3f;
                UISelfCharge_P2.color = col;
            }
        }
    }
   
    void OnCollisionStay(Collision col)
    {
        // Vacumm finish
        if(col.collider.tag == "DeadTrash" && isInVacum)
        {
            Instantiate(tag == "P1" ? ResourceCenter.instance.OnePlusRed : ResourceCenter.instance.OnePlusBlue, transform.position + new Vector3(0,4,0), Quaternion.identity);
            StoredTrashes += vacumTarget.GetComponent<Trash_AI>().SlotsNeededToCarry;
            Destroy(vacumTarget.gameObject);
            isInVacum = false;
        }
    }
}
// PlayerController ends at here