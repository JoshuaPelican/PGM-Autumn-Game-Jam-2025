using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    static Dictionary<string, Transform> _objectPoolContainers;
    static Dictionary<GameObject, GameObject> _prefabMap;

    static Transform _poolsContainer;

    public const string GAMEOBJ_POOL_ID = "GameObject";
    public const string AUDIO_POOL_ID = "AudioSource";
    public const string PARTICLE_POOL_ID = "ParticleSystem";

    private void Awake()
    {

        _objectPools = new();
        _objectPoolContainers = new();
        _prefabMap = new();

        Setup();
    }

    void Setup()
    {
        _poolsContainer = new GameObject("Pools").transform;
    }

    static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, string poolID = GAMEOBJ_POOL_ID)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolID),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject
        );

        _objectPools.Add(prefab, pool);
    }

    static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, string poolID = GAMEOBJ_POOL_ID)
    {
        prefab.SetActive(false);
        GameObject obj = Instantiate(prefab, pos, rot);
        prefab.SetActive(true);

        obj.transform.SetParent(GetPoolParent(poolID));
        return obj;
    }

    static void OnGetObject(GameObject obj)
    {
        // Optional
    }

    static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    static void OnDestroyObject(GameObject obj)
    {
        if (_prefabMap.ContainsKey(obj))
            _prefabMap.Remove(obj);
    }

    static Transform GetPoolParent(string poolID)
    {
        if (poolID == "null" || poolID == "NULL")
            return null;

        else if (!_objectPoolContainers.ContainsKey(poolID))
        {
            _objectPoolContainers.Add(poolID, new GameObject($"[{poolID}] Object Pool").transform);
            _objectPoolContainers[poolID].SetParent(_poolsContainer);
        }

        return _objectPoolContainers[poolID];
    }

    static T SpawnObject<T>(GameObject prefab, Vector3 pos, Quaternion rot, string poolID = GAMEOBJ_POOL_ID) where T : Object
    {
        if (!_objectPools.ContainsKey(prefab))
        {
            CreatePool(prefab, pos, rot, poolID);
        }

        GameObject obj = _objectPools[prefab].Get();

        if (obj == null)
            return null;

        if (!_prefabMap.ContainsKey(obj))
        {
            _prefabMap.Add(obj, prefab);
        }

        obj.transform.SetPositionAndRotation(pos, rot);
        obj.SetActive(true);

        // If object is game object
        if(typeof(T) == typeof(GameObject))
        {
            return obj as T;
        }

        // If object is component
        if(!obj.TryGetComponent(out T component))
        {
            Debug.LogError($"Object {prefab.name} doesn't have component of type {typeof(T)}");
            return null;
        }
        return component;
    }

    public static T SpawnObject<T>(T prefab, Vector3 pos, Quaternion rot, string poolID = GAMEOBJ_POOL_ID) where T : Component
    {
        return SpawnObject<T>(prefab.gameObject, pos, rot, poolID);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 pos, Quaternion rot, string poolID = GAMEOBJ_POOL_ID)
    {
        return SpawnObject<GameObject>(prefab, pos, rot, poolID);
    }


    public static void ReturnObjectToPool(GameObject obj, string poolID = GAMEOBJ_POOL_ID)
    {
        if(!_prefabMap.TryGetValue(obj, out GameObject prefab))
        {
            Debug.LogWarning($"Trying to an object that is not pooled: {obj.name}");
            return;
        }

        Transform poolParent = GetPoolParent(poolID);
        if (obj.transform.parent != poolParent)
        {
            obj.transform.SetParent(poolParent);
        }

        if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
        {
            pool.Release(obj);
        }
    }
}
