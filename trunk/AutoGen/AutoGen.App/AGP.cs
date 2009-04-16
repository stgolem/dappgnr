using System;
using System.Collections.Generic;
using System.Text;
using AutoGen.I;

namespace AutoGen.App
{
    public class AutoGenParameters : IAutoGenParameters
    {
        private int _CountInVariant;
        private int _Variants;
        private bool _NeedAnswer;
        private string _TaskName;

        public int CountInVariant
        {
            get { return _CountInVariant; }
            set { _CountInVariant = value; }
        }

        public int Variants
        {
            get { return _Variants; }
            set { _Variants = value; }
        }

        public bool NeedAnswer
        {
            get { return _NeedAnswer; }
            set { _NeedAnswer = value; }
        }

        public string TaskName
        {
            get { return _TaskName; }
            set { _TaskName = value; }
        }

        public AutoGenParameters(int countInVariant, int variants, bool needAnswer)
        {
            CountInVariant = countInVariant;
            Variants = variants;
            NeedAnswer = needAnswer;
        }

        public AutoGenParameters()
        {
            CountInVariant = 1;
            Variants = 1;
            NeedAnswer = true;
        }
    }
}
