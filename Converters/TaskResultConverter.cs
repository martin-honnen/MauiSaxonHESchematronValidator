using System.Globalization;


namespace MauiSaxonHESchematronValidator.Converters
{
    public class TaskResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Task<string> task)
            {
                return task.Status == TaskStatus.RanToCompletion ? task.Result : default;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
