using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace KinectHandTracking
{
    public class RowToIndexConvertor : MarkupExtension, IValueConverter
    {
        private static RowToIndexConvertor _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var row = value as DataGridRow;
            if (row != null)
                return row.GetIndex() + 1;
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new RowToIndexConvertor());
        }
    }
}