using MiniGame;
using MiniGame.Steal_Ball;
using UnityEngine;
using UnityEngine.Serialization;

namespace MoreMountains.Tools
{
    /// <summary>
    /// the AI brain is responsible from going from one state to the other based on the defined transitions. It's basically just a collection of states, and it's where you'll link all the actions, decisions, states and transitions together.
    /// </summary>
    public class AIBrain : MonoBehaviour
    {
        /// the collection of states
        private BrainStateData stateData;
        /// whether or not this brain is active
        [FormerlySerializedAs("BrainActive")] public bool brainActive = true;
        /// this brain's current state
        public AIState CurrentState { get; protected set; }
        /// the time we've spent in the current state
        public float TimeInThisState;
        /// the current target

        [Header("Frequencies")]
        /// the frequency (in seconds) at which to perform actions (lower values : higher frequency, high values : lower frequency but better performance)
        public float ActionsFrequency = 0.03f;
        /// the frequency (in seconds) at which to evaluate decisions
        public float DecisionFrequency = 0.03f;
        
        protected float _lastActionsUpdate = 0f;
        protected float _lastDecisionsUpdate = 0f;
	    protected AIState _initialState;
        
	    private PlayerController _playerController;
        public PlayerController PlayerController => _playerController;

        private PlayerStealBallController _playerStealBall;

        public PlayerStealBallController PlayerStealBall => _playerStealBall;

        /// <summary>
        /// On awake we set our brain for all states
        /// </summary>
        public virtual void Init(PlayerController playerController,BrainStateData stateData)
        {
            this.stateData = stateData;
            _playerController = playerController;
        }

        public virtual void InitSteal(PlayerStealBallController playerSteal, BrainStateData stateData)
        {
            this.stateData = stateData;
            _playerStealBall = playerSteal;
        }
        
        public virtual void ActiveBrain(string initState = null)
        {
            brainActive = true;
            if (initState != null && FindState(initState)!=null)
            {
                CurrentState = FindState(initState);
                CurrentState.EnterState(this);
            }
        }

        public void DeActiveBrain()
        {
            brainActive = false;
        }

        /// <summary>
        /// Every frame we update our current state
        /// </summary>
          void Update()
        {
            if (!brainActive || (CurrentState == null) || (Time.timeScale == 0f))
            {
                return;
            }

            if (Time.time - _lastActionsUpdate > ActionsFrequency)
            {
                CurrentState.PerformActions(this);
                _lastActionsUpdate = Time.time;
            }
            
            if (Time.time - _lastDecisionsUpdate > DecisionFrequency)
            {
                CurrentState.EvaluateTransitions(this);
                _lastDecisionsUpdate = Time.time;
            }
            
            TimeInThisState += Time.deltaTime;

        }
        
        /// <summary>
        /// Transitions to the specified state, trigger exit and enter states events
        /// </summary>
        /// <param name="newStateName"></param>
        public virtual void TransitionToState(string newStateName)
        {
            if (CurrentState == null)
            {
                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                {
                    CurrentState.EnterState(this);
                }
                return;
            }
            if (newStateName != CurrentState.StateName)
            {
                CurrentState.ExitState(this);
                OnExitState();

                CurrentState = FindState(newStateName);
                if (CurrentState != null)
                {
                    CurrentState.EnterState(this);
                }                
            }
        }
        
        /// <summary>
        /// When exiting a state we reset our time counter
        /// </summary>
        protected virtual void OnExitState()
        {
            TimeInThisState = 0f;
        }


        /// <summary>
        /// Returns a state based on the specified state name
        /// </summary>
        /// <param name="stateName"></param>
        /// <returns></returns>
        protected AIState FindState(string stateName)
        {
            foreach (AIState state in stateData.States)
            {
                if (state.StateName == stateName)
                {
                    return state;
                }
            }
            if (stateName != "")
            {
                Debug.LogError("You're trying to transition to state '" + stateName + "' in " + this.name + "'s AI Brain, but no state of this name exists. Make sure your states are named properly, and that your transitions states match existing states.");
            }            
            return null;
        }

        /// <summary>
        /// Resets the brain, forcing it to enter its first state
        /// </summary>
        public virtual void ResetBrain()
        {
	        //InitializeDecisions();
            // isOrderAccepted = false;
            // isOrderCanceled = false;
            // isReachedTarget = false;
            // isEndStayInRoomTime = false;
            // isHaveOrderInRoom = false;
            if (CurrentState != null)
            {
                CurrentState.ExitState(this);
                OnExitState();
            }
            
            if (stateData.States.Count > 0)
            {
                CurrentState = stateData.States[0];
                CurrentState?.EnterState(this);
            }  
        }
    }
}
