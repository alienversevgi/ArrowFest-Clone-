using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    interface IDamageable<T>
    {
        void Damage(T damage);
    }
}
