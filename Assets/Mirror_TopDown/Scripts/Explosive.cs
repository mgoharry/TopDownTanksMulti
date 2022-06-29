using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{

    public uint ownerId;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(ownerId);
        if(other.CompareTag("Player") && Mirror_Tanks.CustomNetworkManager.singleton.IsServer )
        {
            if (other.GetComponent<Mirror_Tanks.NetworkPlayer>().netId == ownerId) return; 

            other.GetComponent<Mirror_Tanks.NetworkPlayer>().ApplyDamage(25, ownerId);
            Debug.Log("lol");
            Debug.Log($"Exp Name: {name}, parent: {transform.root.name}");
            Debug.Log($"other Name: {other.name}");


            if (other.GetComponent<Mirror_Tanks.NetworkPlayer>().currHealth <= 0)
            {
                transform.root.gameObject.GetComponent<Mirror_Tanks.NetworkPlayer>().currHealth = 100;
            }
        }
        StartCoroutine(Explode());
        
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
