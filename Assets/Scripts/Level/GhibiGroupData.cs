using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    [System.Serializable]
    public class GhibiGroupData
    {
        public Vector3 Position;
        public List<RowChibiData> rowChibiDatas;
    }
}