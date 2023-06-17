using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_PlayerAttack : ObjectPool<PlayerAttack>
{


    [SerializeField] private SpriteRenderer playerSprite;
    private bool isFlip = false;
    public override void InstPool(Vector2 pos, GameObject target = null)
    {
        if (pool.ContainsKey(key) && pool[key].Count > 0)
        {
            obj = pool[key].Dequeue();
            obj.gameObject.SetActive(true);
            SetFlip();
            obj.gameObject.transform.position = pos;
        }
        else
        {
            obj = Instantiate(_prefab, pos, Quaternion.identity);
            SetFlip();
            if (target != null) obj.transform.SetParent(target.transform);
            if (!pool.ContainsKey(key))
            {
                pool.Add(key, new Queue<PlayerAttack>());
            }

            pool[key].Enqueue(obj);
        }
    }

    private void SetFlip()
    {
        if (isFlip)
        {
            obj.FlipX(true);
            playerSprite.flipX = true;
            isFlip = false;
        }
        else
        {
            obj.FlipX(false);
            playerSprite.flipX = false;
            isFlip = true;
        }
    }
}
