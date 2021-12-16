using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{
    [Header("Info")]
    public int id;


    [Header("Stats")]
    public float moveSpeed;
    public float sprintSpeed;
    public float currSpeed;
    public float jumpForce;
    public int curHP;
    public int maxHP;
    public int kills;
    public bool dead;
    //public bool sprinting;

    private bool flashingDamage;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonPlayer;
    public MeshRenderer mr;

    private int curAttackerId;
    public PlayerWeapon weapon;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.instance.players[id - 1] = this;

        //is this not our local player
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }

        else
        {
            GameUi.instance.Initialize(this);
        }
    }

    void Update()
    {
        if (!photonView.IsMine || dead)
            return;

        Move();
        //Sprint check
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currSpeed = moveSpeed + sprintSpeed;
            Debug.Log("I am sprinting");
        }
        else
        {
            currSpeed = moveSpeed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();

        if (Input.GetMouseButtonDown(0))
            weapon.TryShoot();

        if (Input.GetKeyDown(KeyCode.Q))
            weapon.TryStab();
    }

    void Move()
    {
        // grt axis input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
     
        

            //calculate a direction relative to where we are facing
            Vector3 dir = (transform.forward * z + transform.right * x) * currSpeed;
            dir.y = rig.velocity.y;
            //set as vel
            rig.velocity = dir;

        ////set as vel
        //rig.velocity = dir;
            
    }

    //void Sprint()
    //{
    //    if (Input.GetKeyDown(KeyCode.CapsLock))
    //        sprinting = true;

    //}

    void TryJump()
    {
        //make ray
        Ray ray = new Ray(transform.position, Vector3.down);

        //shoot ray
        if (Physics.Raycast(ray, 1.5f))
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (dead)
            return;

        curHP -= damage;
        curAttackerId = attackerId;

        //flash red
        photonView.RPC("DamageFlash", RpcTarget.Others);

        //update health bar
        GameUi.instance.UpdateHealthBar();

        //die if helth = 0
        if (curHP <= 0)
        {
            photonView.RPC("Die", RpcTarget.All);
        }
    }

    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;

        StartCoroutine(DamageFlashCoRoutine());

        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;
            Color defaultColor = mr.material.color;
            mr.material.color = Color.red;

            yield return new WaitForSeconds(0.05f);

            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
        curHP = 0;
        dead = true;

        GameManager.instance.alivePlayers--;

        //host checks whin condition
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();

        //is not local player
        if (photonView.IsMine)
        {
            if (curAttackerId != 0)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);

            //set cam to spectator 
            GetComponentInChildren<CameraController>().SetAsSpectator();

            //disable the physics and hide player
            rig.isKinematic = true;
            transform.position = new Vector3(0, -50, 0);

        }
    }

    [PunRPC]
    public void AddKill()
    {
        kills++;
        GameUi.instance.UpdatePlayerInfoText();
    }

    [PunRPC]
    public void Heal(int amountToHeal)
    {
        curHP = Mathf.Clamp(curHP + amountToHeal, 0, maxHP);

        // update the health bar UI
        GameUi.instance.UpdateHealthBar();
    }

}

