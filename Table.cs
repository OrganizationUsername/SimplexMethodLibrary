using System;
using System.Collections.Generic;
using System.Linq;

namespace SimplexMethodLibrary
{
    public class Table
    {
        #region Properties
        public double[,] Body { get; private set; }
        public int[] Base { get; private set; }
        public int[] NonBase { get; private set; }
        public TargetFunction TargetFunction { get; private set; }
        public SystemOfLimits SystemOfLimits { get; private set; }
        #endregion

        #region Constructors
        public Table(TargetFunction func, SystemOfLimits limits)
        {
            TargetFunction = func;
            SystemOfLimits = limits;
            Body = new double[limits.Limits.Count + 1, func.Coeffs.Count];
            Base = new int[limits.Limits.Count];
            NonBase = new int[func.Coeffs.Count - 1];
            InitializeNonBase();
            InitializeBase();
            InitializeBody();
        }
        #endregion

        #region Methods
        private void InitializeBody()
        {
            for (int column_index = 0; column_index < Body.GetLength(1); column_index++)
            {
                int row_index = 0;
                if (TargetFunction.Target == Target.MAX)
                {
                    Body[row_index, column_index] = -TargetFunction.Coeffs[column_index];
                }
                else
                {
                    Body[row_index, column_index] = TargetFunction.Coeffs[column_index];
                }
            }

            for (int row_index = 1; row_index < Body.GetLength(0); row_index++)
            {
                Limit current_limit = SystemOfLimits.Limits[row_index - 1];
                for (int column_index = 0; column_index < Body.GetLength(1); column_index++)
                {
                    if (current_limit.Sign == Sign.MoreThan)
                    {
                        Body[row_index, column_index] = -SystemOfLimits.Limits[row_index - 1].Coeffs[column_index];
                    }
                    else
                    {
                        Body[row_index, column_index] = SystemOfLimits.Limits[row_index - 1].Coeffs[column_index];
                    }
                }
                if (current_limit.Sign == Sign.MoreThan) current_limit.Sign = Sign.LessThen;
            }
        }

        private void InitializeBase()
        {
            for (int i = 0; i < Base.Length; i++)
            {
                Base[i] = NonBase.Length + i + 1;
            }
        }

        private void InitializeNonBase()
        {
            for (int i = 0; i < NonBase.Length; i++)
            {
                NonBase[i] = i + 1;
            }
        }

        /// <summary>
        /// Проверка на допустимость
        /// </summary>
        /// <param name="row_index">Индекс ведущей строки</param>
        /// <param name="column_index">Индекс ведущего столбца</param>
        /// <returns>Состояние проверки</returns>
        private CheckStatus ValidityCheck(out int row_index, out int column_index)
        {
            row_index = 0;
            column_index = 0;
            int col_index = Body.GetLength(1) - 1;
            double current_min = 0;
            for (int i = 1; i < Body.GetLength(0); i++)
            {
                if (Body[i, col_index] < 0)
                {
                    if (Body[i, col_index] < current_min)
                    {
                        current_min = Body[i, col_index];
                        row_index = i;
                    }
                }
            }
            if (current_min == 0) return CheckStatus.Complete;
            current_min = 0;
            for (int i = 0; i < Body.GetLength(1) - 1; i++)
            {
                if (Body[row_index, i] < 0)
                {
                    if (Body[row_index, i] < current_min)
                    {
                        current_min = Body[row_index, i];
                        column_index = i;
                    }
                }
            }
            if (current_min == 0) return CheckStatus.NotSolution;
            return CheckStatus.HaveSolution;
        }

        private void SwapInBase(int row_index, int column_index)
        {
            int temp = Base[row_index];
            Base[row_index] = NonBase[column_index];
            NonBase[column_index] = temp;
        }

        /// <summary>
        /// Проверка на оптимальность
        /// </summary>
        /// <param name="row_index">Индекс ведущей строки</param>
        /// <param name="column_index">Индекс ведущего столбца</param>
        /// <returns>Состояние проверки</returns>
        private CheckStatus OptimalityCheck(out int row_index, out int column_index)
        {
            row_index = 0;
            column_index = 0;
            double current_min = 0;
            int b_col_index = Body.GetLength(1) - 1;
            for (int i = 0; i < Body.GetLength(1) - 1; i++)
            {
                if (Body[row_index, i] < 0 && Body[row_index, i] < current_min)
                {
                    current_min = Body[row_index, i];
                    column_index = i;
                }
            }
            if (current_min == 0) return CheckStatus.Complete;

            current_min = double.MaxValue;
            for (int i = 1; i < Body.GetLength(0); i++)
            {
                if (Body[i, b_col_index] > 0 && Body[i, column_index] > 0)
                {
                    if (Body[i, b_col_index] / Body[i, column_index] < current_min)
                    {
                        current_min = Body[i, b_col_index] / Body[i, column_index];
                        row_index = i;
                    }
                }
            }
            if (current_min == double.MaxValue) return CheckStatus.NotSolution;
            return CheckStatus.HaveSolution;
        }

        public double? Calculation()
        {
            CheckStatus validity_status = ValidityCheck(out int row_index, out int column_index);
            switch (validity_status)
            {
                case CheckStatus.Complete:
                    {
                        CheckStatus optimality_status = OptimalityCheck(out row_index, out column_index);
                        switch (optimality_status)
                        {
                            case CheckStatus.Complete:
                                {
                                    List<double> bases = Enumerable.Range(1, TargetFunction.Coeffs.Count - 1).Select(Convert.ToDouble).ToList();
                                    for (int i = 0; i < bases.Count; i++)
                                    {
                                        if (Base.Contains((int)bases[i]))
                                        {
                                            bases[i] = Body[Base.ToList().IndexOf((int)bases[i]) + 1, Body.GetLength(1) - 1];
                                        }
                                        else bases[i] = 0;
                                    }
                                    return bases.Zip(TargetFunction.Coeffs, (argument, coefficient) => argument * coefficient).Sum() + TargetFunction.B;
                                }
                            case CheckStatus.HaveSolution:
                                {
                                    Transformation(row_index, column_index);
                                    return Calculation();
                                }
                            case CheckStatus.NotSolution:
                                {
                                    return null;
                                }
                        }
                        break;
                    }
                case CheckStatus.HaveSolution:
                    {
                        Transformation(row_index, column_index);
                        return Calculation();
                    }
                case CheckStatus.NotSolution:
                    {
                        return null;
                    }
            }
            return null;
        }

        private void Transformation(int row_index, int column_index)
        {
            SwapInBase(row_index - 1, column_index);
            double leading_element = Body[row_index, column_index];

            for (int i = 0; i < Body.GetLength(0); i++)
            {
                for (int j = 0; j < Body.GetLength(1); j++)
                {
                    if (i != row_index && j != column_index) Body[i, j] = Body[i, j] - Body[i, column_index] * Body[row_index, j] / leading_element;
                }
            }

            Body[row_index, column_index] = 1 / leading_element;

            for (int i = 0; i < Body.GetLength(0); i++)
            {
                if (i != row_index) Body[i, column_index] *= -1 / leading_element;
            }

            for (int i = 0; i < Body.GetLength(1); i++)
            {
                if (i != column_index) Body[row_index, i] *= 1 / leading_element;
            }
        }
        #endregion
    }
}
