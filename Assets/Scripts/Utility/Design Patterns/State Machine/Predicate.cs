using System;

namespace AstekUtility.StateMachine
{
    public interface IPredicate
    {
        bool Evaluate();
    }

    /// <summary>
    /// Evaluates given condition to identify if we can change state
    /// </summary>
    public class Predicate : IPredicate
    {
        public readonly Func<bool> Func;

        public Predicate(Func<bool> func)
        {
            Func = func;
        }

        public bool Evaluate()
        {
            return Func.Invoke();
        }
    }

    /// <summary>
    /// Evaluates given condition to identify if we can change state
    /// Can also take an input of generic type.
    /// </summary>
    /// <typeparam name="T">Input type for a Func</typeparam>
    public class Predicate<T> : IPredicate
    {
        public readonly Func<T, bool> Func;
        private T _data;

        public Predicate(Func<T, bool> func)
        {
            Func = func;
        }

        public bool Evaluate(T input)
        {
            _data = input;
            return Evaluate();
        }

        public bool Evaluate()
        {
            return Func.Invoke(_data);
        }
    }
}