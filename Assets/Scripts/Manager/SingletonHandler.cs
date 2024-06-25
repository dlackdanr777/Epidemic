using UnityEngine;

public class SingletonHandler<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                if (!obj)
                {
                    obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                else
                {
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    public virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
