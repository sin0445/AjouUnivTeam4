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

        //private MenuWindow _menuWindow;
        private Win_learn win_learn;
        private Win_quiz win_quiz;
        #endregion Member Variables

        public MainWindow()
        {
            InitializeComponent();
            _KinectController = new KinectController();
            this.Loaded += (s, e) => { SettingKinectDevice(); };
            this.Unloaded += (s, e) => { UninitializeKinectDevice(); };

            this.img_main.Width = this.window.Width / 3;
            this.grid.Width = this.window.Width - this.img_main.Width;
            this.grid.HorizontalAlignment = HorizontalAlignment.Right;
        }
        #region CoreMethods
        private void SettingKinectDevice()
        {
            this._KinectController.DiscoverKinectSensor();
            this._KinectDevice = this._KinectController._KinectDevice;
            this._Skeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
            this._KinectDevice.SkeletonFrameReady += this.MainWindow_SkeletonFrameReady;
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
                    if (this._timer != null)
                        RemoveTimer();

                    CreateTimer();
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
                    if (this._timer != null)
                        RemoveTimer();

                    CreateTimer();
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
                        LayoutRoot.Children.Clear();
                        Close();
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
        //private void MenuClose(object sender, EventArgs e)
        //{
        //    LayoutRoot.Children.Remove(this._menuWindow);
        //    Main.Visibility = Visibility.Visible;
        //    this._menuWindow = null;
        //}

        private void btn_learn_Click(object sender, RoutedEventArgs e)
        {
            this.win_learn = new Win_learn(this, this._KinectController);
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
            this.win_quiz = new Win_quiz(this, this._KinectController);
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
    }
}


        //private void TrackPuzzle(SkeletonPoint position)
        //{
        //    /* 향후 추가되야 할 로직
        //     * 손이 점 따라 그리기 Board 상에 존재하지 않을 경우 손까지 선이 따라가는 것을 방지시켜줘야 할 것이다.
        //     * 그렇기 위해서 우선 손 위치가 게임 보드 상에 존재하는지 아니면 다른 버튼 보드 상에 위치하는지를 먼저 Hittest후
        //     * 게임 보드상에 존재하는 경우 그대로 게임을 진행하며 다른 버튼 보드 상에 존재하는 경우 TrackHandLocation 메소드를 호출하여
        //     * CrayonElement가 게임 보드 바깥으로 따라 그려지는 것을 방지해야 할 것으로 예상된다.
        //     */
        //    if (this._CurrentStroke == null)
        //    {
        //        this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[0];
        //    }

        //    if (this._StrokeDotIndex == this._CurrentStroke.Count && this._StrokeIndex == this._CurrentCharacter.StrokeDotIndex.Count)
        //    {
        //        //모든 점이 클릭된 상태
        //    }
        //    else
        //    {
        //        Point nextDot;              //다음 선택될 점
        //        int lastPoint;

        //        nextDot = this._CurrentCharacter.DotList[this._CurrentStroke[this._StrokeDotIndex]];

        //        DepthImagePoint point = this.KinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(position, DepthImageFormat.Resolution640x480Fps30);
        //        point.X = (int)(point.X * LayoutRoot.ActualWidth / this.KinectDevice.DepthStream.FrameWidth);
        //        point.Y = (int)(point.Y * LayoutRoot.ActualHeight / this.KinectDevice.DepthStream.FrameHeight);
        //        Point handPoint = new Point(point.X, point.Y);

        //        Point dotDiff = new Point(nextDot.X - handPoint.X, nextDot.Y - handPoint.Y);
        //        double length = Math.Sqrt(dotDiff.X * dotDiff.X + dotDiff.Y * dotDiff.Y);

        //        if (this._StrokeDotIndex <= 0)
        //        {
        //            //해당 획의 첫 Dot일 경우 0을 그냥 저장시킴
        //            lastPoint = this._StrokeDotIndex;
        //        }
        //        else
        //        {
        //            //해당 획의 첫 Dot이 아닐 경우 현재 그려진 CrayonElement 중 가장 나중 선의 끝 포인트 -1 의 위치를 받아옴
        //            //이 때 -1을 하는 이유는 가장 Crayon의 가장 끝 포인트는 손의 위치를 의미한다. 그렇기 때문에 -1을 하면 손과 직접적으로 연결된
        //            //Dot의 Point를 갖고 오게 된다.
        //            lastPoint = this.CrayonElement.Points.Count - 1;
        //        }

        //        if (length < 25)            //손의 위치가 점으로부터 일정 거리내에 존재하는 경우
        //        {
        //            if (lastPoint > 0)
        //            {
        //                this.CrayonElement.Points.RemoveAt(lastPoint);
        //            }

        //            this.CrayonElement.Points.Add(new Point(nextDot.X, nextDot.Y));
        //            this.CrayonElement.Points.Add(new Point(nextDot.X, nextDot.Y));

        //            this._StrokeDotIndex++;
        //            if (this._StrokeDotIndex == this._CurrentStroke.Count) //해당 획을 전부 다 그렸을 경우
        //            {
        //                this._StrokeIndex++;
        //                if (this._StrokeIndex == this._CurrentCharacter.StrokeDotIndex.Count) // 모든 획 까지 다 그린 경우
        //                {
        //                    //아무것도 안한다
        //                }
        //                else    //아직 안그린 획이 존재하는 경우
        //                {
        //                    this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[this._StrokeIndex];     //획을 갱신하고
        //                    this._StrokeDotIndex = 0;       //점 카운트를 초기화
        //                }
        //            }
        //        }
        //        else        //손의 위치가 점에서 멀리 떨어져 있을 경우
        //        {
        //            if (lastPoint > 0)      //이는 이미 선택된 점이 존재하는 경우. 즉, 점과 손이 이미 CrayonElement로 연결되어 있다.
        //            {
        //                //그러므로 손이 움직인 위치로 CrayonElement를 갱신해 줄 필요가 있다.
        //                Point lineEndPoint = this.CrayonElement.Points[lastPoint];
        //                this.CrayonElement.Points.RemoveAt(lastPoint);
        //                lineEndPoint.X = handPoint.X;
        //                lineEndPoint.Y = handPoint.Y;
        //                this.CrayonElement.Points.Add(lineEndPoint);
        //            }
        //        }
        //    }
        //}

        //private void MakePuzzle()
        //{
        //    /*
        //     * 이 부분은 로직이 크게 바뀌어야 할 부분으로 예상된다
        //     * 우선 CharacterListLibrary로부터 글자 List를 받아온 후
        //     * 기본 index 값들을 초기화 시킨다.
        //     * 그 이후 일단은 가장 처음 글자를 _CurrentCharacter 값에 셋팅한다.
        //     * 그리고 다음 문제 버튼이 클릭되거나 게임이 클리어 되었을 경우 다음 글자를 _CurrentCharacter에 셋팅한 후
        //     * 게임을 반복하는 로직이 필요할 것이라고 예상된다.
        //     */

        //    this._CurrentCharacter = new Digeuk();
        //    this._StrokeDotIndex = 0;
        //    this._StrokeIndex = 0;
        //}

        //private void DrawPuzzle(CharacterBase puzzle)
        //{
        //    PuzzleBoardElement.Children.Clear();

        //    if (puzzle != null)
        //    {
        //        for (int i = 0; i < puzzle.DotList.Count; i++)
        //        {
        //            //점을 만들어주는 구문
        //            Grid dotContainer = new Grid();
        //            dotContainer.Width = 50;
        //            dotContainer.Height = 50;
        //            dotContainer.Children.Add(new Ellipse() { Fill = Brushes.Gray });

        //            //각 점에 표시될 숫자 입력
        //            TextBlock dotLabel = new TextBlock();
        //            dotLabel.Text = (i + 1).ToString();
        //            dotLabel.Foreground = Brushes.White;
        //            dotLabel.FontSize = 24;
        //            dotLabel.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        //            dotLabel.VerticalAlignment = System.Windows.VerticalAlignment.Center;
        //            dotContainer.Children.Add(dotLabel);

        //            //Poisition the UI element centered on the dot point
        //            Canvas.SetTop(dotContainer, puzzle.DotList[i].Y - (dotContainer.Height / 2));
        //            Canvas.SetLeft(dotContainer, puzzle.DotList[i].X - (dotContainer.Width / 2));
        //            PuzzleBoardElement.Children.Add(dotContainer);
        //        }
        //    }
        //}