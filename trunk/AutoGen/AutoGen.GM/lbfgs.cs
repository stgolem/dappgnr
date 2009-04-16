/*************************************************************************
Copyright (c) 1980-2007, Jorge Nocedal.

Contributors:
    * Sergey Bochkanov (ALGLIB project). Translation from FORTRAN to
      pseudocode.

This software is freely available for educational or commercial  purposes.
We expect that all publications describing work using this software  quote
at least one of the references given below:
    * J. Nocedal. Updating  Quasi-Newton  Matrices  with  Limited  Storage
      (1980), Mathematics of Computation 35, pp. 773-782.
    * D.C. Liu and J. Nocedal. On the  Limited  Memory  Method  for  Large
      Scale  Optimization  (1989),  Mathematical  Programming  B,  45,  3,
      pp. 503-528.
*************************************************************************/

using System;
namespace AutoGen.GM
{
    public delegate void GMLbgfsFuncgrad(double[] x,
                                         ref double f,
                                         ref double[] g);

    public class lbfgs
    {
        /*
        This members must be defined by you:
        static void funcgrad(double[] x,
            ref double f,
            ref double[] g)
        */

        public event GMLbgfsFuncgrad funcgrad;

        /*************************************************************************
                LIMITED MEMORY BFGS METHOD FOR LARGE SCALE OPTIMIZATION
                                  JORGE NOCEDAL

        Подпрограмма минимизирует  функцию  N  аргументов  F(x)  с  использованием
        квази-Ньютоновского метода (LBFGS-схема), оптимизированного по использованию
        оперативной памяти.

        Подпрограмма стоит аппроксимацию матрицы, обратной к  Гессиану  фунции,  с
        использованием информации о M предыдущих шагах алгоритма  (вместо N),  что
        позволяет  снизить  требуемый  объем оперативной памяти с величины порядка
        N^2 до величины порядка 2*N*M.

        Подпрограмма использует в ходе расчетов подпрограмму FuncGrad, вычисляющую
        в точке X (массив с нумерацией элементов от 1 до N) значение  функции F  и
        градиент G (массив с нумерацией элементов от 1 до N). Программист должен
        определить подпрограмму FuncGrad самостоятельно.  Следует  отметить,   что
        подпрограмма не должна тратить время на выделение памяти под массив G, т.к.
        память под массив выделяется в вызывающей программе. Если программист будет
        устанавливать  размер  массива  G  при  каждом вызове подпрограммы, то это
        излишне замедлит работу алгоритма.

        Также  программист  может  переопределить  подпрограмму  LBFGSNewIteration,
        которая вызывается на каждой новой итерации алгоритма и в которую передаются
        текущая точка X, значение функции F, градиент G.  Подпрограмму имеет смысл
        переопределять  в  отладочных  целях,  например, для визуализации процесса
        решения.

        Входные параметры алгоритма:
            N       -   Размерность задачи. N>0
            M       -   Число коррекций в BFGS-схеме обновления аппроксимации
                        Гессиана. Рекомендуемое   значение:  3 <= M <= 7.  Меньшее
                        значение не позволит добиться нормальной скорости сходимости,
                        большее - не позволит получить заметный выигрыш в скорости
                        сходимости, зато приведет к падению быстродействия.
                        M<=N.
            X       -   Начальное приближение к решению.
                        Массив с нумерацией элементов от 1 до N.
            EpsG    -   Положительное число, определяющее точность поиска минимума
                        Подпрограмма прекращает  работу, если выполняется  условие
                        ||G|| < EpsG, где ||.|| обозначает  Евклидову  норму,  G -
                        градиент, X - текущее приближение к минимуму.
            EpsF    -   Положительное число, определяющее точность поиска минимума
                        Подпрограмма прекращает работу, если на  k+1-ой   итерации
                        выполняется условие
                        |F(k+1)-F(k)| <= EpsF*max{|F(k)|, |F(k+1)|, 1}
            EpsX    -   Положительное число, определяющее точность поиска минимума
                        Подпрограмма прекращает работу, если на  k+1-ой   итерации
                        выполняется условие |X(k+1)-X(k)| <= EpsX
            MaxIts  -   Максимальное число итераций алгоритма.
                        Если MaxIts=0, то число итераций не ограничено.

        Выходные параметры алгоритма:
            X   -   Конечное приближение к решению. Массив с нумерацией элементов
                    от 1 до N.
            Info-   причина прекращения работы подпрограммы:
                            * -1    указаны неверные параметры
                            *  0    прервано пользователем.
                            *  1    относительное уменьшение функции не превосходит
                                    EpsF.
                            *  2    изменение текущего приближения не превосходит
                                    EpsX.
                            *  4    норма градиента не превосходит EpsG
                            *  5    превышено максимальное число итераций MaxIts

        ИСТОРИЯ ИЗМЕНЕНИЙ:
            01.08.2005  Перевод с FORTRAN в рамках проекта ALGLIB
            21.02.2006  Параметр Eps переименован в EpsG.
                        Добавлены параметры EpsF и EpsX.
                        Изменен список кодов ошибок.
                        Добавлена подпрограмма LBFGSNewIteration.
        *************************************************************************/
        public void lbfgsminimize(int n,
            int m,
            ref double[] x,
            double epsg,
            double epsf,
            double epsx,
            int maxits,
            ref int info)
        {
            double[] w = new double[0];
            double f = 0;
            double fold = 0;
            double tf = 0;
            double txnorm = 0;
            double v = 0;
            double[] xold = new double[0];
            double[] tx = new double[0];
            double[] g = new double[0];
            double[] diag = new double[0];
            double[] ta = new double[0];
            bool finish = new bool();
            double gnorm = 0;
            double stp1 = 0;
            double ftol = 0;
            double stp = 0;
            double ys = 0;
            double yy = 0;
            double sq = 0;
            double yr = 0;
            double beta = 0;
            double xnorm = 0;
            int iter = 0;
            int nfun = 0;
            int point = 0;
            int ispt = 0;
            int iypt = 0;
            int maxfev = 0;
            int bound = 0;
            int npt = 0;
            int cp = 0;
            int i = 0;
            int nfev = 0;
            int inmc = 0;
            int iycn = 0;
            int iscn = 0;
            double xtol = 0;
            double gtol = 0;
            double stpmin = 0;
            double stpmax = 0;
            int i_ = 0;

            w = new double[n * (2 * m + 1) + 2 * m + 1];
            g = new double[n + 1];
            xold = new double[n + 1];
            tx = new double[n + 1];
            diag = new double[n + 1];
            ta = new double[n + 1];
            funcgrad(x, ref f, ref g);
            fold = f;
            iter = 0;
            info = 0;
            if (n <= 0 | m <= 0 | m > n | epsg < 0 | epsf < 0 | epsx < 0 | maxits < 0)
            {
                info = -1;
                return;
            }
            nfun = 1;
            point = 0;
            finish = false;
            for (i = 1; i <= n; i++)
            {
                diag[i] = 1;
            }
            xtol = 100 * AP.Math.MachineEpsilon;
            gtol = 0.9;
            stpmin = Math.Pow(10, -20);
            stpmax = Math.Pow(10, 20);
            ispt = n + 2 * m;
            iypt = ispt + n * m;
            for (i = 1; i <= n; i++)
            {
                w[ispt + i] = -(g[i] * diag[i]);
            }
            gnorm = Math.Sqrt(lbfgsdotproduct(n, ref g, 1, ref g, 1));
            if (gnorm <= epsg)
            {
                info = 4;
                return;
            }
            stp1 = 1 / gnorm;
            ftol = 0.0001;
            maxfev = 20;
            while (true)
            {
                for (i_ = 1; i_ <= n; i_++)
                {
                    xold[i_] = x[i_];
                }
                iter = iter + 1;
                info = 0;
                bound = iter - 1;
                if (iter != 1)
                {
                    if (iter > m)
                    {
                        bound = m;
                    }
                    ys = lbfgsdotproduct(n, ref w, iypt + npt + 1, ref w, ispt + npt + 1);
                    yy = lbfgsdotproduct(n, ref w, iypt + npt + 1, ref w, iypt + npt + 1);
                    for (i = 1; i <= n; i++)
                    {
                        diag[i] = ys / yy;
                    }
                    cp = point;
                    if (point == 0)
                    {
                        cp = m;
                    }
                    w[n + cp] = 1 / ys;
                    for (i = 1; i <= n; i++)
                    {
                        w[i] = -g[i];
                    }
                    cp = point;
                    for (i = 1; i <= bound; i++)
                    {
                        cp = cp - 1;
                        if (cp == -1)
                        {
                            cp = m - 1;
                        }
                        sq = lbfgsdotproduct(n, ref w, ispt + cp * n + 1, ref w, 1);
                        inmc = n + m + cp + 1;
                        iycn = iypt + cp * n;
                        w[inmc] = w[n + cp + 1] * sq;
                        lbfgslincomb(n, -w[inmc], ref w, iycn + 1, ref w, 1);
                    }
                    for (i = 1; i <= n; i++)
                    {
                        w[i] = diag[i] * w[i];
                    }
                    for (i = 1; i <= bound; i++)
                    {
                        yr = lbfgsdotproduct(n, ref w, iypt + cp * n + 1, ref w, 1);
                        beta = w[n + cp + 1] * yr;
                        inmc = n + m + cp + 1;
                        beta = w[inmc] - beta;
                        iscn = ispt + cp * n;
                        lbfgslincomb(n, beta, ref w, iscn + 1, ref w, 1);
                        cp = cp + 1;
                        if (cp == m)
                        {
                            cp = 0;
                        }
                    }
                    for (i = 1; i <= n; i++)
                    {
                        w[ispt + point * n + i] = w[i];
                    }
                }
                nfev = 0;
                stp = 1;
                if (iter == 1)
                {
                    stp = stp1;
                }
                for (i = 1; i <= n; i++)
                {
                    w[i] = g[i];
                }
                lbfgsmcsrch(n, ref x, ref f, ref g, ref w, ispt + point * n + 1, ref stp, ftol, xtol, maxfev, ref info, ref nfev, ref diag, gtol, stpmin, stpmax);
                if (info != 1)
                {
                    if (info == 0)
                    {
                        info = -1;
                        return;
                    }
                }
                nfun = nfun + nfev;
                npt = point * n;
                for (i = 1; i <= n; i++)
                {
                    w[ispt + npt + i] = stp * w[ispt + npt + i];
                    w[iypt + npt + i] = g[i] - w[i];
                }
                point = point + 1;
                if (point == m)
                {
                    point = 0;
                }
                if (iter > maxits & maxits > 0)
                {
                    info = 5;
                    return;
                }
                lbfgsnewiteration(ref x, f, ref g);
                gnorm = Math.Sqrt(lbfgsdotproduct(n, ref g, 1, ref g, 1));
                if (gnorm <= epsg)
                {
                    info = 4;
                    return;
                }
                tf = Math.Max(Math.Abs(fold), Math.Max(Math.Abs(f), 1.0));
                if (fold - f <= epsf * tf)
                {
                    info = 1;
                    return;
                }
                for (i_ = 1; i_ <= n; i_++)
                {
                    tx[i_] = xold[i_];
                }
                for (i_ = 1; i_ <= n; i_++)
                {
                    tx[i_] = tx[i_] - x[i_];
                }
                xnorm = Math.Sqrt(lbfgsdotproduct(n, ref x, 1, ref x, 1));
                txnorm = Math.Max(xnorm, Math.Sqrt(lbfgsdotproduct(n, ref xold, 1, ref xold, 1)));
                txnorm = Math.Max(txnorm, 1.0);
                v = Math.Sqrt(lbfgsdotproduct(n, ref tx, 1, ref tx, 1));
                if (v <= epsx)
                {
                    info = 2;
                    return;
                }
                fold = f;
                for (i_ = 1; i_ <= n; i_++)
                {
                    xold[i_] = x[i_];
                }
            }
        }


