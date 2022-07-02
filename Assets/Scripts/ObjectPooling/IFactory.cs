using UnityEngine;

namespace EnverPool
{
    public interface IFactory<T>
    {
        T Create();

        void ResetMember(GameObject pooledObject);
    }
}