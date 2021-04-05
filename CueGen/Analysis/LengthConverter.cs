using BinarySerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CueGen.Analysis
{
    internal class LengthConverter : IValueConverter
    {
        public object Convert(object value, object parameter, BinarySerializationContext context)
        {
            return System.Convert.ToInt32(value) - (int)parameter;
        }

        [ExcludeFromCodeCoverage]
        public object ConvertBack(object value, object parameter, BinarySerializationContext context)
        {
            return System.Convert.ToInt32(value) + (int)parameter;
        }
    }
}
