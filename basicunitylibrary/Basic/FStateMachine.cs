using System.Collections;
using System.Diagnostics;

namespace Basic
{
    public abstract class StateBase<T>
    {
        FStateMachine<T> _fsm;
        public FStateMachine<T> FSM
        {
            get { return _fsm; }
            set { _fsm = value; }
        }

        public T Owner { get { return _fsm.Owner; } }

        public System.Action OnEnterCallback { get; set; }
        public System.Action OnEnterFinishCallback { get; set; }
        public System.Action OnExitCallback { get; set; }
        public System.Action OnExitFinishCallback { get; set; }

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();
    }

    /// <summary>
    /// Finite state machine
    /// </summary>
    /// <typeparam name="T">Type of owner</typeparam>
    public class FStateMachine<T>
    {
        T _owner;
        public T Owner { get { return _owner; } }

        // Set only
        StateBase<T> _statePrevious;
        public StateBase<T> StatePrevious
        {
            set
            {
                _statePrevious = value;

                if (value != null)
                    _statePrevious.FSM = this;
            }
        }

        // Set only
        StateBase<T> _stateCurrent;
        public StateBase<T> StateCurrent
        {
            set
            {
#if UNITY_EDITOR
                UnityEngine.Debug.Assert(value != null);
#else
                System.Diagnostics.Debug.Assert(value != null);
#endif

                _stateCurrent = value;
                _stateCurrent.FSM = this;
            }
        }

        // Set only
        StateBase<T> _stateGlobal;
        public StateBase<T> StateGlobal
        {
            set
            {
                _stateGlobal = value;

                if (value != null)
                    _stateGlobal.FSM = this;
            }
        }
        

        bool IsStateCurrent<SType>() where SType : StateBase<T>
        {
            var cast = _stateCurrent as SType;

            return cast != null;
        }

        public FStateMachine(T owner, StateBase<T> stateFirst, StateBase<T> stateGlobal = null)
        {
            _owner = owner;
            this.StatePrevious = null;
            this.StateGlobal = stateGlobal;

            this.ChangeState(stateFirst);
        }

        /// <summary>
        /// Please call from owners update
        /// </summary>
        public void OnUpdate()
        {
            if (_stateGlobal != null)
                _stateGlobal.OnUpdate();

            if (_stateCurrent != null)
                _stateCurrent.OnUpdate();
        }

        /// <summary>
        /// Change current state and call OnExit() and callbacks. Then enter next state and call OnEnter() and callbacks.
        /// </summary>
        /// <param name="stateNew">Next state</param>
        public void ChangeState(StateBase<T> stateNew)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(stateNew != null);
#else
            System.Diagnostics.Debug.Assert(stateNew != null);
#endif

            this.StatePrevious = _stateCurrent;

            /* Call OnExit on current state */

            if (_stateCurrent != null)
            {
                if (_stateCurrent.OnEnterCallback != null)
                    _stateCurrent.OnEnterCallback();

                _stateCurrent.OnExit();

                if (_stateCurrent.OnEnterFinishCallback != null)
                    _stateCurrent.OnEnterFinishCallback();
            }
            /**/

            this.StateCurrent = stateNew;
            _stateCurrent.FSM = this;

            /* Call OnEnter on next state */

            if (stateNew.OnEnterCallback != null)
                stateNew.OnEnterCallback();

            stateNew.OnEnter();

            if (stateNew.OnEnterFinishCallback != null)
                stateNew.OnEnterFinishCallback();
        }
    }
}
