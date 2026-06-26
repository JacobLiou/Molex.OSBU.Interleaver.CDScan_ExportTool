
using System;

namespace LinearFit
{
    public partial class LeastSquareFit
    {
        private static double[,] Em = new double[10, 10];
        private static double sum(double[] dNumarry, int n)
        {

            double[] dTemp = new double[n];

            double dSum = 0;

            dTemp = dNumarry;
            for (int i = 0; i < n; i++)
            {
                dSum += dTemp[i];
            }

            return dSum;
        }

        private static double MutilSum(double[] dX, double[] dY, int n)
        {
            double[] dXTemp = new double[n];
            double[] dYTemp = new double[n];
            double dMultiSum = 0;

            dXTemp = dX;
            dYTemp = dY;

            for (int i = 0; i < n; i++)
            {
                dMultiSum += dX[i] * dY[i];
            }

            return dMultiSum;

        }

        private static double RelatePow(double[] dx, int n, int ex)
        {
            double[] dTemp = new double[n];
            double ReSum = 0;
            dTemp = dx;
            for (int j = 0; j < n; j++)
            {
                ReSum += Math.Pow(dTemp[j], ex);
            }
            return ReSum;

        }

        private static double RelateMutiXY(double[] dx, double[] dy, int n, int ex)
        {
            double[] dXTemp = new double[n];
            double[] dYTemp = new double[n];
            double dReMultiSum = 0;

            dXTemp = dx;
            dYTemp = dy;

            for (int i = 0; i < n; i++)
            {
                dReMultiSum += Math.Pow(dXTemp[i], ex) * dYTemp[i];
            }
            return dReMultiSum;

        }
        //------------------------------------------------------------------
        //函数名：EMatrix
        //输入：double *dx 输入点的x坐标 double *dy 输入点的y坐标
        //      int n  输入点的个数  int ex 拟合的阶数
        //      double coefficient[]  拟合后获得的系数数组
        //注意：
        //     拟合的阶数输入的时候需要加一，如果要求4阶那么需要输入5
        //     拟合后的系数数组不是从0下标开始而是从1下标开始
        //作者：wanxin wang
        //时间：2010.2.20
        //-------------------------------------------------------------------
        public static void EMatrix(double[] dx, double[] dy, int n, int ex, double[] coefficient)
        {
            double[] dXTemp = new double[n];
            double[] dYTemp = new double[n];
            dXTemp = dx;
            dYTemp = dy;

            //Array.Clear(Em, 0, 10 * 10);
            for (int i = 1; i <= ex; i++)
            {

                for (int j = 1; j <= ex; j++)
                {
                    Em[i, j] = RelatePow(dXTemp, n, i + j - 2);
                }
                Em[i, ex + 1] = RelateMutiXY(dXTemp, dYTemp, n, i - 1);
            }
            Em[1, 1] = n;
            CalEquation(ex, coefficient);
        }

        // can only support up to order 19
        public static void ReturnFitedArray(double[] dx, double[] dy, int n, int ex, double[] dfitX, out double[] dFitY)
        {
            double[] dblCoeff = new double[10];

            double dblBaseVal = dx[0];
            for (int i = 0; i < dx.Length; i++)
            {
                dx[i] -= dblBaseVal;
            }
            for (int i = 0; i < dfitX.Length; i++)
            {
                dfitX[i] -= dblBaseVal;
            }

            EMatrix(dx, dy, n, ex, dblCoeff);

            dFitY = new double[dfitX.Length];
            for (int i = 0; i < dfitX.Length; i++)
            {
                dFitY[i] = CalcFitedVal(dfitX[i], dblCoeff, ex - 1);
            }

            for (int i = 0; i < dfitX.Length; i++)
            {
                dfitX[i] += dblBaseVal;
            }
        }

        public static double CalcFitedVal(double dblX, double[] dblCoeff, int nOrder)
        {
            double dblTempVal = dblCoeff[1];
            double dblCurX = 1;

            for (int i = 0; i < nOrder; i++)
            {
                dblCurX = dblCurX * dblX;
                dblTempVal += dblCoeff[i + 2] * dblCurX;
            }

            return dblTempVal;
        }

        private static void CalEquation(int exp, double[] coefficient)
        {
            for (int k = 1; k < exp; k++) //消元过程
            {
                for (int i = k + 1; i < exp + 1; i++)
                {
                    double p1 = 0;

                    if (Em[k, k] != 0)
                        p1 = Em[i, k] / Em[k, k];

                    for (int j = k; j < exp + 2; j++)
                        Em[i, j] = Em[i, j] - Em[k, j] * p1;
                }
            }

            coefficient[exp] = Em[exp, exp + 1] / Em[exp, exp];
            for (int l = exp - 1; l >= 1; l--)   //回代求解
                coefficient[l] = (Em[l, exp + 1] - F(coefficient, l + 1, exp)) / Em[l, l];
        }

        private static double F(double[] c, int l, int m)
        {
            double sum = 0;
            for (int i = l; i <= m; i++)
                sum += Em[l - 1, i] * c[i];
            return sum;
        }

        public static double LinearFitByTwoPoint(double dblX1, double dblX2, double dblY1, double dblY2, double dblX)
        {
            double k = (dblY2 - dblY1) / (dblX2 - dblX1);
            double c = dblY1 - k * dblX1;

            return (k * dblX + c);
        }
    }

