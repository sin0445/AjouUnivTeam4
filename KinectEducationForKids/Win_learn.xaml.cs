using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectEducationForKids
{
    /// <summary>
    /// Form_learning.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_learn : UserControl
    {
        public event EventHandler<EventArgs> LearnCloseHandler;

        MainWindow mainwindow;

        public Win_learn(MainWindow win)
        {
            InitializeComponent();

            mainwindow = win;

            this.img_main.Width = mainwindow.window.Width / 3;
            this.grid.Width = mainwindow.window.Width - this.img_main.Width;
            this.grid.HorizontalAlignment = HorizontalAlignment.Right;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.LearnCloseHandler(this, new EventArgs());
        }
    }
}
