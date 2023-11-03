using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TLCViewer
{
    /// <summary>
    /// HorizontalStack.xaml の相互作用ロジック
    /// </summary>
    public partial class HorizontalStack : UserControl
    {
        int colmns = 1;
        Image[]? thumbnails;
        Image[]? pictures;

        public HorizontalStack()
        {
            InitializeComponent();
        }

        public HorizontalStack(int colmns, Image[] thumbnails, Image[] pictures)
        {
            this.colmns = colmns;
            this.thumbnails = thumbnails;
            this.pictures = pictures;

            for (int i = 0; i < colmns; ++i)
            {
                var item = new ThumbButton();
                stack.Children.Add(item);
            }

            InitializeComponent();
        }
    }
}