        /*************************************************************************
        Линейная комбинация векторов. Служебная подпрограмма.
        *************************************************************************/
        private static void lbfgslincomb(int n,
            double da,
            ref double[] dx,
            int sx,
            ref double[] dy,
            int sy)
        {
            int fx = 0;
            int fy = 0;
            int i_ = 0;
            int i1_ = 0;

            fx = sx + n - 1;
            fy = sy + n - 1;
            i1_ = (sx) - (sy);
            for (i_ = sy; i_ <= fy; i_++)
            {
                dy[i_] = dy[i_] + da * dx[i_ + i1_];
            }
        }


        /*************************************************************************
        Скалярное произведение векторов. Служебная подпрограмма.
        *************************************************************************/
        private static double lbfgsdotproduct(int n,
            ref double[] dx,
            int sx,
            ref double[] dy,
            int sy)
        {
            double result = 0;
            double v = 0;
            int fx = 0;
            int fy = 0;
            int i_ = 0;
            int i1_ = 0;

            fx = sx + n - 1;
            fy = sy + n - 1;
            i1_ = (sy) - (sx);
            v = 0.0;
            for (i_ = sx; i_ <= fx; i_++)
            {
                v += dx[i_] * dy[i_ + i1_];
            }
            result = v;
            return result;
        }


