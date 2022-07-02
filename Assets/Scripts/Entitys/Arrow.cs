using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnverPool;

namespace Game.Level
{
    public class Arrow : Entity
    {
        public override void Reset()
        {
            base.Reset();
            this.transform.eulerAngles = Vector3.zero;
        }
    }
}
