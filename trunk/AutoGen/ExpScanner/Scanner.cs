using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;

namespace ExpScanner
{
  [ProgId("ScScanner.Connect")]
  public class Scanner
  {
    private int sPos;//Положение в строке начального курсора
    private int lPos;//Положение в строке конечного курсора
    private Char sc_Current = Convert.ToChar(0);
    private CharEnumerator sc_Buf;
    private String sc_String;
    public Scanner(String scString)
    {
      sc_String = scString.ToUpper();
      sc_Buf = sc_String.GetEnumerator();
      sPos = -1;
      lPos = -1;
    }
    public bool IsEmpty()
    {
      return sc_Current.Equals('\0');
    }
    public char Current()
    {
      return sc_Current;
    }
    public void MoveNext()
    {
      if (sc_Buf.MoveNext())
      { sc_Current = sc_Buf.Current; }
      else
      { sc_Current = Convert.ToChar(0); }
      sPos++;
      lPos++;
    }
    public void Reset()
    {
      sc_Buf.Reset();
      sc_Current = Convert.ToChar(0);
      sPos = -1;
      lPos = -1;
    }
    public bool IsStartEq(string start)
    {
      return sc_String.Substring(sPos).StartsWith(start);
    }
    public void MoveSteps(int stepsCount)
    {
      for (int i = 0; i < stepsCount; i++)
      {
        this.MoveNext();
      }
    }
    public String GetScString()
    {
      return sc_String;
    }
  }
  [ProgId("ScErrors.Connect")]
  public class ScErrors
  {
    /// <summary>
    /// 
    /// </summary>
    public class ScErrBase : System.Exception
    {
      public ScErrBase() { }
      public override string Message
      {
        get { return "Exspression Calc Error"; }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrEndOfLine : ScErrBase
    {
      private Char sBadChar;
      public ScErrEndOfLine(Char b) { sBadChar = b; }
      public override string Message { get { return @"End of line expected"; } }
      public override string Source
      {
        get { return sBadChar.ToString(); }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrDigitOrOpen : ScErrBase
    {
      private Char sBadChar;
      public ScErrDigitOrOpen(Char b) { sBadChar = b; }
      public override string Message { get { return @"Digit or ( expected"; } }
      public override string Source
      {
        get { return sBadChar.ToString(); }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrDigit : ScErrBase
    {
      private Char sBadChar;
      public ScErrDigit(Char b) { sBadChar = b; }
      public override string Message { get { return @"Digit expected"; } }
      public override string Source
      {
        get { return sBadChar.ToString(); }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrClose : ScErrBase
    {
      private Char sBadChar;
      public ScErrClose(Char b) { sBadChar = b; }
      public override string Message { get { return @"Close ) expected"; } }
      public override string Source
      {
        get { return sBadChar.ToString(); }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrEmptyBuf : ScErrBase
    {
      public ScErrEmptyBuf() { }
      public override string Message { get { return @"Bufer is empty"; } }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ScErrDivideZero : ScErrBase
    {
      public ScErrDivideZero() { }
      public override string Message { get { return @"Divizion by zero"; } }
    }
  }
  [ProgId("ScInterpret.Connect")]
  public class ScInterpret
  {
    #region Alphabet
    //Разделитель десятичных дробей
    private char decDelimit = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
    //Массивы опрераторов и символов
    private static String[] iNumbers = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
    private static String[] iAddOpers = { @"+", @"-" };
    private static String[] iMulOpers = { @"*", @"/" };
    private static String[] iPowOpers = { @"^" };
    //внимательно с функциями проверка идет на совпадение "<имя>("
    private static String[] iStrOpers = { @"COS", @"SIN", @"SQRT" };
    //тоже самое с константами только без "("
    private static String[] iStrConst = { @"PI", @"E" };
    private static Double[] iDblConst = { Math.PI, Math.E };
    //private static System.Collections.DictionaryBase scBaseOpers
    private static System.Collections.ArrayList scNumbers = new System.Collections.ArrayList();
    private static System.Collections.ArrayList scAddOpers = new System.Collections.ArrayList();
    private static System.Collections.ArrayList scMulOpers = new System.Collections.ArrayList();
    private static System.Collections.ArrayList scPowOpers = new System.Collections.ArrayList();
    private static System.Collections.ArrayList scStrOpers = new System.Collections.ArrayList();
    private static System.Collections.ArrayList scStrConst = new System.Collections.ArrayList();
    //Приоритет операторов
    private static System.Collections.SortedList scPriority = new SortedList();
    #endregion

    #region PrivateFuncs
    /// <summary>
    /// Переводим символ в цифру
    /// </summary>
    /// <param name="c">Символ</param>
    /// <returns>Цифра</returns>
    /// <exception>Символ не цифра</exception>
    private int CharToInt(Char c)
    {
      try
      { return Convert.ToInt32(c.ToString()); }
      catch
      {
        throw new ScErrors.ScErrDigit(c);
      }
    }
    /// <summary>
    /// Обработка числа
    /// </summary>
    /// <param name="b">Типа Scanner</param>
    /// <returns>Новое число из буфера</returns>
    private Double GetNumber(ref Scanner b)
    {
      // проверяем, не пуст ли буфер
      if (b.IsEmpty())
      { throw new ScErrors.ScErrEmptyBuf(); }
      // проверяем, является ли текущая литера цифрой
      if (!Char.IsNumber(b.Current()))
      { throw new ScErrors.ScErrDigit(b.Current()); }
      // все в порядке - как и ожидалось, в буфере число
      double resultValue = 0.0;
      // считываем целую часть числа
      while (Char.IsNumber(b.Current()))
      {
        resultValue *= 10.0;
        resultValue += CharToInt(b.Current());
        b.MoveNext();
      } // while
      // проверяем наличие необязательной дробной части
      if (b.Current().Equals(decDelimit))
      {
        // пропускаем десятичную точку
        b.MoveNext();
        // считываем дробную часть
        // сначала убедимся, что после точки следует цифра
        if (!Char.IsNumber(b.Current()))
        { throw new ScErrors.ScErrDigit(b.Current()); }
        double step = 1.0;
        while (Char.IsNumber(b.Current()))
        {
          step /= 10.0;
          resultValue += step * CharToInt(b.Current());
          b.MoveNext();
        } // while
      } // if
      // возвращаем значение считанного числа
      return resultValue;

    }
    /// <summary>
    /// Обработка сомножителя
    /// суда добавлять обработку любых функций
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private Double GetFactor(ref Scanner b)
    {
      // сомножитель может быть числом,
      if (Char.IsNumber(b.Current())) // в этом случае его значение = значению этого числа,
      { return GetNumber(ref b); }
      // либо выражением в скобках, в этом случае его следует вычислить
      else if (b.Current().Equals('('))
      {
        // пропускаем открывающую скобку
        b.MoveNext();
        // получаем значение выражения
        double resultValue = GetExpr(ref b);
        // проверяем наличие закрывающей скобки
        if (!b.Current().Equals(')'))
        { throw new ScErrors.ScErrClose(b.Current()); }
        // пропускаем закрывающую скобку
        b.MoveNext();
        // возвращаем значение сомножителя
        return resultValue;
      } // else if
      //Либо строковой функцией
      String curOper = IsBeginFormAny(ref b, iStrOpers, true);
      if (curOper != null)//тогда считаем значение функции
      {
        double resultValue = 0.0;
        b.MoveSteps(curOper.Length);
        if (b.Current().Equals('('))
        {
          b.MoveNext();
          switch (curOper)
          {
            case "SIN":
            resultValue = Math.Sin(GetExpr(ref b));
            break;
            case "COS":
            resultValue = Math.Cos(GetExpr(ref b));
            break;
            case "SQRT":
            resultValue = Math.Sqrt(GetExpr(ref b));
            break;
          }
        }
        if (!b.Current().Equals(')'))
        { throw new ScErrors.ScErrClose(b.Current()); }
        b.MoveNext();
        return resultValue;
      }
      //Либо константой
      String curConst = IsBeginFormAny(ref b, iStrConst, false);
      if (curConst != null)//Заменяем на значение константы
      {
        return iDblConst[scStrConst.IndexOf(curConst)];
      }
      // текущий символ - не нашего поля
      throw new ScErrors.ScErrDigitOrOpen(b.Current());
    }
    /// <summary>
    /// Обработка слагаемого
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private Double GetItem(ref Scanner b)
    {
      // получаем первый сомножитель
      double resultValue = GetFactor(ref b);

      //проверяем, следует ли за ним операция типа степень
      while (scPowOpers.Contains(b.Current().ToString()))
      {
        // сохраняем литеру операции
        Char powOp = b.Current();
        b.MoveNext();
        // получаем следующий сомножитель
        double secondOp = GetFactor(ref b);
        // выполняем операцию типа степень
        switch (powOp)
        {
          case '^':
          resultValue = Math.Pow(resultValue,secondOp);
          break;
          default:
          throw new ScErrors.ScErrBase();
        }
      } // while

      // проверяем, следует ли за ним операция типа умножения
      while (scMulOpers.Contains(b.Current().ToString()))
      {
        // сохраняем литеру операции
        Char mulOp = b.Current();
        b.MoveNext();
        // получаем следующий сомножитель
        double secondOp = GetFactor(ref b);
        // выполняем операцию типа умножения
        switch (mulOp)
        {
          case '*':
          resultValue *= secondOp;
          break;
          case '/':
          // если операция - деление, следует проверить, 
          //   не равен ли нулю второй операнд
          if (secondOp == 0.0)
            throw new ScErrors.ScErrDivideZero();
          // все в порядке - можно делить
          resultValue /= secondOp;
          break;
          default:
          throw new ScErrors.ScErrBase();
        }
      } // while
      return resultValue;
    }
    /// <summary>
    /// Обработка выражения
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    private Double GetExpr(ref Scanner b)
    {
      // получить первое слагаемое
      double resultValue;
      if (b.Current().Equals('-'))
      {
        b.MoveNext();
        resultValue = (-1)*GetItem(ref b);
      }
      else if (b.Current().Equals('+'))
      {
        b.MoveNext();
        resultValue = GetItem(ref b);
      }
      else
      {
        resultValue = GetItem(ref b);
      }
      // проверяем, следует ли за ним операция типа сложения
      while (scAddOpers.Contains(b.Current().ToString()))
      {
        // сохраняем литеру операции
        Char addOp = b.Current();
        b.MoveNext();
        // получаем следующее слагаемое
        double secondOp = GetItem(ref b);
        // выполняем операцию типа сложения
        switch (addOp)
        {
          case '+':
          resultValue += secondOp;
          break;
          case '-':
          resultValue -= secondOp;
          break;
          default:
          throw new ScErrors.ScErrBase();
        }
      } // while
      return resultValue;
      //...
    }

    #endregion

    #region SupportFuncs
    /// <summary>
    /// Проверяет наличие имени из коллекции в начале строки
    /// </summary>
    /// <param name="b">Сканер</param>
    /// <param name="funcCollection">Массив с именами</param>
    /// <param name="isFunc">Признак функции(добавляет "(")</param>
    /// <returns></returns>
    private String IsBeginFormAny(ref Scanner b, String[] funcCollection, bool isFunc)
    {
      string addon = "";
      if (isFunc)
      {
        addon = "(";
      }
      foreach (String function in funcCollection)
      {
        if (b.IsStartEq(function+addon))
        {
          return function;
        }
      }
      return null;
    }
    
    #endregion

    #region MainFunc
    /// <summary>
    /// Возвращает значение переданной строки <typeparamref name="Double"/>
    /// </summary>
    /// <param name="b">Тип Scanner</param>
    /// <returns><typeparamref name="Double"/></returns>
    public Double GetValue(Scanner b)
    {
      b.MoveNext();
      // вычисляем выражение, находящееся в сканере
      Double resultValue = GetExpr(ref b);
      // если по завершении работы сканер не пуст, 
      //   значит, в конце выражения что-то неладно
      if (!b.IsEmpty())
      { throw new ScErrors.ScErrEndOfLine(b.Current()); }
      // все в порядке - нормальное завершение, возвращаем значение выражения
      return resultValue;
      //...
    }
    #endregion

    public ScInterpret()
    {
      #region Переброс в листы
      int i = 0;
      for (i = 0; i < iNumbers.Length; i++)
      {
        scNumbers.Add(iNumbers[i]);
      }
      for (i = 0; i < iAddOpers.Length; i++)
      {
        scAddOpers.Add(iAddOpers[i]);
      }
      for (i = 0; i < iMulOpers.Length; i++)
      {
        scMulOpers.Add(iMulOpers[i]);
      }
      for (i = 0; i < iStrOpers.Length; i++)
      {
        scStrOpers.Add(iStrOpers[i]);
      }
      for (i = 0; i < iStrOpers.Length; i++)
      {
        scStrConst.Add(iStrOpers[i]);
      }
      for (i = 0; i < iPowOpers.Length; i++)
      {
        scPowOpers.Add(iPowOpers[i]);
      }
      #endregion

      scPriority.Clear();
      scPriority.Add(1,iPowOpers);
      scPriority.Add(2,iMulOpers);
      scPriority.Add(3,iAddOpers);
      scPriority.Add(4,iStrOpers);
      scPriority.Add(5,iStrConst);
      scPriority.Add(6,iNumbers);
    }
  }
}
