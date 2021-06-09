using Core.Entities;
using UnityEngine;

namespace Core
{
    public class ActionData
    {
        internal Entity entity;
        internal AIDestinationSetter aiDestinationSetter;
        internal Vector2 movementDirection;
        internal bool isGrounded;
        internal bool isDone;

        public ActionData(Entity entity, AIDestinationSetter aiDestinationSetter)
        {
            this.entity = entity;
            this.aiDestinationSetter = aiDestinationSetter;
        }
    }
}
