using System;
using System.Collections.Generic;
using System.Text;

namespace sFraction
{
    // <summary>
    // Класс длинных дробей
    // </summary>
    public class Fraction
    {
        #region Определение переменных и функций
        //private System.Math
        private string numerator;
        private string denominator;

        #region Логическое сравнение строк
        private bool LessThanOrEqualS(string s1, string s2)
        {
            string S1, S2;
            bool res = false;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());

            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1.Equals(S2))
            {
                res = true;
            }
            else if (LessThanS(s1, s2))
            {
                res = true;
            }
            else
            {
                res = false;
            }
            return res;
        }
        private bool LessThanS(string s1, string s2)
        {
            string S1, S2;
            bool res = false;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());

            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1.Equals(S2))
            {
                res = false;
            }
            else if (S1[0].Equals('-') & S2[0].Equals('+'))
            {
                res = true;
            }
            else if (S1[0].Equals('+') & S2[0].Equals('-'))
            {
                res = false;
            }
            else if (S1[0].Equals('+') & S2[0].Equals('+'))
            {
                if (S1.Length < S2.Length)
                {
                    res = true;
                }
                else if (S1.Length > S2.Length)
                {
                    res = false;
                }
                else
                {
                    res = S1.CompareTo(S2) < 0;
                }
            }
            else
            {
                if (S1.Length > S2.Length)
                {
                    res = true;
                }
                else if (S1.Length < S2.Length)
                {
                    res = false;
                }
                else
                {
                    res = S1.CompareTo(S2)>0;
                }
            }
            return res;
        }
        private bool GreaterThanS(string s1, string s2)
        {
            string S1, S2;
            bool res = false;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());
            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1.Equals(S2))
            {
                res = false;
            }
            else if (LessThanS(S1, S2))
            {
                res = false;
            }
            else
            {
                res = true;
            }
            return res;
        }
        private bool GreaterThanOrEqualS(string s1, string s2)
        {
            string S1, S2;
            bool res = false;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());
            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1.Equals(S2))
            {
                res = true;
            }
            else if (GreaterThanS(S1, S2))
            {
                res = true;
            }
            else
            {
                res = false;
            }
            return res;
        }
        
        #endregion
        #region Строковые процедуры и функции
        private string mAddString(string s1, string s2)
        {
            int i, l1, l2;
            string s, S1, S2, res;
            res = "";
            int x1, x2, y, m;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());
            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1[0].Equals('+') & S2[0].Equals('+'))
            {
                if (S1.Length < S2.Length)
                {
                    s = S1;
                    S1 = S2;
                    S2 = s;
                }
                l1 = S1.Length;
                l2 = S2.Length;
                if (l1 > l2)
                {
                    for (i = l2 - 1; i <= l1 - 1; i++)
                    {
                        S2 = S2.Insert(1, "0");
                    }
                }
                m = 0;
                s = "";
                for (i = 0; i < l1 - 1; i++)
                {
                    x1 = Convert.ToInt32(S1[l1 - i - 1].ToString());
                    x2 = Convert.ToInt32(S2[l1 - i - 1].ToString());
                    y = x1 + x2 + m;
                    if (y >= 10)
                    {
                        m = 1;
                        y = y - 10;
                    }
                    else
                    {
                        m = 0;
                    }
                    //s.Insert(0,y.ToString());
                    s = y.ToString() + s;
                }
                if (m > 0)
                {
                    s = "1" + s;
                }
                s = "+" + s;
                res = s;
            }
            else if (S1[0].Equals('-') & S2[0].Equals('-'))
            {

                S1 = "+" + S1.Remove(0, 1);
                //S1 = S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2 = S2.Insert(0, "+");
                s = mAddString(S1, S2);
                s = "+" + s.Remove(0, 1);
                //s = s.Insert(0, "-");
                res = s;
            }
            else if (S1[1].Equals('+') & S2[1].Equals('-'))
            {
                S2 = "+" + S2.Remove(0, 1);
                //S2=S2.Insert(0, "+");
                s = mSubString(S1, S2);
                res = s;
            }
            else if (S1[1].Equals('-') & S2[1].Equals('+'))
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1=S1.Insert(0, "+");
                s = mSubString(S2, S1);
                res = s;
            }
            this.ControlS(ref res);
            return res;
        }
        private string mSubString(string s1, string s2)
        {
            int i, l1, l2;
            string s, S1, S2, res = "";
            int x1, x2, y, m;
            s = "";
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());
            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1[0].Equals('+') & S2[0].Equals('+'))
            {
                l1 = S1.Length;
                l2 = S2.Length;
                if (l1 > l2)
                {
                    for (i = l2; i < l1; i++)
                    {
                        S2 = S2.Insert(1, "0");
                    }
                }
                else if (l1 < l2)
                {
                    for (i = l1; i < l2; i++)
                    {
                        S1 = S1.Insert(1, "0");
                    }
                }
                if (GreaterThanOrEqualS(S1, S2))
                {
                    m = 0;
                    for (i = 0; i < l1 - 1; i++)
                    {
                        x1 = Convert.ToInt32(S1[l1 - i-1].ToString());
                        x2 = Convert.ToInt32(S2[l1 - i-1].ToString());
                        y = x1 - x2 - m;
                        if (y < 0)
                        {
                            m = 1;
                            y = y + 10;
                        }
                        else
                        {
                            m = 0;
                        }
                        s = y.ToString() + s;
                    }
                    res = "+" + s;
                }
                else
                {
                    s = mSubString(S2, S1);
                    s = "-" + s.Remove(0, 1);
                    //s.Insert(0, "-");
                    res = s;
                }
            }
            else if (S1[0].Equals('-') & S2[0].Equals('-'))
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = mSubString(S2, S1);
            }
            else if (S1[0].Equals('+') & S2[0].Equals('-'))
            {
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = mAddString(S1, S2);
            }
            else if (S1[0].Equals('-') & S2[0].Equals('+'))
            {
                S2 = "-" + S2.Remove(0, 1);
                //S2.Insert(0, "-");
                res = mAddString(S1, S2);
            }
            this.ControlS(ref res);
            return res;
        }
        private string mMultString(string s1, string s2)
        {
            int i, j, k;
            string x, y, z;
            string S1, S2, res = "";
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());

            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1[0].Equals('+') & S2[0].Equals('+'))
            {
                if (this.LessThanOrEqualS(S1, S2))
                {
                    x = S1;
                    y = S2;
                }
                else
                {
                    x = S2;
                    y = S1;
                }
                res = "+0";
                i = x.Length-1;
                while (i >= 1)
                {
                    k = Convert.ToInt32(x[i].ToString());
                    z = "+0";
                    if (k > 0)
                    {
                        for (j = 0; j < k; j++)
                        {
                            z = this.mAddString(z, y);
                        }
                    }
                    res = this.mAddString(res, z);
                    y = y + "0";
                    i--;
                }
            }
            else if (S1[0].Equals('-') & S2[0].Equals('-'))
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = this.mMultString(S1, S2);
            }
            else
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = this.mMultString(S1, S2);
                res = "-" + res.Remove(0, 1);
                //res.Insert(0, "-");
            }
            this.ControlS(ref res);
            return res;
        }
        private string mDivString(string s1, string s2, ref string r)
        {
            int i, k;
            string x;
            string S1, S2, res = "";
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());

            this.ControlS(ref S1);
            this.ControlS(ref S2);
            if (S1[0].Equals('+') & S2[0].Equals('+'))
            {
                if (this.LessThanS(S1, S2))
                {
                    res = "+0";
                    r = S1;
                }
                else
                {
                    res = "+";
                    i = 2;
                    x = S1.Substring(0, i);
                    i--;
                    while (this.LessThanS(x, S2))
                    {
                        i++;
                        x = x + S1[i].ToString();
                    }
                    while (i < S1.Length)
                    {
                        k = 0;
                        while (this.GreaterThanOrEqualS(x, S2))
                        {
                            x = this.mSubString(x, S2);
                            k++;
                        }
                        res = res + k.ToString();
                        i++;
                        if (i >= S1.Length)
                        {
                            r = x;
                        }
                        else
                        {
                            x = x + S1[i].ToString();
                        }
                    }
                }
            }
            else if (S1[0].Equals('-') & S2[0].Equals('-'))
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = this.mDivString(S1, S2, ref r);
                r = "-" + r.Remove(0, 1);
                //r.Insert(0, "-");
            }
            else if (S1[0].Equals('-'))
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = this.mDivString(S1, S2, ref r);
                res = "-" + res.Remove(0, 1);
                //res.Insert(0, "-");
                r = "-" + r.Remove(0, 1);
                //r.Insert(0, "-");
            }
            else
            {
                S1 = "+" + S1.Remove(0, 1);
                //S1.Insert(0, "+");
                S2 = "+" + S2.Remove(0, 1);
                //S2.Insert(0, "+");
                res = this.mDivString(S1, S2, ref r);
                res = "-" + res.Remove(0, 1);
                //res.Insert(0, "-");
            }
            this.ControlS(ref res);
            this.ControlS(ref r);
            return res;
        }
        #endregion

        #region Вспомогательные процедуры
        private void ControlS(ref string s)
        {
            int i;
            if (!(s[0].Equals('+') | s[0].Equals('-')))
            {
                s = "+" + s;
            }
            i = 1;
            while (i < s.Length)
            {
                if (!(Convert.ToInt32(s[i].ToString()) >= 0 & Convert.ToInt32(s[i].ToString()) <= 9))
                {
                    s = s.Remove(i, 1);
                }
                else
                {
                    i++;
                }
            }
            if (s.Length == 1)
            { s += "0"; }
            while (s[1].Equals('0') & s.Length > 2)
            {
                s = s.Remove(1, 1);
            }
            if (s == "-0")
            {
                s = "+0";
            }
        }
        private void Control()
        {
            string S, r = "";
            if (this.denominator == "")
            { this.denominator = "+1"; }
            this.ControlS(ref this.numerator);
            this.ControlS(ref this.denominator);
            S = this.NOD(this.numerator, this.denominator);
            this.numerator = this.mDivString(this.numerator, S, ref r);
            this.denominator = this.mDivString(this.denominator, S, ref r);
            if (this.denominator[0].Equals('-'))
            {
                if (this.numerator[0].Equals('+'))
                {
                    this.numerator = this.numerator.Replace("+", "-");
                }
                else
                {
                    this.numerator = this.numerator.Replace("-", "+");
                }
            }
            //else
            //{
            //    //this.numerator = "+" + this.numerator.Remove(0, 1);
            //    //this.numerator.Insert(0, "+");
            //}
            this.denominator = "+" + this.denominator.Remove(0, 1);
            //this.denominator.Insert(0, "+");

        }
        private string NOD(string s1, string s2)
        {
            string S1, S2, res, s;
            S1 = new string(s1.ToCharArray());
            S2 = new string(s2.ToCharArray());
            this.ControlS(ref S1);
            this.ControlS(ref S2);
            S1 = "+" + S1.Remove(0, 1);
            //S1.Insert(0, "+");
            S2 = "+" + S2.Remove(0, 1);
            //S2.Insert(0, "+");
            if (S1 == "+0" & S2 == "+0")
            { res = "+1"; }
            else if (S1 == "+0")
            {
                res = S2;
            }
            else if (S2 == "+0")
            {
                res = S1;
            }
            else
            {
                while (S2 != "+0")
                {
                    mDivString(S1, S2, ref S1);
                    s = S1;
                    S1 = S2;
                    S2 = s;
                }
                res = S1;
            }
            return res;
        }
        private string NOK(string s1, string s2)
        {
            string c, s, res;
            s = mMultString(s1, s2);
            c = NOD(s1, s2);
            res = mDivString(s, c, ref s);
            res = "+" + res.Remove(0, 1);
            //res.Insert(0, "+");
            return res;
        }
        #endregion

        #region Конструкторы
        public Fraction(string p, string q)
        {
            this.numerator = p;
            this.denominator = q;
            this.Control();
        }
        //public Fraction(double e)
        //{
        //    string s1, s2;
        //    int i, j;
        //    double e1;
        //    Fraction E;
        //    e1 = e; i = 0;
        //    while (e1 != Math.Floor(e1))
        //    {
        //        e1 = e1 * 10;
        //        i++;
        //    }
        //    s1 = e1.ToString("[sign]integral-digits");
        //    s2 = "+1";
        //    for (j = 0; j < i; j++)
        //    {
        //        s2 += "0";
        //    }
        //    E = new Fraction(s1, s2);
        //    this.numerator = E.numerator;
        //    this.denominator = E.denominator;
        //}
        public Fraction(string st)
        {
            int i;
            string s1, s2;
            Fraction E;
            i = st.IndexOf("/");
            if (i == -1)
            {
                s1 = st;
                s2 = "+1";
                E = new Fraction(s1, s2);
                this.numerator = E.numerator;
                this.denominator = E.denominator;
            }
            else
            {
                s1 = st.Substring(0, i);
                s2 = st.Substring(i + 1, st.Length - i-1);
                E = new Fraction(s1, s2);
                this.numerator = E.numerator;
                this.denominator = E.denominator;
            }
        }
        public Fraction(Fraction a)
        {
            numerator = a.numerator;
            denominator = a.denominator;
            Control();
        }
        #endregion

        #endregion

        public override String ToString()
        {
            string s1, s2, res="";
            this.Control();
            s1 = this.numerator;
            s2 = this.denominator;
            s1 = s1.Remove(0, 1);
            s2 = s2.Remove(0, 1);
            if (this.numerator[0].Equals('-'))
            {
                res = "-";
            }
            else
            {
                res = "";
            }
            res = res + s1;
            if (this.denominator != "+1")
            {
                res = res + @"/" + s2;
            }
            return res;
            //Условие для вывода при нуле
        }
        public String ToString(int cou)
        {
            string s1, s2, res = "";
            int v = cou;
            s2 = "";
            s1 = this.mDivString(this.numerator, this.denominator, ref s2);
            if (v > 0)
            {
                if (s1[0].Equals('+'))
                { s1 = s1.Remove(0, 1); }
                res = s1;
                if (s2 != "+0")
                { res = res + @","; }
                while (s2 != "+0" & v > 0)
                {
                    s1 = s2 + "0";
                    s1 = this.mDivString(s1, this.denominator, ref s2);
                    s1 = s1.Remove(0, 1);
                    res = res + s1;
                    v--;
                }
            }
            else
            {
                if (s1[0].Equals('-'))
                { s1 = s1.Remove(0, 1); }
                res = s1;
            }
            if (this.numerator[0].Equals('-'))
            { res = "-" + res; }
            return res;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static Fraction operator -(Fraction a)
        {
            return (double)(-1) * a;
        }
        public static implicit operator double(Fraction a)
        { 
            Fraction f = new Fraction(a);
            string s1 = f.numerator.Remove(0,1);
            string s2 = f.denominator.Remove(0,1);
            return Convert.ToDouble(f.ToString(s2.Length));
        }
        public static implicit operator Fraction(double e)
        {
            string s1, s2;
            int i, j;
            double e1;
            Fraction E;
            e1 = e; i = 0;
            while (e1 != Math.Floor(e1))
            {
                e1 = e1 * 10;
                i++;
            }
            s1 = e1.ToString();
            s2 = "+1";
            for (j = 0; j < i; j++)
            {
                s2 += "0";
            }
            E = new Fraction(s1, s2);
            return E;
        }

        #region Операторы сравнения
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        } 
        public static bool operator ==(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return (v.numerator == "+0");
        }
        //public static bool operator ==(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator == "+0");
        //}
        //public static bool operator ==(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator == "+0");
        //}

        public static bool operator !=(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return !(v.numerator == "+0");
        }

        //public static bool operator !=(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return !(v.numerator == "+0");
        //}
        //public static bool operator !=(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return !(v.numerator == "+0");
        //}

        public static bool operator >=(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return v.numerator[0].Equals('+');
        }
        //public static bool operator >=(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return v.numerator[0].Equals('+');
        //}
        //public static bool operator >=(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return v.numerator[0].Equals('+');
        //}

        public static bool operator >(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return (v.numerator[0].Equals('+') & v.numerator != "+0");
        }
        //public static bool operator >(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator[0].Equals('+') & v.numerator != "+0");
        //}
        //public static bool operator >(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator[0].Equals('+') & v.numerator != "+0");
        //}

        public static bool operator <(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return v.numerator[0].Equals('-');
        }
        //public static bool operator <(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return v.numerator[0].Equals('-');
        //}
        //public static bool operator <(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return v.numerator[0].Equals('-');
        //}

        public static bool operator <=(Fraction a, Fraction b)
        {
            Fraction v = a - b;
            return (v.numerator[0].Equals('-') | v.numerator == "+0");
        }

        //public static bool operator <=(double a, Fraction b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator[0].Equals('-') | v.numerator == "+0");
        //}
        //public static bool operator <=(Fraction a, double b)
        //{
        //    Fraction v = a - b;
        //    return (v.numerator[0].Equals('-') | v.numerator == "+0");
        //}

        #endregion

        #region Сложение
        public static Fraction operator +(Fraction a, Fraction b)
        { 
            string s1, s2;
            s1 = a.mMultString(a.numerator,b.denominator);
            s2 = a.mMultString(a.denominator,b.numerator);
            s1 = a.mAddString(s1,s2);
            s2 = a.mMultString(a.denominator,b.denominator);
            return new Fraction(s1,s2);
        }
        //public static Fraction operator +(Fraction a, double b)
        //{
        //    Fraction c = new Fraction(b);
        //    return a + c;
        //}
        //public static Fraction operator +(double a, Fraction b)
        //{
        //    Fraction c = new Fraction(a);
        //    return c + b;
        //}
        #endregion

        #region Вычитание
        public static Fraction operator -(Fraction a, Fraction b)
        {
            string s1, s2;
            s1 = a.mMultString(a.numerator, b.denominator);
            s2 = a.mMultString(a.denominator, b.numerator);
            s1 = a.mSubString(s1, s2);
            s2 = a.mMultString(a.denominator, b.denominator);
            return new Fraction(s1, s2);
        }
        //public static Fraction operator -(Fraction a, double b)
        //{
        //    Fraction c = new Fraction(b);
        //    return a - c;
        //}
        //public static Fraction operator -(double a, Fraction b)
        //{
        //    Fraction c = new Fraction(a);
        //    return c - b;
        //}
        #endregion

        #region Умножение
        public static Fraction operator *(Fraction a, Fraction b)
        {
            string s1, s2;
            s1 = a.mMultString(a.numerator, b.numerator);
            s2 = a.mMultString(a.denominator, b.denominator);
            return new Fraction(s1, s2);
        }
        //public static Fraction operator *(Fraction a, double b)
        //{
        //    Fraction c = new Fraction(b);
        //    return a * c;
        //}
        //public static Fraction operator *(double a, Fraction b)
        //{
        //    Fraction c = new Fraction(a);
        //    return c * b;
        //}
        #endregion

        #region Деление
        public static Fraction operator /(Fraction a, Fraction b)
        {
            string s1, s2;
            s1 = a.mMultString(a.numerator, b.denominator);
            s2 = a.mMultString(a.denominator, b.numerator);
            return new Fraction(s1, s2);
        }
        //public static Fraction operator /(Fraction a, double b)
        //{
        //    Fraction c = new Fraction(b);
        //    return a / c;
        //}
        //public static Fraction operator /(double a, Fraction b)
        //{
        //    Fraction c = new Fraction(a);
        //    return c / b;
        //}
        #endregion

    }
}
