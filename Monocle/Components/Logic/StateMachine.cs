using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monocle
{
    public class StateMachine : Component
    {
        private int state;
        private Action[] enterStates;
        private Func<int>[] updates;
        private Action[] leaveStates;
        private Func<IEnumerator>[] coroutines;
        private Coroutine currentCoroutine;

        public bool ChangedStates;
        public bool Log;
        public int PreviousState { get; private set; }

        public StateMachine(int initialState, int maxStates = 10)
            : base(true, false)
        {
            PreviousState = state = initialState;

            enterStates = new Action[maxStates];
            updates = new Func<int>[maxStates];
            leaveStates = new Action[maxStates];
            coroutines = new Func<IEnumerator>[maxStates];

            currentCoroutine = new Coroutine();
            currentCoroutine.RemoveOnComplete = false;
        }

        public StateMachine(int maxStates = 10)
            : this(0, maxStates)
        {

        }

        public int State
        {
            get { return state; }
            set
            {
#if DEBUG
                if (value >= updates.Length || value < 0)
                    throw new Exception("StateMachine state out of range");
#endif 
                if (state != value)
                {
                    if (Log)
                        Calc.Log("Enter State " + value + " (leaving " + state + ")");
                    ChangedStates = true;

                    if (leaveStates[state] != null)
                        leaveStates[state]();
                    PreviousState = state;
                    state = value;
                    if (enterStates[state] != null)
                        enterStates[state]();

                    if (coroutines[state] != null)
                    {
                        if (Log)
                            Calc.Log("Starting coroutine " + state);
                        currentCoroutine.Replace(coroutines[state]());
                    }
                    else
                        currentCoroutine.Cancel();
                }
            }
        }

        public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action onEnterState = null, Action onLeaveState = null)
        {
            updates[state] = onUpdate;
            enterStates[state] = onEnterState;
            leaveStates[state] = onLeaveState;
            coroutines[state] = coroutine;
        }

        public void ReflectState(Entity from, int index, string name)
        {
            updates[index] = (Func<int>)Calc.GetMethod<Func<int>>(from, name + "Update");
            enterStates[index] = (Action)Calc.GetMethod<Action>(from, name + "Enter");
            leaveStates[index] = (Action)Calc.GetMethod<Action>(from, name + "Leave");
            coroutines[index] = (Func<IEnumerator>)Calc.GetMethod<Func<IEnumerator>>(from, name + "Coroutine");
        }

        public override void Update()
        {
            ChangedStates = false;

            if (updates[state] != null)
                State = updates[state]();
            if (currentCoroutine.Active)
            {
                currentCoroutine.Update();
                if (!ChangedStates && Log && currentCoroutine.Finished)
                    Calc.Log("Coroutine " + state + " finished");
            }
        }

        static public implicit operator int(StateMachine s)
        {
            return s.state;
        }

        public void LogAllStates()
        {
            for (int i = 0; i < updates.Length; i++)
                LogState(i);
        }

        public void LogState(int index)
        {
            Calc.Log("State " + index + ": "
                + (updates[index] != null ? "U" : "")
                + (enterStates[index] != null ? "E" : "")
                + (leaveStates[index] != null ? "L" : "")
                + (coroutines[index] != null ? "C" : ""));
        }
    }
}
