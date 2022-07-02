using System.Collections.Generic;
using UnityEngine;

namespace EnverPool
{
    public class PrefabFactory<T> : IFactory<T> where T : MonoBehaviour
    {
        #region Fields

        private GameObject _root;
        private GameObject _prefab;
        private string _name;
        private int _index = 0;

        #endregion

        #region Public Methods

        public PrefabFactory(GameObject prefab) : this(prefab, prefab.name) { }

        public PrefabFactory(GameObject prefab, string name)
        {
            this._prefab = prefab;
            this._name = name;
            _root = new GameObject();
            _root.name = $"{name} Pool";
        }

        public T Create()
        {
            GameObject tempGameObject = GameObject.Instantiate(_prefab) as GameObject;
            tempGameObject.name = $"{_name}_{_index.ToString()}";
            tempGameObject.transform.SetParent(_root.transform);
            T objectOfType = tempGameObject.GetComponent<T>();
            _index++;
            return objectOfType;
        }

        public void ResetMember(GameObject pooledObject)
        {
            pooledObject.transform.SetParent(_root.transform);
        }

        #endregion
    }
}