using System.Collections.Generic;
using System.Linq;
using SimplexMethodLibrary.Components.Helpers;

namespace SimplexMethodLibrary.Components
{
    public class TargetFunction
    {
        #region Properties
        public List<double> Coeffs { get; private set; }
        public double B { get { return Coeffs.Last(); } }
        public Target Target { get; set; }
        #endregion

        #region Constructors
        public TargetFunction(List<double> coeffs, Target target)
        {
            Coeffs = coeffs;
            Target = target;
        }
        #endregion
    }
}
