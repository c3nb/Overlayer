using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vostok.Commons.Helpers.Observable
{
    /// <summary>
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>This class has a simple 4-state machine:</term>
    ///     </listheader>
    ///     <item>
    ///         <description>1 - Initial state. Value = null, error = null, completed = false</description>
    ///     </item>
    ///     <item>
    ///         <description>2 - State with value. Value = value, error = null, completed = false</description>
    ///     </item>
    ///     <item>
    ///         <description>3 - Completed. Value = any, error = null, completed = true</description>
    ///     </item>
    ///     <item>
    ///         <description>4 - Completed with error. Value = any, error = Err,  completed = true</description>
    ///     </item>
    /// </list>
    /// 
    /// <list type="bullet">
    ///     <listheader>
    ///         <term>Allowed transitions are:</term>
    ///     </listheader>
    ///     <item><description>1 - initial</description></item>
    ///     <item><description>1 -> 2 (obtaining the first value); 1 -> 3; 1 -> 4;</description></item>
    ///     <item><description>2 -> 2 (with another value); 2 -> 3; 2 -> 4;</description></item>
    ///     <item><description>3, 4 - terminal</description></item>
    /// </list>
    /// 
    /// All state changes occur under one lock. Weak reading without locks. Read safety is achieved by the atomic state changes and atomic state reads.
    /// 
    /// </summary>
    [PublicAPI]
    internal class CachingObservable<T> : IObservable<T>
    {
        private const int InitialState = 0;
        private const int HasValue = 1 << 0;
        private const int Completed = 1 << 1;

        /// <summary>
        /// This collection is readonly and always exists as a single object (tied to this class instance).
        /// So we can use it as "lock" object.
        /// </summary>
        private readonly Dictionary<IObserver<T>, Subscription> observers;

        [NotNull]
        private volatile State state;

        public CachingObservable()
            : this(new State(default, InitialState, null))
        {
        }

        public CachingObservable(T initialValue)
            : this(new State(initialValue, HasValue, null))
        {
        }

        private CachingObservable(State state)
        {
            observers = new Dictionary<IObserver<T>, Subscription>(ByReferenceEqualityComparer<IObserver<T>>.Instance);
            this.state = state;
        }

        public bool IsCompleted => state.IsCompleted();

        public T Get()
        {
            lock (observers)
            {
                return GetWeak();
            }
        }

        /// <summary>
        /// LockFree implementation of the <see cref="Get"/> method. May return a new value slightly before observers receive <see cref="IObserver{T}.OnNext"/> event.
        /// </summary>
        public T GetWeak()
        {
            var cachedState = state;

            if (!cachedState.WithValue())
                throw new InvalidOperationException("Observable has no value.");
            if (cachedState.SavedError != null)
                throw cachedState.SavedError;

            return cachedState.Value;
        }

        public T GetOrDefault()
        {
            lock (observers)
            {
                return GetOrDefaultWeak();
            }
        }

        /// <summary>
        /// LockFree implementation of the <see cref="GetOrDefault"/> method. May return a new value slightly before observers receive <see cref="IObserver{T}.OnNext"/> event.
        /// </summary>
        public T GetOrDefaultWeak()
        {
            var cachedState = state;

            if (!cachedState.WithValue() || cachedState.SavedError != null)
                return default;

            return cachedState.Value;
        }

        public void Next([CanBeNull] T nextValue)
        {
            lock (observers)
            {
                var cachedState = state;
                if (cachedState.IsCompleted())
                    return;

                var cachedObservers = observers.Keys.ToArray();

                state = new State(nextValue, cachedState.Flags | HasValue, cachedState.SavedError);

                foreach (var observer in cachedObservers)
                    try
                    {
                        observer.OnNext(nextValue);
                    }
                    catch
                    {
                        // ignored
                    }
            }
        }

        public void Error([NotNull] Exception error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            lock (observers)
            {
                var cachedState = state;
                var cachedObservers = observers.Keys.ToArray();

                if (cachedState.IsCompleted())
                    return;

                state = new State(cachedState.Value, cachedState.Flags | Completed, error);

                foreach (var observer in cachedObservers)
                    try
                    {
                        observer.OnError(error);
                    }
                    catch
                    {
                        // ignored
                    }

                observers.Clear();
            }
        }

        public void Complete()
        {
            lock (observers)
            {
                var cachedState = state;
                var cachedObservers = observers.Keys.ToArray();

                if (cachedState.IsCompleted())
                    return;

                state = new State(cachedState.Value, cachedState.Flags | Completed, cachedState.SavedError);

                foreach (var observer in cachedObservers)
                    try
                    {
                        observer.OnCompleted();
                    }
                    catch
                    {
                        // ignored
                    }

                observers.Clear();
            }
        }

        public void Complete([CanBeNull] T lastValue)
        {
            lock (observers)
            {
                Next(lastValue);
                Complete();
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (observers)
            {
                if (observers.TryGetValue(observer, out var existing))
                    return existing;

                var cachedState = state;

                if (cachedState.SavedError != null)
                {
                    observer.OnError(cachedState.SavedError);
                    return new EmptyDisposable();
                }

                if (cachedState.WithValue())
                    observer.OnNext(cachedState.Value);

                if (cachedState.IsCompleted())
                {
                    observer.OnCompleted();
                    return new EmptyDisposable();
                }

                var subscription = new Subscription(this, observer);
                observers.Add(observer, subscription);
                return subscription;
            }
        }

        private sealed class State
        {
            public readonly T Value;
            public readonly int Flags;
            public readonly Exception SavedError;

            public State(T value, int flags, Exception savedError)
            {
                Value = value;
                Flags = flags;
                SavedError = savedError;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool WithValue() => (Flags & HasValue) != 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsCompleted() => (Flags & Completed) != 0;
        }

        #region Subscription

        private sealed class Subscription : IDisposable
        {
            private readonly CachingObservable<T> observable;
            private readonly IObserver<T> observer;

            public Subscription(CachingObservable<T> observable, IObserver<T> observer)
            {
                this.observable = observable;
                this.observer = observer;
            }

            public void Dispose()
            {
                lock (observable.observers)
                {
                    observable.observers.Remove(observer);
                }
            }
        }

        #endregion

        #region EmptyDisposable

        private sealed class EmptyDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion

        #region ByReferenceEqualityComparer

        // note (kungurtsev, 02.12.2021): copied from vostok.commons.collections

        private sealed class ByReferenceEqualityComparer<TT> : IEqualityComparer<TT>
        {
            public static readonly ByReferenceEqualityComparer<TT> Instance = new ByReferenceEqualityComparer<TT>();

            public bool Equals(TT x, TT y) => ReferenceEquals(x, y);

            public int GetHashCode(TT obj) => RuntimeHelpers.GetHashCode(obj);
        }

        #endregion
    }
}