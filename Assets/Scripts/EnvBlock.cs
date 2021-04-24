using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class EnvBlock : MonoBehaviour
{

    public int block_number { get; protected set; } = -1;
    public bool active = false;
    Dictionary<Collider2D, EnvBlock> triggered_colliders = new Dictionary<Collider2D, EnvBlock>();

    private void Update()
    {
        if (Mathf.Abs(GM.player.transform.position.x - transform.position.x) > SC.env.destroy_block_distance)
        {
            Destroy(gameObject);
        }

    }

    public void TriggerCollider(Collider2D c2d)
    {
        if (!active)
        {
            return;
        }
        if (triggered_colliders.ContainsKey(c2d) && triggered_colliders[c2d] != null)
        {
            return;
        }
        
        bool left = c2d.offset.x < 0f;

        if (triggered_colliders.ContainsKey(c2d))
        {
            triggered_colliders.Remove(c2d);
        }
        
        triggered_colliders.Add(c2d, SC.env.CreateAdjecentEnvBlock(this, left));
    }
    [SerializeField]
    int dispbn;
    
    public void Initialize(EnvBlock eb)
    {
        bool left = eb.transform.position.x < transform.position.x;
        triggered_colliders.Add(
            new List<Collider2D>(GetComponents<Collider2D>()).Find(c => c.offset.x < 0f == left),
            eb);

        block_number = eb.block_number + (left ? 1 : -1);
        dispbn = block_number;
        active = true;
    }

}
