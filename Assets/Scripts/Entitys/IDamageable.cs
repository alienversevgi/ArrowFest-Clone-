using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable<T>
{
    T HP { get; }

    void Damage(T damage);
}