using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreMountains.Tools
{
    /// <summary>
    /// Transitions are a combination of one or more decisions and destination states whether or not these transitions are true or false. An example of a transition could be "_if an enemy gets in range, transition to the Shooting state_".
    /// </summary>
    [System.Serializable]
    public class AITransition 
	{
		public enum TransitionType
		{
			AND,
			OR
		}
		public TransitionType transitionType;
        /// this transition's decision
		public AIDecision[] Decisions;
		public bool GetResult(AIBrain _brain)
		{
				switch(transitionType)
				{
				case TransitionType.AND:
					return  ANDResult(_brain);
				case TransitionType.OR:
					return OrResult(_brain);
				}
			return false;
		}
		private bool ANDResult(AIBrain _brain)
		{
			return Decisions.All(t => t.Decide(_brain));
		}
		private bool OrResult(AIBrain _brain)
		{
			return Decisions.Any(t => t.Decide(_brain));
		}
		public void OnEnterState(AIBrain _brain)
		{
			foreach (var t in Decisions)
			{
				t.OnEnterState(_brain);
			}
		}
		public void OnExitState(AIBrain _brain)
		{
			foreach (var t in Decisions)
			{
				t.OnExitState(_brain);
			}
		}
        /// the state to transition to if this Decision returns true
        public string TrueState;
        /// the state to transition to if this Decision returns false
        public string FalseState;
    }
}
