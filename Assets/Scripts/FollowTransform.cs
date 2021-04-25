using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    Transform _to_follow;
    public Transform to_follow
    {
        get { return _to_follow; }
        set
        {
            _to_follow = value;
        }
    }
    [SerializeField]
    public bool auto_offset = true;
    public Vector3 offset = Vector3.zero;
    public update_type update_type = update_type.late;
    [SerializeField]
    public float x_multiplier = 1f, y_multiplier = 1f;
    public bool follow_z = false;
    Vector3 to_follow_pos_mod
    {
        get
        {
            if (to_follow == null)
            {
                return Vector2.zero;
            }
            Vector3 ret = to_follow.position;
            ret.x *= x_multiplier;
            ret.y *= y_multiplier;
            if (!follow_z)
            {
                ret.z = 0f;
            }
            return ret;
        }
    }
    bool initialized = false;

    public virtual void Awake()
    {

        Initialize();
    }

    public virtual void Initialize()
    {
        if (initialized)
        {
            return;
        }
        initialized = true;

        offset = auto_offset ? transform.position - to_follow_pos_mod : offset;
    }
    void Follow()
    {
        if (!initialized || to_follow == null)
        {

            return;
        }
        if (to_follow == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = offset + to_follow_pos_mod;
    }
    private void FixedUpdate()
    {
        if (update_type == update_type._fixed)
        {
            Follow();
        }
    }
    protected virtual void LateUpdate()
    {
        if (update_type == update_type.late)
        {
            Follow();
        }

    }

}

public enum update_type
{
    _fixed,
    late,
    normal
}