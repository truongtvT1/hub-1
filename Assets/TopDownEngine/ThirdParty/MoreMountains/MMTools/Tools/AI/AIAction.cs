using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Actions are behaviours and describe what your character is doing. Examples include patrolling, shooting, jumping, etc. 
    /// </summary>
	public abstract class AIAction : ScriptableObject
    {
        public virtual void PerformAction(AIBrain _brain)
        {
        }

        /// <summary>
        /// Initializes the action. Meant to be overridden
        /// </summary>
        protected virtual void Initialization()
        {

        }

        protected virtual void OneTimeAction(AIBrain _brain)
        {
            
        }

        /// <summary>
        /// Describes what happens when the brain enters the state this action is in. Meant to be overridden.
        /// </summary>
	    public virtual void OnEnterState(AIBrain _brain)
        {
            OneTimeAction(_brain);
        }

        /// <summary>
        /// Describes what happens when the brain exits the state this action is in. Meant to be overridden.
        /// </summary>
        public virtual void OnExitState(AIBrain _brain)
        {
        }
    }
}