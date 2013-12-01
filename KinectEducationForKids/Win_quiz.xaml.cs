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
using System.Windows.Threading;
using Microsoft.Kinect;

namespace KinectEducationForKids
{
    /// <summary>
    /// Win_quiz.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_quiz : UserControl
    {
        public event EventHandler<EventArgs> QuizCloseHandler;

        #region MemberVariables
        private MainWindow _mainWindow;

        private Win_quiz_content _win_quiz_content; 

        private Grid _layoutRoot;
        private Skeleton[] _Skeletons;
        private KinectSensor _KinectDevice;
        private KinectController _KinectController;

        private UIElement _lastElement;
        private int _ticks;
        private DispatcherTimer _timer;
        private const int _hoverTime = 20;
        #endregion MemberVariables

        public Win_quiz(MainWindow win, KinectController control)
        {
            InitializeComponent();

            this._mainWindow = win;
            this._layoutRoot = this._mainWindow.LayoutRoot;
            this._KinectController = control;

            this.Loaded += (s, e) => { InitQuizWindow(); };
            this.Unloaded += (s, e) => { UninitQuizWindow(); };
        }

        private void InitQuizWindow()
        {
            this._KinectDevice = this._KinectController._KinectDevice;
            this._KinectDevice.SkeletonFrameReady += QuizWindow_SkeletonFrameReady;
            this._Skeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
        }

        private void UninitQuizWindow()
        {
            this._KinectDevice.SkeletonFrameReady -= QuizWindow_SkeletonFrameReady;
            this._Skeletons = null;
        }

        private void QuizWindow_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (this.IsEnabled)
            {
                using (SkeletonFrame frame = e.OpenSkeletonFrame())
                {
                    if (frame != null)
                    {
                        if (this.IsEnabled)
                        {
                            frame.CopySkeletonDataTo(this._Skeletons);

                            /* 여기서 처리해야 할 일
                             * 기본적으로 Mainwindow에서 처리하는 방식과 동일
                             * PrimarySkeleton을 찾은 후 PrimaryHand를 찾는다.
                             * 그리고 사용자의 hand joint로부터 Point를 얻어 View 상의 Point로 변환 후
                             * 각 element로 translatepoint후 hit test을 수행
                             * 이 때도 hovering 방식을 차용하여 dispatcher timer를 이용, 약 3초 정도의 시간을 둔 후
                             * 3초 이상 버튼 위에 올라가져 있는 경우 이전 Window로 복귀하는 테스트를 수행한다.
                             */

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
            }
        }

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

            point.X = (int)(dp.X * this._layoutRoot.RenderSize.Width / this._KinectDevice.DepthStream.FrameWidth);
            point.Y = (int)(dp.Y * this._layoutRoot.RenderSize.Height / this._KinectDevice.DepthStream.FrameHeight);

            return new Point(point.X, point.Y);
        }

        private IInputElement GetHitImage(Joint hand, UIElement target)
        {
            Point targetPoint = GetJointPoint(hand);
            targetPoint = this._layoutRoot.TranslatePoint(targetPoint, target);
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
                point.X = (int)((point.X * this._layoutRoot.ActualWidth / this._KinectDevice.DepthStream.FrameWidth) - (HandCursorElement.ActualWidth / 2.0));
                point.Y = (int)((point.Y * this._layoutRoot.ActualHeight / this._KinectDevice.DepthStream.FrameHeight) - (HandCursorElement.ActualHeight / 2.0));

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
            if ((element = (UIElement)GetHitImage(hand, btn_animal)) != null)         //StartBtn을 클릭하는 로직
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

                        this.btn_animal_Click(btn_animal, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    if (this._timer != null)
                        RemoveTimer();

                    CreateTimer();
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_fruit)) != null)
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

                        this.btn_fruit_Click(btn_fruit, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    if (this._timer != null)
                        RemoveTimer();

                    CreateTimer();
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_back)) != null)     //ExitBtn을 클릭하는 로직
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
                        btn_back_Click(btn_back, new RoutedEventArgs());
                        //프로그램 종료
                    }
                }
                else                         //새롭게 ExitBtn에 손을 댄 경우
                {
                    if (this._timer != null)            //만일 타이머가 기존에 존재하는 경우 이를 제거한후
                    {
                        RemoveTimer();
                    }
                    CreateTimer();                      //다시 타이머를 생성한다
                }
                _lastElement = element;                 //그리고 이전 element에 ExitBtn을 등록
            }
            else                                        //GameStartBtn이나 ExitBtn이 아닌 다른 부분에 손이 닿아져 있는 경우
            {
                if (this._lastElement != null)          //lastElement를 제거
                    this._lastElement = null;

                if (this._timer != null)                //타이머가 있는 경우에도 이를 제거
                {
                    RemoveTimer();
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

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.QuizCloseHandler(this, new EventArgs());
        }

        private void btn_animal_Click(object sender, RoutedEventArgs e)
        {
            this._win_quiz_content = new Win_quiz_content(this._mainWindow, this._KinectController, QuizElementListLibrary.QUIZTYPE.ANIMAL);
            this._win_quiz_content.QuizContentCloseHandler += QuizContentClose;
            this.view_quiz.Visibility = Visibility.Hidden;
            _mainWindow.LayoutRoot.Children.Add(_win_quiz_content);
        }

        private void btn_fruit_Click(object sender, RoutedEventArgs e)
        {
            this._win_quiz_content = new Win_quiz_content(this._mainWindow, this._KinectController, QuizElementListLibrary.QUIZTYPE.FRUIT);
            this._win_quiz_content.QuizContentCloseHandler += QuizContentClose;
            this.view_quiz.Visibility = Visibility.Hidden;
            _mainWindow.LayoutRoot.Children.Add(_win_quiz_content);
        }

        public void QuizContentClose(object sender, EventArgs e)
        {
            this.view_quiz.Visibility = Visibility.Visible;
            _mainWindow.LayoutRoot.Children.Remove(this._win_quiz_content);
            this._win_quiz_content = null;
        }
    }
}
