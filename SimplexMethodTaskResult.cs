using System.Collections.Generic;

namespace SimplexMethodLibrary
{
    public class SimplexMethodTaskResult
    {
        public double OptimalValue { get; }
        public List<double> ArgumentsValues { get; }
        public SimplexMethodTaskResult(double optimal_value, List<double> arguments_values)
        {
            OptimalValue = optimal_value;
            ArgumentsValues = arguments_values;
        }
    }
}
