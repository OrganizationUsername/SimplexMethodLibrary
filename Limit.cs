using System.Collections.Generic;
using System.Linq;

namespace SimplexMethodLibrary
{
    public class Limit
    {
        #region Properties
        public List<double> Coeffs { get; set; }
        public double B { get { return Coeffs.Last(); } }
        public Sign Sign { get; set; }
        #endregion

        #region Contructors
        public Limit(List<double> coeffs, Sign sign)
        {
            Coeffs = coeffs;
            Sign = sign;
        }
        #endregion
    }
}
