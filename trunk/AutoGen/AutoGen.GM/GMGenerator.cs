using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using DevExpress.XtraEditors;
using ExpScanner;

namespace AutoGen.GM
{
    public class GMGenerator
    {
        public const string CountFunc = "(F(X)-X)^2";
        private string funcTemplate = "";
        private List<string> names = null;
        private List<int> fix = null;
        private Dictionary<int, int> compadre = null;
        private GMBrentopt opt;
        private GMTask innerTask;
        private GMTaskProp what;

        public void Generate(GMTask task)
        {
            innerTask = task;
            int propCount = innerTask.Table.GetCanGenerate().Count;
            if (propCount > 0)
            {
                if (innerTask.Table.Count > 2)
                {
                    propCount = GMMath.GetRandom(0, propCount);
                    what = innerTask.Table[propCount];
                    double needResult = GMMath.GetRandom(what.MinValue, what.MaxValue);
                    needResult = GMMath.CalcToAccuracy(needResult, what.Accuracy, what.MaxValue, what.MinValue);

                    funcTemplate = "((" + what.SolveString + ")-(" + needResult + "))^2";
                    List<double> Xj = new List<double>();
                    List<double> Xj1 = new List<double>();
                    List<string> Sj = new List<string>();
                    fix = new List<int>();
                    compadre = new Dictionary<int, int>();
                    for (int i = 0; i < innerTask.Table.Count; i++)
                    {
                        if (i == propCount)
                            continue;
                        GMTaskProp prop = innerTask.Table[i];
                        double val = GMMath.GetRandom(prop.MinValue, prop.MaxValue);
                        val = GMMath.CalcToAccuracy(val, prop.Accuracy, prop.MaxValue, prop.MinValue);
                        Xj.Add(val);
                        Sj.Add(prop.View);
                        compadre.Add(Sj.IndexOf(prop.View), i);
                    }
                    names = new List<string>(Sj);
                    Xj1 = new List<double>(Xj);
                    //В Xj теперь у нас начальное приближение.
                    double gradDiff = 0;
                    double Epsilon = what.Accuracy > GMMath.Eps ? GMMath.Eps : what.Accuracy;
                    double lam = 0.1;
                    double h = Epsilon*2;
                    List<double> grad;
                    List<double> sd;

                    do
                    {
                        Xj = new List<double>(Xj1);
                        //Найдем вектор градиента
                        //Составим из частных производных df/dx = 1/12h*(f(x-2h)-8*f(x-h)+8*f(x+h)-f(x+2h))
                        grad = new List<double>();
                        for (int i = 0; i < Xj.Count; i++)
                        {
                            List<double> lgrad = new List<double>(Xj);
                            lgrad[i] -= h;
                            List<double> l2grad = new List<double>(Xj);
                            l2grad[i] -= 2*h;
                            List<double> rgrad = new List<double>(Xj);
                            rgrad[i] += h;
                            List<double> r2grad = new List<double>(Xj);
                            r2grad[i] += 2*h;
                            grad.Add((
                                         (Functional(l2grad) - 8*Functional(lgrad) +
                                          8*Functional(rgrad) - Functional(r2grad))
                                         /(12*h)));
                        }
                        //Найдем направление поиска S = -dF(Xj)/||-dF(Xj)||
                        sd = new List<double>();
                        double gNorm = GMMath.Norma(grad.ToArray());
                        if (gNorm > 0)
                        {
                            for (int i = 0; i < grad.Count; i++)
                            {
                                sd.Add((-1*grad[i])/gNorm);
                            }

                            //Найдем значение lam при котором F(X + lam*S) -> min
                            opt = new GMBrentopt();
                            opt.f += opt_f;
                            opt.xList = Xj;
                            opt.sList = sd;
                            double fr = opt.Optimize(0, (gNorm > 1000000 ? gNorm*2 : 2000000), Epsilon, ref lam);
                        }
                        else
                        {
                            lam = 0;
                            for (int i = 0; i < grad.Count; i++)
                            {
                                sd.Add(0);
                            }
                        }

                        //Найдем след точку Xj1
                        //Xj1 = Xj + lam * S
                        Xj1 = new List<double>();
                        for (int i = 0; i < Xj.Count; i++)
                        {
                            Xj1.Add(DoRoundValue(Xj[i] + lam*sd[i], i));
                        }

                        gradDiff = Functional(Xj) - Functional(Xj1);
                        gradDiff = Math.Abs(gradDiff);
                    } while (GMMath.VectDiff(Xj, Xj1) > Epsilon || gradDiff > Epsilon);
                    FormatOutputTask(needResult, Xj1);
                } else if(innerTask.Table.Count == 2)
                {
                    propCount = GMMath.GetRandom(0, 2);
                    what = innerTask.Table[propCount];
                    GMTaskProp who = innerTask.Table[1 - propCount];
                    double needResult = GMMath.GetRandom(what.MinValue, what.MaxValue);
                    needResult = GMMath.CalcToAccuracy(needResult, what.Accuracy, what.MaxValue, what.MinValue);
                    double Epsilon = what.Accuracy > GMMath.Eps ? GMMath.Eps : what.Accuracy;

                    funcTemplate = "((" + what.SolveString + ")-(" + needResult + "))^2";

                    List<string> Sj = new List<string>();
                    fix = new List<int>();
                    compadre = new Dictionary<int, int>();
                    for (int i = 0; i < innerTask.Table.Count; i++)
                    {
                        if (i == propCount)
                            continue;
                        GMTaskProp prop = innerTask.Table[i];
                        Sj.Add(prop.View);
                        compadre.Add(Sj.IndexOf(prop.View), i);
                    }

                    names = new List<string>(Sj);
                    double res = 1.0;
                    opt = new GMBrentopt();

                    opt.f += opt_sf;
                    double fr = opt.Optimize(who.MinValue, who.MaxValue, Epsilon, ref res);
                    List<double> xj = new List<double>();
                    res = GMMath.CalcToAccuracy(res, who.Accuracy, who.MaxValue, who.MinValue);
                    xj.Add(res);
                    FormatOutputTask(needResult, xj);
                }
            }
        }

