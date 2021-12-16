using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int curAmmo;
    public int maxAmmo;
    public float bulletSpeed;
    public float shootRate;
    public float stabRate;

    private float lastShootTime;
    private float lastStabTime;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    public Transform knifeHandPos;
    public GameObject knifePrefab;

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    public void TryShoot()
    {
        //can we shoot
        if (curAmmo <= 0 || Time.time - lastShootTime < shootRate)
            return;

        curAmmo--;
        lastShootTime = Time.time;

        // update ammo ui
        GameUi.instance.UpdateAmmoText();

        // spawn the bullet
        player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);
    }

    public void TryStab()
    {
        //can we stab
        if (Time.time - lastStabTime < stabRate)
            return;
        
        lastStabTime = Time.time;

        // spawn the knife
        player.photonView.RPC("SpawnKnife", RpcTarget.All, knifeHandPos.transform.position, Camera.main.transform.forward);
    }

    [PunRPC]
   void SpawnBullet (Vector3 pos, Vector3 dir)
    {
        //spawn & orient
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        //get bullet script
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();

        //initilize and set vel
        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript.rig.velocity = dir * bulletSpeed;
    }

    [PunRPC]
    void SpawnKnife(Vector3 pos, Vector3 dir)
    {
        //spawn & orient
        GameObject knifeObj = Instantiate(knifePrefab, pos, Quaternion.identity);
        knifeObj.transform.forward = dir;

        //get bullet script
        Knife knifeScript = knifeObj.GetComponent<Knife>();

        //initilize and set vel
        knifeScript.Initialize(damage, player.id, player.photonView.IsMine);
        knifeScript.rig.velocity = dir * bulletSpeed;
    }

    [PunRPC]
    public void GiveAmmo(int ammoToGive)
    {
        curAmmo = Mathf.Clamp(curAmmo + ammoToGive, 0, maxAmmo);
        // update the ammo text
        GameUi.instance.UpdateAmmoText();
    }
}
