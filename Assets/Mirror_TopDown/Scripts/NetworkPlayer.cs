using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;

namespace Mirror_Tanks
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [Header("Movement")]
        Rigidbody rb;
        [SerializeField] float movSpeed;
        [SerializeField] float rotationSpeed;
        [SerializeField] Transform cannonPivot;
        [SerializeField] Bullet bullet;
        [SerializeField] Transform cannonMuzzle;
        [SerializeField] GameObject ExplosiveRange;
        bool isDead = false;
        bool exploded = false;

        [Header("HUD")]
        [SerializeField] TextMeshProUGUI txt_PlayerName;
        [SerializeField] Image Health_Bar;
        [SyncVar(hook = nameof(HUDUpdateName))] public string pName;
        [SyncVar(hook = nameof(UpdateKillFeed))] string killer;
        [SerializeField] float maxHealth;
        [SyncVar(hook = nameof(HUDUpdateHealth))] public float currHealth;
        // Start is called before the first frame update

        public override void OnStartLocalPlayer()
        {
            
            base.OnStartLocalPlayer();
            CustomNetworkManager.singleton.AddPlayer(this);
            CmdSetName(CustomNetworkManager.singleton.PlayerName);

            ExplosiveRange.AddComponent<Explosive>().ownerId = netId;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            currHealth = maxHealth;

            CustomNetworkManager.singleton.AddPlayer(this);

            ExplosiveRange.AddComponent<Explosive>().ownerId = netId;

        }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            if( currHealth > 0)
            {
                isDead = false; exploded = false;
            }
            if (isLocalPlayer && !isDead)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");

                Vector3 mov = new Vector3(rb.position.x + h * movSpeed * Time.deltaTime, rb.position.y, rb.position.z + v * movSpeed * Time.deltaTime);
                rb.MovePosition(mov);

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    cannonPivot.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    cannonPivot.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                }
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    CmdShoot();
                }
                
            }
            if(isLocalPlayer && isDead && !exploded)
            {
                if(Input.GetKeyDown(KeyCode.G))
                {
                    CmdBomb();
                }
            }
        }
        [Command]

        public void CmdSetName(string name)
        {
            pName = name;
        }

        [Server]
        public void ApplyDamage(float damage, uint id)
        {
            currHealth -= damage;

           if(currHealth < 0)
            {
                foreach (NetworkPlayer nt in CustomNetworkManager.singleton.networkPlayers)
                {
                    if (nt.netId == id)
                    {
                        killer = nt.pName;
                    }
                }
            }

        }

        void HUDUpdateName(string oldName, string newName)
        {
            txt_PlayerName.text = newName;
            
        }

       
        void UpdateKillFeed(string oldName, string newName)
        {
            txt_PlayerName.text = txt_PlayerName.text + " was killed by " + newName;

        }
        void HUDUpdateHealth(float oldHealth, float newHealth)
        {
            Health_Bar.fillAmount = newHealth / maxHealth;
            if (currHealth <= 0)
            {
                isDead = true;
            }

           
        }
        [Command]
        void CmdShoot()
        {
            Bullet b = Instantiate(bullet, cannonMuzzle.position, cannonPivot.rotation) as Bullet;
            b.ownerId = netId;
            RpcShoot(cannonMuzzle.position, cannonPivot.rotation);
        }

        [Command]
        void CmdBomb()
        {
           ExplosiveRange.SetActive(true);
            exploded = true;
            RpcBomb();
        }
        [ClientRpc]
        void RpcShoot(Vector3 startPos, Quaternion rotation) 
        {
            if (isClientOnly)
            {
                Bullet b = Instantiate(bullet, startPos, rotation) as Bullet; 
            }
        }

        [ClientRpc]
        void RpcBomb()
        {
            if (isClientOnly)
            {
                ExplosiveRange.SetActive(true);
                exploded = true;
            }
        }

        [Server]

        public void Bomb(float damage)
        {
            currHealth -= damage;
        }
    }
}