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
    /// Win_quiz_ani.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_quiz_ani : UserControl
    {
        public event EventHandler<EventArgs> AniCloseHandler;

        private MainWindow mainwindow;

        private List<string> btn_path = new List<string>();
        private List<string> img_path = new List<string>();

        private string[] list = { "elephant", "giraffe", "hippo", "lion", "monkey", "penguin", "pig", "rabbit", "racoon" };

        private int prev_num = -1;

        private int num_quiz = -1;
        private int num_answer = -1;

        private int wrong_num1 = -1;
        private int wrong_num2 = -1;
        private int wrong_num3 = -1;

        public Win_quiz_ani(MainWindow win)
        {
            InitializeComponent();

            mainwindow = win;

            // image / button 추가
            function_add();

            // 문제 출제
            making_quiz();

            // layout 크기 수정
            edit_layout();
        }

        private void edit_layout()
        {
            this.panel2.Width = mainwindow.window.Width;
            this.panel2.Height = mainwindow.window.Height * 0.66;

            this.img_quiz.Width = this.panel2.Width * 0.66 - 60;
            this.img_quiz.Height = this.panel2.Height - 60;

            this.btn_back.Width = this.panel2.Width - this.img_quiz.Width - 120;
            this.btn_back.Height = this.panel2.Height * 0.5 - 60;

            this.btn_next.Width = this.panel2.Width - this.img_quiz.Width - 120;
            this.btn_next.Height = this.panel2.Height * 0.5 - 60;

            this.grid.Width = mainwindow.window.Width;
            this.grid.Height = mainwindow.window.Height - this.panel2.Height;
        }

        private void function_add()
        {
            for (int i = 0; i < 9; i++)
            {
                string name = list[i];
                btn_path.Add("btn_" + name + ".png");
                img_path.Add("img_" + name + ".jpg");
            }
        }

        private void making_quiz()
        {
            generating();

            ImageSourceConverter imgConv = new ImageSourceConverter();

            img_quiz.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + img_path[num_quiz].ToString());

            switch (num_answer)
            {
                case 0:
                    {
                        quiz1.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[num_quiz].ToString());
                        quiz2.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num1].ToString());
                        quiz3.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num2].ToString());
                        quiz4.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num3].ToString());
                        break;
                    }
                case 1:
                    {
                        quiz1.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num1].ToString());
                        quiz2.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[num_quiz].ToString());
                        quiz3.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num2].ToString());
                        quiz4.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num3].ToString());
                        break;
                    }
                case 2:
                    {
                        quiz1.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num1].ToString());
                        quiz2.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num2].ToString());
                        quiz3.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[num_quiz].ToString());
                        quiz4.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num3].ToString());
                        break;
                    }
                case 3:
                    {
                        quiz1.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num1].ToString());
                        quiz2.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num2].ToString());
                        quiz3.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[wrong_num3].ToString());
                        quiz4.ImageSource = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + btn_path[num_quiz].ToString());
                        break;
                    }
            }
        }

        private void generating()
        {
            int max = list.Length;

            System.Random random = new System.Random();

            bool flag = true;

            do
            {
                num_quiz = random.Next(0, max);

                if (num_quiz != prev_num)
                {
                    flag = false;
                }
            }
            while (flag);

            num_answer = random.Next(0, 4);

            flag = true;
            do
            {
                wrong_num1 = random.Next(0, max);

                if ((wrong_num1 != num_quiz) && (wrong_num1 != wrong_num2) && (wrong_num1 != wrong_num3))
                {
                    flag = false;
                }
            }
            while (flag);

            flag = true;
            do
            {
                wrong_num2 = random.Next(0, max);

                if ((wrong_num2 != num_quiz) && (wrong_num2 != wrong_num1) && (wrong_num2 != wrong_num3))
                {
                    flag = false;
                }
            }
            while (flag);

            flag = true;
            do
            {
                wrong_num3 = random.Next(0, max);

                if ((wrong_num3 != num_quiz) && (wrong_num3 != wrong_num2) && (wrong_num3 != wrong_num2))
                {
                    flag = false;
                }
            }
            while (flag);
        }


        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.AniCloseHandler(this, new EventArgs());
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            next_quiz();
        }

        private void next_quiz()
        {
            prev_num = num_quiz;

            // image / button 추가
            function_add();

            // 문제 출제
            making_quiz();

            // layout 크기 수정
            edit_layout();
        }
        private void btn_1_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(0);
        }

        private void btn_2_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(1);
        }

        private void btn_3_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(2);
        }

        private void btn_4_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(3);
        }

        private void checking_answer(int user_answer)
        {
            if (user_answer == num_answer)
            {
                next_quiz();
            }
        }
    }
}
