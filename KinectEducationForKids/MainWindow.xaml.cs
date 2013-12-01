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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.IO;
using Microsoft.Kinect;
using System.Windows.Threading;

namespace KinectEducationForKids
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Member Variables
        private KinectController _KinectController;
        private KinectSensor _KinectDevice;
        private Skeleton[] _Skeletons;

        private DispatcherTimer _timer; 
        private const int _hoverTime = 20;
        private int _ticks;
        private UIElement _lastElement;
        private List<Button> _animatedBtnList;
        private SoundManager _soundManager;

        private Win_learn win_learn;
        private Win_quiz win_quiz;
        #endregion Member Variables

        public MainWindow()
        {
            InitializeComponent();
            _KinectController = new KinectController();
            this.Loaded += (s, e) => { SettingKinectDevice(); };
            this.Unloaded += (s, e) => { UninitializeKinectDevice(); };
        }
        #region CoreMethods
        private void SettingKinectDevice()
        {
            this._KinectController.DiscoverKinectSensor();
            this._KinectDevice = this._KinectController._KinectDevice;
            this._Skeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
            this._KinectDevice.SkeletonFrameReady += this.MainWindow_SkeletonFrameReady;
            this._soundManager = new SoundManager();
            this._animatedBtnList = new List<Button>();
        }

        private void UninitializeKinectDevice()
        {
            this._KinectDevice.SkeletonFrameReady -= this.MainWindow_SkeletonFrameReady;
            this._KinectController.UninitializeKinectSensor();
            this._KinectDevice = null;
            this._KinectController = null;
        }
        private void MainWindow_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    frame.CopySkeletonDataTo(this._Skeletons);
                    Skeleton skeleton = GetPrimarySkeleton(this._Skeletons);

                    if (skeleton == null)
                    {
                        //만일 인식된 스켈레톤이 없는 경우 손 커서를 숨기게 된다.
                        HandCursorElement.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        Joint primaryHand = GetPrimaryHand(skeleton);
                        TrackHand(primaryHand);
                        TrackHandLocation(primaryHand);
                        //손에 따른 UI 변경 메소드 호출
                    }
                }
            }
        }
        #endregion CoreMethods

        #region GettingMethods
        private static Skeleton GetPrimarySkeleton(Skeleton[] skeletons)
        {
            //만일 한명의 스켈레톤만 인식되는 경우 해당 스켈레톤을, 여러명의 스켈레톤이 인식되는 경우 가장 가까운 스켈레톤을 선택
            Skeleton skeleton = null;
            if (skeletons != null)
            {
                for (int i = 0; i < skeletons.Length; i++)
                {
                    if (skeletons[i].TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (skeleton == null)
                        {
                            skeleton = skeletons[i];
                        }
                        else
                        {
                            //이미 저장된 스켈레톤 보다 새로 계산하는 스켈레톤의 거리가 가까울 때 더 가까운 스켈레톤을 사용하도록 리턴할 스켈레톤을 변경하여 준다
                            if (skeleton.Position.Z > skeletons[i].Position.Z)
                            {
                                skeleton = skeletons[i];
                            }
                        }
                    }
                }
            }
            return skeleton;
        }

        private static Joint GetPrimaryHand(Skeleton skeleton)
        {
            Joint primaryHand = new Joint();

            if (skeleton != null)
            {
                primaryHand = skeleton.Joints[JointType.HandLeft];
                Joint righHand = skeleton.Joints[JointType.HandRight];

                if (righHand.TrackingState != JointTrackingState.NotTracked)
                {
                    if (primaryHand.TrackingState == JointTrackingState.NotTracked)
                    {
                        primaryHand = righHand;
                    }
                    else
                    {
                        if (primaryHand.Position.Z > righHand.Position.Z)
                        {
                            primaryHand = righHand;
                        }
                    }
                }
            }
            return primaryHand;
        }

        private Point GetJointPoint(Joint joint)
        {
            DepthImagePoint dp = this._KinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, this._KinectDevice.DepthStream.Format);
            Point point = new Point();

            point.X = (int)(dp.X * this.LayoutRoot.RenderSize.Width / this._KinectDevice.DepthStream.FrameWidth);
            point.Y = (int)(dp.Y * this.LayoutRoot.RenderSize.Height / this._KinectDevice.DepthStream.FrameHeight);
         
            return new Point(point.X, point.Y);
        }

        private IInputElement GetHitImage(Joint hand, UIElement target)
        {
            Point targetPoint = GetJointPoint(hand);
            targetPoint = Main.TranslatePoint(targetPoint, target);
            return target.InputHitTest(targetPoint);
        }
        #endregion GettingMethods

        #region TrackingMethods
        private void TrackHand(Joint hand)
        {
            //손의 Joint를 파라메터로 받아서 손의 위치를 스크린 상에 표시해주기 위한 메서드

            if (hand.TrackingState == JointTrackingState.NotTracked)
            {
                HandCursorElement.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                HandCursorElement.Visibility = System.Windows.Visibility.Visible;

                DepthImagePoint point = this._KinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, this._KinectDevice.DepthStream.Format);
                point.X = (int)((point.X * Main.ActualWidth / this._KinectDevice.DepthStream.FrameWidth) - (HandCursorElement.ActualWidth / 2.0));
                point.Y = (int)((point.Y * Main.ActualHeight / this._KinectDevice.DepthStream.FrameHeight) - (HandCursorElement.ActualHeight / 2.0));

                Canvas.SetLeft(HandCursorElement, point.X);
                Canvas.SetTop(HandCursorElement, point.Y);

              
                if (hand.JointType == JointType.HandRight)
                {
                    HandcursorScale.ScaleX = 1;
                }
                else
                {
                    HandcursorScale.ScaleX = -1;
                }
            }
        }

        private void TrackHandLocation(Joint hand)
        {
            UIElement element;
            //손이 버튼 위에 있는 경우
            //계속 버튼위에 있는 경우(Timer 체크 후 일정 시간 이상 지나면 현재 윈도우 hidden 후 메뉴창 add) 
            //새로운 버튼위에 있는 경우(Timer 초기화 후 lastElement에 현재 버튼 등록)

            //손이 버튼위에 없는 경우
            //버튼에서 벗어난 경우(lastElement null, Timer stop후 null)
            //원래 밖에 있었던 경우(그냥 무시)
            
            if ((element = (UIElement)GetHitImage(hand, btn_learn)) != null)         //StartBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //StartBtn에 계속하여 손을 대고 있는 경우
                {
                    if (this._timer == null)
                    {
                        CreateTimer();
                    }
                    else if(this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();
                        
                        this.btn_learn_Click(btn_learn, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    HandMovedNewButton(this._lastElement, btn_learn);
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_quiz)) != null)
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //StartBtn에 계속하여 손을 대고 있는 경우
                {
                    if (this._timer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();

                        this.btn_quiz_Click(btn_quiz, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    HandMovedNewButton(this._lastElement, btn_quiz);

                    //CreateTimer();
                    //ApplyProgressAnimationOnButton(btn_quiz);
                    //this._soundManager.PlayAudio(AudioList.Lists.한글퀴즈);
                    //SetSoundPlayer(this._effectPlayer, Properties.Resources.hand_over);
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_exit)) != null)     //ExitBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //계속해서 ExitBtn에 손을 대고 있는 경우
                {
                    if (this._timer == null)    //만일 타이머가 없으면 생성시킨다
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)     //타이머가 있으나 특정 시간 이상 손을 댄 경우
                    {
                        RemoveTimer();
                        this.btn_exit_Click(btn_learn, new RoutedEventArgs());
                        //프로그램 종료
                    }
                }
                else                         //새롭게 ExitBtn에 손을 댄 경우
                {
                    HandMovedNewButton(this._lastElement, btn_exit);
                }
                _lastElement = element;                 //그리고 이전 element에 ExitBtn을 등록
            }
            else                                        //GameStartBtn이나 ExitBtn이 아닌 다른 부분에 손이 닿아져 있는 경우
            {
                if (this._lastElement != null)          //lastElement를 제거
                    this._lastElement = null;

                if (this._timer != null)
                    RemoveTimer();

                foreach (Button btn in this._animatedBtnList)
                {
                    btn.Background = new SolidColorBrush(Colors.White);
                }
            }
        }
        #endregion TrackingMethods

        #region TimerMethods
        private void CreateTimer()
        {
            this._ticks = 0;
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromSeconds(0.1);
            this._timer.Tick += new EventHandler(OnTimerTick);
            this._timer.Start();
        }

        private void RemoveTimer()
        {
            this._timer.Stop();
            this._timer.Tick -= OnTimerTick;
            this._ticks = 0;
            this._timer = null;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this._ticks++;
        }
        #endregion TimerMethods

        #region AnimationMethods

        private void ApplyProgressAnimationOnButton(Button btn)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.EndPoint = new Point(0, 1);
            brush.StartPoint = new Point(0, 0);
            brush.Opacity = 0.8;
            brush.GradientStops.Add(new GradientStop(Colors.White, 1));
            brush.GradientStops.Add(new GradientStop(Colors.LightPink, 1));
            btn.Background = brush;

            this.RegisterName("GradientStop1", brush.GradientStops[0]);
            this.RegisterName("GradientStop2", brush.GradientStops[1]);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 1.0;
            animation.To = 0.0;
            animation.Duration = TimeSpan.FromSeconds(2.5);

            Storyboard.SetTargetName(animation, "GradientStop1");
            Storyboard.SetTargetProperty(animation, new PropertyPath(GradientStop.OffsetProperty));

            DoubleAnimation animation2 = new DoubleAnimation();
            animation2.From = 1.0;
            animation2.To = 0.0;
            animation2.Duration = TimeSpan.FromSeconds(2.5);

            Storyboard.SetTargetName(animation2, "GradientStop2");
            Storyboard.SetTargetProperty(animation2, new PropertyPath(GradientStop.OffsetProperty));

            Storyboard sb = new Storyboard();
            sb.Children.Add(animation);
            sb.Children.Add(animation2);

            sb.Begin(this);

            this.UnregisterName("GradientStop1");
            this.UnregisterName("GradientStop2");
        }

        #endregion AnimationMethods

        #region ButtonMethods
        private void HandMovedNewButton(UIElement lastElement, Button currentBtn)
        {  
            if (this._timer != null)
                RemoveTimer();
            CreateTimer();

            foreach (Button btn in this._animatedBtnList)
            {
                btn.Background = new SolidColorBrush(Colors.White);
            }

            HandEnterButtonHandler(currentBtn, new RoutedEventArgs());
            this._animatedBtnList.Add(currentBtn);

            if(currentBtn.Equals(btn_learn))
            {
                this._soundManager.PlayAudio(AudioList.Lists.한글쓰기);
            }
            else if (currentBtn.Equals(btn_quiz))
            {
                this._soundManager.PlayAudio(AudioList.Lists.한글퀴즈);
            }
            else if (currentBtn.Equals(btn_exit))
            {
                this._soundManager.PlayAudio(AudioList.Lists.종료하기);
            }
        }

        private void HandEnterButtonHandler(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ApplyProgressAnimationOnButton(btn);
        }

        private void HandLeaveButtonHandler(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Background = new SolidColorBrush(Colors.White);
        }

        private void btn_learn_Click(object sender, RoutedEventArgs e)
        {
            this.win_learn = new Win_learn(this, this._KinectController, this._soundManager);
            this.win_learn.LearnCloseHandler += LearnClose;
            Main.Visibility = Visibility.Hidden;
            LayoutRoot.Children.Add(win_learn);
        }

        public void LearnClose(object sender, EventArgs e)
        {
            Main.Visibility = Visibility.Visible;
            LayoutRoot.Children.Remove(this.win_learn);
            this.win_learn = null;
        }

        private void btn_quiz_Click(object sender, RoutedEventArgs e)
        {
            this.win_quiz = new Win_quiz(this, this._KinectController, this._soundManager);
            this.win_quiz.QuizCloseHandler += QuizClose;
            Main.Visibility = Visibility.Hidden;
            LayoutRoot.Children.Add(win_quiz);
        }

        public void QuizClose(object sender, EventArgs e)
        {
            Main.Visibility = Visibility.Visible;
            LayoutRoot.Children.Remove(this.win_quiz);
            this.win_quiz = null;
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            LayoutRoot.Children.Clear();
            Close();
        }
        #endregion ButtonMethods
    }
}