        /*************************************************************************
        Подпрограмма MCSRCH.

        Служебная подпрограмма, не должна вызываться непосредственно.

        Ищет  шаг  вдоль  указанной прямой, удовлетворяющий условиям существенного
        уменьшения значения функции.

        Ниже приведено оригинальное описание алгоритма.

        THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
        DECREASE CONDITION AND A CURVATURE CONDITION.

        AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
        ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
        SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

            F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

        IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
        FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
        UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

        THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
        DECREASE CONDITION

            F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

        AND THE CURVATURE CONDITION

            ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

        IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
        BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
        IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
        ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
        IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.

        PARAMETERS DESCRIPRION

        N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

        X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
        THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

        F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
        IT CONTAINS THE VALUE OF F AT X + STP*S.

        G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
        ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

        S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

        STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
        OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

        FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
        SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
        SATISFIED.

        XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
        WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

        STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
        UPPER BOUNDS FOR THE STEP.

        MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
        NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

        INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
            INFO = 0  IMPROPER INPUT PARAMETERS.

            INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
                      DIRECTIONAL DERIVATIVE CONDITION HOLD.

            INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
                      IS AT MOST XTOL.

            INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

            INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

            INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

            INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
                      THERE MAY NOT BE A STEP WHICH SATISFIES THE
                      SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
                      TOLERANCES MAY BE TOO SMALL.

        NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

        WA IS A WORK ARRAY OF LENGTH N.

        ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
        JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        private void lbfgsmcsrch(int n,
            ref double[] x,
            ref double f,
            ref double[] g,
            ref double[] s,
            int sstart,
            ref double stp,
            double ftol,
            double xtol,
            int maxfev,
            ref int info,
            ref int nfev,
            ref double[] wa,
            double gtol,
            double stpmin,
            double stpmax)
        {
            int infoc = 0;
            int j = 0;
            bool brackt = new bool();
            bool stage1 = new bool();
            double dg = 0;
            double dgm = 0;
            double dginit = 0;
            double dgtest = 0;
            double dgx = 0;
            double dgxm = 0;
            double dgy = 0;
            double dgym = 0;
            double finit = 0;
            double ftest1 = 0;
            double fm = 0;
            double fx = 0;
            double fxm = 0;
            double fy = 0;
            double fym = 0;
            double p5 = 0;
            double p66 = 0;
            double stx = 0;
            double sty = 0;
            double stmin = 0;
            double stmax = 0;
            double width = 0;
            double width1 = 0;
            double xtrapf = 0;
            double zero = 0;
            double mytemp = 0;

            sstart = sstart - 1;
            p5 = 0.5;
            p66 = 0.66;
            xtrapf = 4.0;
            zero = 0;
            funcgrad(x, ref f, ref g);
            infoc = 1;
            info = 0;
            if (n <= 0 | stp <= 0 | ftol < 0 | gtol < zero | xtol < zero | stpmin < zero | stpmax < stpmin | maxfev <= 0)
            {
                return;
            }
            dginit = 0;
            for (j = 1; j <= n; j++)
            {
                dginit = dginit + g[j] * s[j + sstart];
            }
            if (dginit >= 0)
            {
                return;
            }
            brackt = false;
            stage1 = true;
            nfev = 0;
            finit = f;
            dgtest = ftol * dginit;
            width = stpmax - stpmin;
            width1 = width / p5;
            for (j = 1; j <= n; j++)
            {
                wa[j] = x[j];
            }
            stx = 0;
            fx = finit;
            dgx = dginit;
            sty = 0;
            fy = finit;
            dgy = dginit;
            while (true)
            {
                if (brackt)
                {
                    if (stx < sty)
                    {
                        stmin = stx;
                        stmax = sty;
                    } else
                    {
                        stmin = sty;
                        stmax = stx;
                    }
                } else
                {
                    stmin = stx;
                    stmax = stp + xtrapf * (stp - stx);
                }
                if (stp > stpmax)
                {
                    stp = stpmax;
                }
                if (stp < stpmin)
                {
                    stp = stpmin;
                }
                if (brackt & (stp <= stmin | stp >= stmax) | nfev >= maxfev - 1 | infoc == 0 | brackt & stmax - stmin <= xtol * stmax)
                {
                    stp = stx;
                }
                for (j = 1; j <= n; j++)
                {
                    x[j] = wa[j] + stp * s[j + sstart];
                }
                funcgrad(x, ref f, ref g);
                info = 0;
                nfev = nfev + 1;
                dg = 0;
                for (j = 1; j <= n; j++)
                {
                    dg = dg + g[j] * s[j + sstart];
                }
                ftest1 = finit + stp * dgtest;
                if (brackt & (stp <= stmin | stp >= stmax) | infoc == 0)
                {
                    info = 6;
                }
                if (stp == stpmax & f <= ftest1 & dg <= dgtest)
                {
                    info = 5;
                }
                if (stp == stpmin & (f > ftest1 | dg >= dgtest))
                {
                    info = 4;
                }
                if (nfev >= maxfev)
                {
                    info = 3;
                }
                if (brackt & stmax - stmin <= xtol * stmax)
                {
                    info = 2;
                }
                if (f <= ftest1 & Math.Abs(dg) <= -(gtol * dginit))
                {
                    info = 1;
                }
                if (info != 0)
                {
                    return;
                }
                mytemp = ftol;
                if (gtol < ftol)
                {
                    mytemp = gtol;
                }
                if (stage1 & f <= ftest1 & dg >= mytemp * dginit)
                {
                    stage1 = false;
                }
                if (stage1 & f <= fx & f > ftest1)
                {
                    fm = f - stp * dgtest;
                    fxm = fx - stx * dgtest;
                    fym = fy - sty * dgtest;
                    dgm = dg - dgtest;
                    dgxm = dgx - dgtest;
                    dgym = dgy - dgtest;
                    lbfgsmcstep(ref stx, ref fxm, ref dgxm, ref sty, ref fym, ref dgym, ref stp, fm, dgm, ref brackt, stmin, stmax, ref infoc);
                    fx = fxm + stx * dgtest;
                    fy = fym + sty * dgtest;
                    dgx = dgxm + dgtest;
                    dgy = dgym + dgtest;
                } else
                {
                    lbfgsmcstep(ref stx, ref fx, ref dgx, ref sty, ref fy, ref dgy, ref stp, f, dg, ref brackt, stmin, stmax, ref infoc);
                }
                if (brackt)
                {
                    if (Math.Abs(sty - stx) >= p66 * width1)
                    {
                        stp = stx + p5 * (sty - stx);
                    }
                    width1 = width;
                    width = Math.Abs(sty - stx);
                }
            }
        }


        /*************************************************************************
        Подпрограмма MCSTEP.

        Служебная подпрограмма, не должна вызываться непосредственно.

        Подпрограмма проводит один шаг поиска минимума вдоль прямой. Ниже приведено
        оригинальное описание.

             THE PURPOSE OF MCSTEP IS TO COMPUTE A SAFEGUARDED STEP FOR
             A LINESEARCH AND TO UPDATE AN INTERVAL OF UNCERTAINTY FOR
             A MINIMIZER OF THE FUNCTION.

             THE PARAMETER STX CONTAINS THE STEP WITH THE LEAST FUNCTION
             VALUE. THE PARAMETER STP CONTAINS THE CURRENT STEP. IT IS
             ASSUMED THAT THE DERIVATIVE AT STX IS NEGATIVE IN THE
             DIRECTION OF THE STEP. IF BRACKT IS SET TRUE THEN A
             MINIMIZER HAS BEEN BRACKETED IN AN INTERVAL OF UNCERTAINTY
             WITH ENDPOINTS STX AND STY.

             THE SUBROUTINE STATEMENT IS

               SUBROUTINE MCSTEP(STX,FX,DX,STY,FY,DY,STP,FP,DP,BRACKT,
                                STPMIN,STPMAX,INFO)

             WHERE

               STX, FX, AND DX ARE VARIABLES WHICH SPECIFY THE STEP,
                 THE FUNCTION, AND THE DERIVATIVE AT THE BEST STEP OBTAINED
                 SO FAR. THE DERIVATIVE MUST BE NEGATIVE IN THE DIRECTION
                 OF THE STEP, THAT IS, DX AND STP-STX MUST HAVE OPPOSITE
                 SIGNS. ON OUTPUT THESE PARAMETERS ARE UPDATED APPROPRIATELY.

               STY, FY, AND DY ARE VARIABLES WHICH SPECIFY THE STEP,
                 THE FUNCTION, AND THE DERIVATIVE AT THE OTHER ENDPOINT OF
                 THE INTERVAL OF UNCERTAINTY. ON OUTPUT THESE PARAMETERS ARE
                 UPDATED APPROPRIATELY.

               STP, FP, AND DP ARE VARIABLES WHICH SPECIFY THE STEP,
                 THE FUNCTION, AND THE DERIVATIVE AT THE CURRENT STEP.
                 IF BRACKT IS SET TRUE THEN ON INPUT STP MUST BE
                 BETWEEN STX AND STY. ON OUTPUT STP IS SET TO THE NEW STEP.

               BRACKT IS A LOGICAL VARIABLE WHICH SPECIFIES IF A MINIMIZER
                 HAS BEEN BRACKETED. IF THE MINIMIZER HAS NOT BEEN BRACKETED
                 THEN ON INPUT BRACKT MUST BE SET FALSE. IF THE MINIMIZER
                 IS BRACKETED THEN ON OUTPUT BRACKT IS SET TRUE.

               STPMIN AND STPMAX ARE INPUT VARIABLES WHICH SPECIFY LOWER
                 AND UPPER BOUNDS FOR THE STEP.

               INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
                 IF INFO = 1,2,3,4,5, THEN THE STEP HAS BEEN COMPUTED
                 ACCORDING TO ONE OF THE FIVE CASES BELOW. OTHERWISE
                 INFO = 0, AND THIS INDICATES IMPROPER INPUT PARAMETERS.

             SUBPROGRAMS CALLED

               FORTRAN-SUPPLIED ... ABS,MAX,MIN,SQRT

             ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
             JORGE J. MORE', DAVID J. THUENTE
        *************************************************************************/
        private static void lbfgsmcstep(ref double stx,
            ref double fx,
            ref double dx,
            ref double sty,
            ref double fy,
            ref double dy,
            ref double stp,
            double fp,
            double dp,
            ref bool brackt,
            double stmin,
            double stmax,
            ref int info)
        {
            bool bound = new bool();
            double gamma = 0;
            double p = 0;
            double q = 0;
            double r = 0;
            double s = 0;
            double sgnd = 0;
            double stpc = 0;
            double stpf = 0;
            double stpq = 0;
            double theta = 0;

            info = 0;
            if (brackt & (stp <= Math.Min(stx, sty) | stp >= Math.Max(stx, sty)) | dx * (stp - stx) >= 0 | stmax < stmin)
            {
                return;
            }
            sgnd = dp * (dx / Math.Abs(dx));
            if (fp > fx)
            {
                info = 1;
                bound = true;
                theta = 3 * (fx - fp) / (stp - stx) + dx + dp;
                s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                gamma = s * Math.Sqrt(AP.Math.Sqr(theta / s) - dx / s * (dp / s));
                if (stp < stx)
                {
                    gamma = -gamma;
                }
                p = gamma - dx + theta;
                q = gamma - dx + gamma + dp;
                r = p / q;
                stpc = stx + r * (stp - stx);
                stpq = stx + dx / ((fx - fp) / (stp - stx) + dx) / 2 * (stp - stx);
                if (Math.Abs(stpc - stx) < Math.Abs(stpq - stx))
                {
                    stpf = stpc;
                } else
                {
                    stpf = stpc + (stpq - stpc) / 2;
                }
                brackt = true;
            } else
            {
                if (sgnd < 0)
                {
                    info = 2;
                    bound = false;
                    theta = 3 * (fx - fp) / (stp - stx) + dx + dp;
                    s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                    gamma = s * Math.Sqrt(AP.Math.Sqr(theta / s) - dx / s * (dp / s));
                    if (stp > stx)
                    {
                        gamma = -gamma;
                    }
                    p = gamma - dp + theta;
                    q = gamma - dp + gamma + dx;
                    r = p / q;
                    stpc = stp + r * (stx - stp);
                    stpq = stp + dp / (dp - dx) * (stx - stp);
                    if (Math.Abs(stpc - stp) > Math.Abs(stpq - stp))
                    {
                        stpf = stpc;
                    } else
                    {
                        stpf = stpq;
                    }
                    brackt = true;
                } else
                {
                    if (Math.Abs(dp) < Math.Abs(dx))
                    {
                        info = 3;
                        bound = true;
                        theta = 3 * (fx - fp) / (stp - stx) + dx + dp;
                        s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dx), Math.Abs(dp)));
                        gamma = s * Math.Sqrt(Math.Max(0, AP.Math.Sqr(theta / s) - dx / s * (dp / s)));
                        if (stp > stx)
                        {
                            gamma = -gamma;
                        }
                        p = gamma - dp + theta;
                        q = gamma + (dx - dp) + gamma;
                        r = p / q;
                        if (r < 0 & gamma != 0)
                        {
                            stpc = stp + r * (stx - stp);
                        } else
                        {
                            if (stp > stx)
                            {
                                stpc = stmax;
                            } else
                            {
                                stpc = stmin;
                            }
                        }
                        stpq = stp + dp / (dp - dx) * (stx - stp);
                        if (brackt)
                        {
                            if (Math.Abs(stp - stpc) < Math.Abs(stp - stpq))
                            {
                                stpf = stpc;
                            } else
                            {
                                stpf = stpq;
                            }
                        } else
                        {
                            if (Math.Abs(stp - stpc) > Math.Abs(stp - stpq))
                            {
                                stpf = stpc;
                            } else
                            {
                                stpf = stpq;
                            }
                        }
                    } else
                    {
                        info = 4;
                        bound = false;
                        if (brackt)
                        {
                            theta = 3 * (fp - fy) / (sty - stp) + dy + dp;
                            s = Math.Max(Math.Abs(theta), Math.Max(Math.Abs(dy), Math.Abs(dp)));
                            gamma = s * Math.Sqrt(AP.Math.Sqr(theta / s) - dy / s * (dp / s));
                            if (stp > sty)
                            {
                                gamma = -gamma;
                            }
                            p = gamma - dp + theta;
                            q = gamma - dp + gamma + dy;
                            r = p / q;
                            stpc = stp + r * (sty - stp);
                            stpf = stpc;
                        } else
                        {
                            if (stp > stx)
                            {
                                stpf = stmax;
                            } else
                            {
                                stpf = stmin;
                            }
                        }
                    }
                }
            }
            if (fp > fx)
            {
                sty = stp;
                fy = fp;
                dy = dp;
            } else
            {
                if (sgnd < 0.0)
                {
                    sty = stx;
                    fy = fx;
                    dy = dx;
                }
                stx = stp;
                fx = fp;
                dx = dp;
            }
            stpf = Math.Min(stmax, stpf);
            stpf = Math.Max(stmin, stpf);
            stp = stpf;
            if (brackt & bound)
            {
                if (sty > stx)
                {
                    stp = Math.Min(stx + 0.66 * (sty - stx), stp);
                } else
                {
                    stp = Math.Max(stx + 0.66 * (sty - stx), stp);
                }
            }
        }


        /*************************************************************************
        Подпрограмма, вызываемая на каждой итерации алгоритма.

        Может переопределяться программистом для отладочных целей, например -  для
        визуализации итеративного процесса.
        *************************************************************************/
        private static void lbfgsnewiteration(ref double[] x,
            double f,
            ref double[] g)
        {
        }
    }
}