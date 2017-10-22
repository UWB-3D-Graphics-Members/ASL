using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASL.Manipulation.Objects.Vive
{
    public class MoveObject : ASL.Manipulation.Objects.MoveBehavior
    {
        public override void Awake()
        {
            base.Awake();
            MoveScale = 0.25f;
        }
    }
}