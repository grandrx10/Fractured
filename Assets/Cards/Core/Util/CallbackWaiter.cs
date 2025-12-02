using System;
using Cards.Environments;

namespace Cards.Core.Util
{
    public class CallbackWaiter2<T>
    {
        private T _a, _b;
        private int _count = 0;
        private readonly Action<T, T> _onDone;

        public CallbackWaiter2(Action<T, T> onDone)
        {
            _onDone = onDone;
        }

        public CardSubmitState SetA(T value)
        {
            _a = value;
            if (++_count == 2) _onDone(_a, _b);
            return CardSubmitState.Success;
        }

        public CardSubmitState SetB(T value)
        {
            _b = value;
            if (++_count == 2) _onDone(_a, _b);
            return CardSubmitState.Success;
        }
    }
    
    public class CallbackWaiterN<T>
    {
        private readonly T[] _results;
        private readonly Action<T[]> _onDone;
        private int _completed = 0;

        public CallbackWaiterN(int count, Action<T[]> onDone)
        {
            _results = new T[count];
            _onDone = onDone;
        }

        public CardSubmitState AddResult(int index, T value)
        {
            _results[index] = value;
            _completed++;

            if (_completed == _results.Length)
            {
                _onDone(_results);
            }
            return CardSubmitState.Success;
        }
    }
}