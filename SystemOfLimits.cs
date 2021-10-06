using System.Collections.Generic;

namespace SimplexMethodLibrary
{
    public class SystemOfLimits
    {
        #region Properties
        public List<Limit> Limits { get; private set; }
        #endregion

        #region Constructors
        public SystemOfLimits(List<Limit> limits)
        {
            Limits = limits;
        }
        #endregion
    }
}
