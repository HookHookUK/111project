using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : Component
{
    #region Singleton
    private static ObjectPool<T> instance;
    public static ObjectPool<T> Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPool<T>>();
                if (instance == null)
                {
                    instance = new GameObject($"ObjectPool").AddComponent<ObjectPool<T>>();
                }
            }
            return instance;
        }
    }
    #endregion


    [SerializeField] protected T _prefab;
    protected T obj;

    protected Dictionary<int, Queue<T>> pool = new Dictionary<int, Queue<T>>();
    protected int key;


    private void Awake()
    {
        if(obj != null)
        key = obj.gameObject.GetInstanceID();
    }

    public virtual void InstPool(Vector2 pos, GameObject parent = null)
    {
        if(pool.ContainsKey(key) && pool[key].Count > 0)
        {
            obj = pool[key].Dequeue();
            obj.gameObject.SetActive(true);
            obj.gameObject.transform.position = pos;
        }
        else
        {
            obj = Instantiate(_prefab, pos, Quaternion.identity);
            if (parent != null) obj.transform.SetParent(parent.transform);
            if(!pool.ContainsKey(key))
            {
                pool.Add(key, new Queue<T>());
            }

            pool[key].Enqueue(obj);
        }
    }

    public void DestroyPool(T obj)
    {
        //obj.transform.position = resetPos;
        obj.gameObject.SetActive(false);

        if (pool.ContainsKey(key))
        {
            pool[key].Enqueue(obj);
        }
        else
        { 
            pool.Add(key, new Queue<T>());
            pool[key].Enqueue(obj);
        }
    }
}
