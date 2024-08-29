using System.Collections.Generic;

namespace Data
{
    public class StateWieghtPriorityComparer : IComparer<State>
    {
        public int Compare(State x, State y)
        {
            return x.EdgeWeight.CompareTo(y.EdgeWeight);
        }
    }
}