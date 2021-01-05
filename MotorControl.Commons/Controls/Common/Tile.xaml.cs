using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorControl.Commons.Controls.Common
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Tile));
        public static readonly DependencyProperty NeighbourTitleTemplateProperty = DependencyProperty.Register(nameof(NeighbourTitleTemplate), typeof(object), typeof(Tile));

        public Tile()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => GetValue(TitleProperty) as string;
            set => SetValue(TitleProperty, value);
        }

        public object NeighbourTitleTemplate
        {
            get => GetValue(NeighbourTitleTemplateProperty);
            set => SetValue(NeighbourTitleTemplateProperty, value);
        }
    }
}
