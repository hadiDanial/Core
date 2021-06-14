using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Entities
{
    public class JumpCurveMinMax : JumpMinMax
    {
        [SerializeField] private AnimationCurve jumpCurve;
        internal override float GetVerticalSpeed()
        {
            float b = finalMaxPos - initialHeight, c = currentHeight - initialHeight;
            return jumpSpeed * jumpCurve.Evaluate(Mathf.Abs(c / b));
        }
    }
}
