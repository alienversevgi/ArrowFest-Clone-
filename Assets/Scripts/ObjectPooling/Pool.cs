using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnverPool
{
    public class Pool<T> where T : Entity
    {
        #region Fields

        public List<T> Members = new List<T>();
        public List<T> Unavailable = new List<T>();
        private IFactory<T> _factory;

        #endregion

        #region MyRegion

        #endregion

        #region Public Methods

        public Pool(IFactory<T> factory, int size)
        {
            this._factory = factory;
            for (int i = 0; i < size; i++)
            {
                Create();
            }
        }

        public T Allocate()
        {
            for (int i = 0; i < Members.Count; i++)
            {
                if (!Unavailable.Contains(Members[i]))
                {
                    Unavailable.Add(Members[i]);
                    return Members[i];
                }
            }

            T newMembers = Create();
            Unavailable.Add(newMembers);
            return newMembers;
        }

        public void Release(T member)
        {
            member.Reset();
            _factory.ResetMember(member.gameObject);
            Unavailable.Remove(member);
        }

        public void ReleaseAll()
        {
            for (int i = 0; i < Unavailable.Count; i++)
            {
                Unavailable[i].Reset();
                _factory.ResetMember(Unavailable[i].gameObject);
            }

            Unavailable.Clear();
        }

        #endregion

        #region Private Methods

        private T Create()
        {
            T member = _factory.Create();
            Members.Add(member);
            return member;
        }

        #endregion
    }
}