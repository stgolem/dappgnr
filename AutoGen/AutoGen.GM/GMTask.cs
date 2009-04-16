using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AutoGen.GM
{
    [Serializable]
    public class GMTask
    {
        private GMPropList table = null;
        private string template = "";
        private string output = "";
        private string prefix = "{$";
        private string suffix = "#}";
        private string answer = "ANSWR";
        private string lookfor = "WTS";

        public GMPropList Table
        {
            get { return table; }
            set { table = value; }
        }

        public string Template
        {
            get { return template; }
            set { template = value; }
        }

        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        public string Suffix
        {
            get { return suffix; }
            set { suffix = value; }
        }

        public string Answer
        {
            get { return answer; }
            set { answer = value; }
        }

        public string Lookfor
        {
            get { return lookfor; }
            set { lookfor = value; }
        }

        public GMTask()
        {
            table = new GMPropList();
        }
    }

    #region Базовый класс строки в таблице

    /// <summary>
    /// Базовый класс строки в таблице соответствия
    /// </summary>
    [Serializable]
    public class GMTaskProp
    {
        private int btpropNumber = 0;
        private string btpropView = "";
        private string btpropName = "";
        private double btpropStep = 1;
        private double btpropMin = 0;
        private double btpropMax = 0;
        private double btpropKf = 1;
        private double btpropAcc = 1;
        private string btpropMesName = "";
        private string btpropSolveString = "";
        private bool btpropCanGenerate = false;

        /// <summary>
        /// Строковое представление параметра.
        /// </summary>
        public string View
        {
            get
            { return btpropView; }
            set
            { btpropView = value; }
        }
        /// <summary>
        /// Имя параметра в задаче.
        /// </summary>
        public string Name
        {
            get
            { return btpropName; }
            set
            { btpropName = value; }
        }
        /// <summary>
        /// Минимальное значение
        /// </summary>
        public double MinValue
        {
            get
            { return btpropMin; }
            set
            { btpropMin = value; }
        }
        /// <summary>
        /// Максимальное значение
        /// </summary>
        public double MaxValue
        {
            get
            { return btpropMax; }
            set
            { btpropMax = value; }
        }
        /// <summary>
        /// Шаг при генерации
        /// </summary>
        public double Step
        {
            get { return btpropStep; }
            set { btpropStep = value; }
        }
        /// <summary>
        /// Коэффициент пересчета.
        /// </summary>
        public double Koeff
        {
            get { return btpropKf; }
            set { btpropKf = value; }
        }
        /// <summary>
        /// Точность при создании
        /// </summary>
        public double Accuracy
        {
            get { return btpropAcc; }
            set { btpropAcc = value; }
        }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string Measure
        {
            get { return btpropMesName; }
            set { btpropMesName = value; }
        }
        /// <summary>
        /// Функция зависимости
        /// </summary>
        public string SolveString
        {
            get
            { return btpropSolveString; }
            set
            { btpropSolveString = value; }
        }
        public int Number
        {
            get { return btpropNumber; }
            set { btpropNumber = value; }
        }
        /// <summary>
        /// Можно ли использовать как искомую
        /// </summary>
        public bool CanGenerate
        {
            get
            { return btpropCanGenerate; }
            set
            { btpropCanGenerate = value; }
        }
    }

    [Serializable]
    public class GMPropList : BindingList<GMTaskProp>
    {
        public GMPropList(IList<GMTaskProp> list) : base(list)
        {
            AllowNew = true;
        }

        public GMPropList()
        {
            AllowNew = true;
        }

        public GMPropList GetCanGenerate()
        {
            List<GMTaskProp> list = new List<GMTaskProp>(Items);
            return new GMPropList(list.FindAll(delegate(GMTaskProp prop)
                                                   {
                                                       return prop.CanGenerate;
                                                   }));
        }
    }

    #endregion
}
