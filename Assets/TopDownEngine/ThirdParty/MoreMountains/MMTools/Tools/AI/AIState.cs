using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// A State is a combination of one or more actions, and one or more transitions. An example of a state could be "_patrolling until an enemy gets in range_".
    /// </summary>
    [System.Serializable]
	public class AIState 
    {
        /// the name of the state (will be used as a reference in Transitions
        public string StateName;
	    public AIAction[] Actions;
	    public AITransition[] Transitions;

        /// <summary>
        /// On enter state we pass that info to our actions and decisions
        /// </summary>
        public virtual void EnterState(AIBrain brain)
        {
            foreach (AIAction action in Actions)
            {
	            action.OnEnterState(brain);
            }
            foreach (AITransition transition in Transitions)
            {
	            if (transition.Decisions.Length != 0)
                {
                    transition.OnEnterState(brain);
                }
            }
        }

        /// <summary>
        /// On exit state we pass that info to our actions and decisions
        /// </summary>
        public virtual void ExitState(AIBrain brain)
        {
            foreach (AIAction action in Actions)
            {
                action.OnExitState(brain);
            }
            foreach (AITransition transition in Transitions)
            {
	            if (transition.Decisions.Length != 0)
                {
                    transition.OnExitState(brain);
                }
            }
        }

        /// <summary>
        /// Performs this state's actions
        /// </summary>
        public virtual void PerformActions(AIBrain brain)
        {
            if (Actions.Length == 0) { return; }

            foreach (var t in Actions)
            {
                if (t != null)
                {
                    t.PerformAction(brain);
                }
                else
                {
                    Debug.LogError("An action is null.");
                }
            }
        }

        /// <summary>
        /// Tests this state's transitions
        /// </summary>
        public virtual void EvaluateTransitions(AIBrain brain)
        {
            if (Transitions.Length == 0) { return; }
            for (var i = 0; i < Transitions.Length; i++) 
            {
	            if (Transitions[i].Decisions.Length != 0)
                {
	                if (Transitions[i].GetResult(brain))
                    {
                        if (!string.IsNullOrEmpty(Transitions[i].TrueState))
                        {
                            brain.TransitionToState(Transitions[i].TrueState);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Transitions[i].FalseState))
                        {
                            brain.TransitionToState(Transitions[i].FalseState);
                        }
                    }
                }                
            }
        }        
	}
}