    public partial class LeastSquareFitEx
    {
        private static decimal[,] Em = new decimal[10, 10];
        private static decimal sum(decimal[] dNumarry, int n)
        {

            decimal[] dTemp = new decimal[n];

            decimal dSum = 0;

            dTemp = dNumarry;
            for (int i = 0; i < n; i++)
            {
                dSum += dTemp[i];
            }

            return dSum;
        }

        private static decimal MutilSum(decimal[] dX, decimal[] dY, int n)
        {
            decimal[] dXTemp = new decimal[n];
            decimal[] dYTemp = new decimal[n];
            decimal dMultiSum = 0;

            dXTemp = dX;
            dYTemp = dY;

            for (int i = 0; i < n; i++)
            {
                dMultiSum += dX[i] * dY[i];
            }

            return dMultiSum;

        }

        private static decimal RelatePow(decimal[] dx, int n, int ex)
        {
            decimal[] dTemp = new decimal[n];
            decimal ReSum = 0;
            dTemp = dx;
            for (int j = 0; j < n; j++)
            {
                ReSum += (decimal)Math.Pow((double)dTemp[j], ex);
            }
            return ReSum;

        }

        private static decimal RelateMutiXY(decimal[] dx, decimal[] dy, int n, int ex)
        {
            decimal[] dXTemp = new decimal[n];
            decimal[] dYTemp = new decimal[n];
            decimal dReMultiSum = 0;

            dXTemp = dx;
            dYTemp = dy;

            for (int i = 0; i < n; i++)
            {
                dReMultiSum += ((decimal)Math.Pow((double)dXTemp[i], ex)) * dYTemp[i];//Math.Pow(dXTemp[i], ex) * dYTemp[i];
            }
            return dReMultiSum;

        }
        //------------------------------------------------------------------
        //函数名：EMatrix
        //输入：decimal *dx 输入点的x坐标 decimal *dy 输入点的y坐标
        //      int n  输入点的个数  int ex 拟合的阶数
        //      decimal coefficient[]  拟合后获得的系数数组
        //注意：
        //     拟合的阶数输入的时候需要加一，如果要求4阶那么需要输入5
        //     拟合后的系数数组不是从0下标开始而是从1下标开始
        //作者：wanxin wang
        //时间：2010.2.20
        //-------------------------------------------------------------------
        public static void EMatrix(decimal[] dx, decimal[] dy, int n, int ex, decimal[] coefficient)
        {
            decimal[] dXTemp = new decimal[n];
            decimal[] dYTemp = new decimal[n];
            dXTemp = dx;
            dYTemp = dy;

            //Array.Clear(Em, 0, 10 * 10);
            for (int i = 1; i <= ex; i++)
            {

                for (int j = 1; j <= ex; j++)
                {
                    Em[i, j] = RelatePow(dXTemp, n, i + j - 2);
                }
                Em[i, ex + 1] = RelateMutiXY(dXTemp, dYTemp, n, i - 1);
            }
            Em[1, 1] = n;
            CalEquation(ex, coefficient);
        }

        // can only support up to order 19
        public static void ReturnFitedArray(decimal[] dx, decimal[] dy, int n, int ex, decimal[] dfitX, out decimal[] dFitY)
        {
            decimal[] dblCoeff = new decimal[10];

            EMatrix(dx, dy, n, ex, dblCoeff);

            dFitY = new decimal[dfitX.Length];
            for (int i = 0; i < dfitX.Length; i++)
            {
                dFitY[i] = CalcFitedVal(dfitX[i], dblCoeff, ex - 1);
            }
        }

        public static decimal CalcFitedVal(decimal dblX, decimal[] dblCoeff, int nOrder)
        {
            decimal dblTempVal = dblCoeff[1];
            decimal dblCurX = 1;

            for (int i = 0; i < nOrder; i++)
            {
                dblCurX = dblCurX * dblX;
                dblTempVal += dblCoeff[i + 2] * dblCurX;
            }

            return dblTempVal;
        }

        private static void CalEquation(int exp, decimal[] coefficient)
        {
            for (int k = 1; k < exp; k++) //消元过程
            {
                for (int i = k + 1; i < exp + 1; i++)
                {
                    decimal p1 = 0;

                    if (Em[k, k] != 0)
                        p1 = Em[i, k] / Em[k, k];

                    for (int j = k; j < exp + 2; j++)
                        Em[i, j] = Em[i, j] - Em[k, j] * p1;
                }
            }

            coefficient[exp] = Em[exp, exp + 1] / Em[exp, exp];
            for (int l = exp - 1; l >= 1; l--)   //回代求解
                coefficient[l] = (Em[l, exp + 1] - F(coefficient, l + 1, exp)) / Em[l, l];
        }

        private static decimal F(decimal[] c, int l, int m)
        {
            decimal sum = 0;
            for (int i = l; i <= m; i++)
                sum += Em[l - 1, i] * c[i];
            return sum;
        }

        public static decimal LinearFitByTwoPoint(decimal dblX1, decimal dblX2, decimal dblY1, decimal dblY2, decimal dblX)
        {
            decimal k = (dblY2 - dblY1) / (dblX2 - dblX1);
            decimal c = dblY1 - k * dblX1;

            return (k * dblX + c);
        }
    }
}