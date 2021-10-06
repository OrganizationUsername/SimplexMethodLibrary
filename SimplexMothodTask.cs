using SimplexMethodLibrary.Components;

namespace SimplexMethodLibrary
{
    public class SimplexMethodTask
    {
        #region Properties
        public TargetFunction TargetFunction { get; private set; }
        public SystemOfLimits SystemOfLimits { get; private set; }
        public Table Table { get; private set; }
        #endregion

        #region Constructors
        public SimplexMethodTask(TargetFunction target_function, SystemOfLimits system_of_limits)
        {
            TargetFunction = target_function;
            SystemOfLimits = system_of_limits;
            Table = new Table(target_function, system_of_limits);
        }
        #endregion

        #region Methods
        public double? Calculation()
        {
            return Table.Calculation();
        }
        #endregion
    }
}
