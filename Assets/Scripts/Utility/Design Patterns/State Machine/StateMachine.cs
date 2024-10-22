using System;
using System.Collections.Generic;

namespace AstekUtility.StateMachine
{
    public class StateMachine
    {
        public StateNode Current { get; private set; }
        private Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();
        private HashSet<ITransition> _anyTransitions = new HashSet<ITransition>();

        public void Update()
        {
            ITransition transition = GetTransition();
            if (transition != null)
                ChangeState(transition.To);

            Current.State?.Update();
        }

        public void FixedUpdate()
        {
            Current.State?.FixedUpdate();
        }

        public void SetState(IState state)
        {
            Current = _nodes[state.GetType()];
            Current.State?.OnEnter();
        }

        public void ChangeState(IState state)
        {
            if (state == Current.State) return;

            IState previousState = Current.State;
            IState nextState = _nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState?.OnEnter();
            Current = _nodes[state.GetType()];
        }

        ITransition GetTransition()
        {
            foreach (var transition in _anyTransitions)
                if (transition.Condition.Evaluate())
                    return transition;

            foreach (var transition in Current.Transitions)
                if (transition.Condition.Evaluate())
                    return transition;

            return null;
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            _anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        public StateNode GetOrAddNode(IState state)
        {
            StateNode node = _nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                _nodes.Add(state.GetType(), node);
            }

            return node;
        }

        /// <summary>
        /// Node containing a state and its transitions 
        /// </summary>
        public class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}