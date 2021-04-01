using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CueGen.Analysis
{
    internal class LengthConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            return System.Convert.ToInt32(value) - (int)parameter;
        }

        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            return System.Convert.ToInt32(value) + (int)parameter;
        }
    }
}