        private static string GetFromDouble(double value)
        {
            string text = "";
            if (value < 0)
                text = "(" + value + ")";
            else
                text = value.ToString();
            return text;
        }

        private void FormatOutputTask(double result, List<double> xj1)
        {
            GMString gs = new GMString(innerTask.Template, innerTask);
            for (int i = 0; i < xj1.Count; i++)
            {
                gs.Items.Add(new GMStringParametr(names[i], GetFromDouble(xj1[i])));
            }
            gs.Items.Add(new GMStringParametr(what.View, what.View));
            gs.Items.Add(new GMStringParametr(innerTask.Answer, GetFromDouble(result)));
            gs.Items.Add(new GMStringParametr(innerTask.Lookfor, what.View));
            innerTask.Output = gs.Format();
        }

        double opt_f(double l)
        {
            //расчет вектора X-l*S
            List<double> v = new List<double>();
            for (int i = 0; i < opt.xList.Count; i++)
            {
                v.Add(opt.xList[i] + l*opt.sList[i]);
            }
            return Functional(v);
        }

        double opt_sf(double l)
        {
            List<double> v = new List<double>();
            v.Add(l);
            return Functional(v);
        }

        /// <summary>
        /// Вычисляет значение функционала при данных значениях
        /// </summary>
        protected double Functional(List<double> vals)
        {
            double res = 0;
            //Заменяем в строке решения обозначения переменных на уже известные значения
            GMString gs = new GMString(funcTemplate, innerTask);
            for (int i = 0; i < names.Count; i++)
            {
                GMTaskProp prop = innerTask.Table[compadre[i]];
                double val = vals[i];
                if (what.Koeff != 0)
                    val = val*prop.Koeff/what.Koeff;
                else
                    val = val*prop.Koeff;

                gs.Items.Add(new GMStringParametr(names[i], "(" + GMMath.GetRealText(val) + ")"));
            }
            //Генерируем промежуточное значение функционала
            Scanner lineScan = new Scanner(gs.Format().Replace(" ", "").Replace("\r", "").Replace("\n", ""));
            ScInterpret lineInterp = new ScInterpret();
            lineScan.Reset();
            try
            {
                res = lineInterp.GetValue(lineScan);
            }
            catch (Exception err)
            {
                XtraMessageBox.Show(err.Message, "Ошибка интерпретатора");
                return res;
            }

            return res;
        }

