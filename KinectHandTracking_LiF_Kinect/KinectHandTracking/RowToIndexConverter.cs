using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace KinectHandTracking
{
    public class RowToIndexConvertor : MarkupExtension, IValueConverter
    {
        static RowToIndexConvertor _converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridRow row = value as DataGridRow;
            if (row != null)
                return row.GetIndex() + 1;
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null) _converter = new RowToIndexConvertor();
            return _converter;
        }

        public RowToIndexConvertor()
        {
        }
    }
}
