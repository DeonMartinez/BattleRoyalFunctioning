using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo,
    Grenade
}


public class Pickup : MonoBehaviour
{
    public PickupType type;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            //get the player
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if (type == PickupType.Health)
                player.photonView.RPC("Heal", player.photonPlayer, value);
            if (type == PickupType.Ammo)
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
            //this is where you put in the GiveGrenade call


            // destroy the object
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
