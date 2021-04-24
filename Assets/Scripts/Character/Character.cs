using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;
    
    [SerializeField]
    protected int hp_max, hp, attack = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize()
    {
        hp = hp_max;
    }

    public void MoveTo(Vector2 to)
    {
        SC.routines.StartCoroutine(MoveToStep(to), "char_movement_" + GetHashCode());
    }

    IEnumerator MoveToStep(Vector2 to)
    {
        while(Vector2.Distance(transform.position, to) > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * speed);
            yield return null;
        }
    }

    public void ReceiveAttack(AttackData ad)
    {
        hp -= ad.damage;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
