using System.Windows;

namespace PAWSwords.Style
{
    public sealed class CustomBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public CustomBooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed)
        { }
    }
}
