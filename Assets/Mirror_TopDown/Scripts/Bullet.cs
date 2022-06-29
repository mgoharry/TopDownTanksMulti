using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror_Tanks;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float speed, damage;
    [SerializeField] GameObject Player;
    Rigidbody rb;
    public uint ownerId;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.velocity = transform.forward * speed;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && Mirror_Tanks.CustomNetworkManager.singleton.IsServer)
        {
            other.GetComponent<Mirror_Tanks.NetworkPlayer>().ApplyDamage(damage, ownerId);
            
        }
        Destroy(gameObject);
    }
}