        protected double DoRoundValue(double val, int ni)
        {
            GMTaskProp prop = innerTask.Table[compadre[ni]];
            return GMMath.CalcToAccuracy(val, prop.Accuracy, prop.MaxValue, prop.MinValue);
        }
    }

    public static class GMMath
    {
        private static readonly Random r = new Random(DateTime.Now.Millisecond);
        public const double Eps = 0.0000001;
        public const int DecNumbers = 7;

        public static string GetRealText(double value)
        {
            double val = Math.Abs(value);
            if (val > Eps)
                return value.ToString("F" + DecNumbers);
            if (val > 0 && val < Eps)
                return (Eps * Math.Sign(value)).ToString("F" + DecNumbers);
            if (val == 0)
                return "0";
            return value.ToString("F" + DecNumbers);
        }

        /// <summary>
        /// Возвращяет кол-во знаков после запятой.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>+после запятой -до запятой</returns>
        public static int DigitAfterComma(double number)
        {
            int res = 0;

            double anum = Math.Abs(number);
            if (Double.IsInfinity(number))
            {
                res = 0;
                return res;
            }
            if (Double.IsNaN(number))
            {
                res = 0;
                return res;
            }
            if (number == 0.0)
            {
                return 0;
            }
            if (anum - Math.Truncate(anum) == 0)
            {
                while (anum > 0)
                {
                    res--;
                    anum = anum / 10.0;
                    anum = Math.Truncate(anum);
                }
                return res;
            } else
            {
                anum = anum - Math.Truncate(anum);
                while (anum < 1 && anum > 0)
                {
                    res++;
                    anum = anum * 10.0;
                    anum = anum - Math.Truncate(anum);
                }
                return res;
            }
        }

        public static double Norma(double[] vector)
        {
            double norm = 0.0;
            foreach (double x in vector)
                norm += x * x;
            return Math.Sqrt(norm);
        }

        public static int GetRandom(int valMin, int valMax)
        {
            return r.Next(valMin, valMax);
        }

        public static double GetRandom(double valMin, double valMax)
        {
            double diff = (valMax - valMin) * r.NextDouble();
            return valMin + diff;
        }

        public static double RoundAcc(double value, double accuracy)
        {
            double rem = Math.IEEERemainder(value, accuracy);
            return (accuracy/2 > rem) ? value - rem : value + rem;
        }

        public static double CalcToAccuracy(double value, double acc, double max, double min)
        {
            if (RoundAcc(value, acc) < min)
            {
                if (value + acc >= min)
                    return CalcToAccuracy(value + acc, acc, max, min);
                else
                    return CalcToAccuracy(min, acc, max, min);
            }
            else if (RoundAcc(value, acc) > max)
            {
                if (value - acc <= max)
                    return CalcToAccuracy(value - acc, acc, max, min);
                else
                    return CalcToAccuracy(max, acc, max, min);
            }
            else
                return RoundAcc(value, acc);
        }

        public static double VectDiff(List<double> v1, List<double> v2)
        {
            List<double> v = new List<double>();
            for (int i = 0; i < v1.Count; i++)
            {
                v.Add(v1[i] - v2[i]);
            }
            return Norma(v.ToArray());
        }
    }
}