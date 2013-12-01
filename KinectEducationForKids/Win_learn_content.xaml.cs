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
using System.Windows.Media.Animation;
using Microsoft.Kinect;

namespace KinectEducationForKids
{
    /// <summary>
    /// Win_learn_content.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_learn_content : UserControl
    {
        public event EventHandler<EventArgs> LearnContentCloseHandler;

        #region MemberVariables
        private MainWindow _mainWindow;
        private KinectController _KinectController;
        private KinectSensor _KinectDevice;
        private Grid _layoutRoot;
        private Skeleton[] _Skeletons;

        private UIElement _lastElement;
        private int _ticks;
        private DispatcherTimer _timer;
        private const int _hoverTime = 20;
        private SoundManager _soundManager;
        private List<Button> _animatedBtnList;

        private List<CharacterBase> _CharacterList;
        private int _CharacterIndex;                //글자 리스트 중 몇 번째 글자를 제공중인지를 알려주는 인덱스
        private int _StrokeDotIndex;                //현재 획수에 몇 번째 점에 손을 대어야 하는지를 알려주는 인덱스
        private int _StrokeIndex;                   //그려야할 획수를 나타내주는 인덱스
        private CharacterBase _CurrentCharacter;
        private List<int> _CurrentStroke;
        private Polyline _CrayonElement;
        private Brush[] _BrushPallete;
        private Brush _brush;
        private Ellipse _NextDot;
        private Random _BrushPicker;

        private string _img_path;
        #endregion MemberVariables

        #region Constructor
        public Win_learn_content(MainWindow win, KinectController control, CharacterListLibrary.CHARACTERTYPE type, SoundManager sound)
        {
            InitializeComponent();

            this._mainWindow = win;
            this._layoutRoot = this._mainWindow.LayoutRoot;
            this._KinectController = control;
            this._soundManager = sound;
            this._animatedBtnList = new List<Button>();
            PrepareLearnContent(type);

            this.Loaded += (s, e) => { InitLearnContentWindow(); };
            this.Unloaded += (s, e) => { UninitLearnContentWindow(); };
        }

        private void InitLearnContentWindow()
        {
            this._KinectDevice = this._KinectController._KinectDevice;
            this._KinectDevice.SkeletonFrameReady += LearnContentWindow_SkeletonFrameReady;
            this._Skeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
        }
        private void UninitLearnContentWindow()
        {
            this._KinectDevice.SkeletonFrameReady -= LearnContentWindow_SkeletonFrameReady;
            this._Skeletons = null;
        }

        #endregion Constructor

        private void LearnContentWindow_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
            Point targetPoint = GetJointPoint(hand);
            double panelWidth = PuzzleBoardElement.ActualWidth;
            double panelHeight = PuzzleBoardElement.ActualHeight;

            if (targetPoint.X < panelWidth && targetPoint.Y < panelHeight)
            {
                TrackHandLocationOnPuzzle(hand.Position);
            }
            else
            {
                TrackHandLocationOnMenu(hand);
            }
        }

        private void TrackHandLocationOnMenu(Joint hand)
        {
            UIElement element;
            //손이 버튼 위에 있는 경우
            //계속 버튼위에 있는 경우(Timer 체크 후 일정 시간 이상 지나면 현재 윈도우 hidden 후 메뉴창 add) 
            //새로운 버튼위에 있는 경우(Timer 초기화 후 lastElement에 현재 버튼 등록)

            //손이 버튼위에 없는 경우
            //버튼에서 벗어난 경우(lastElement null, Timer stop후 null)
            //원래 밖에 있었던 경우(그냥 무시)
            if ((element = (UIElement)GetHitImage(hand, btn_prev)) != null)
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //btn_next에 계속하여 손을 대고 있는 경우
                {
                    if (this._timer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();
                        btn_prev_Click(btn_next, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    HandMovedNewButton(this._lastElement, btn_prev);
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_next)) != null)
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //btn_next에 계속하여 손을 대고 있는 경우
                {
                    if (this._timer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();
                        btn_next_Click(btn_next, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    HandMovedNewButton(this._lastElement, btn_next);
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
                    HandMovedNewButton(this._lastElement, btn_back);
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

                foreach (Button btn in this._animatedBtnList)
                {
                    btn.Background = new SolidColorBrush(Colors.White);
                }
            }
        }
        #endregion TrackingMethods

        #region PuzzleMethods
        private void TrackHandLocationOnPuzzle(SkeletonPoint position)
        {
            /* 향후 추가되야 할 로직
            * 손이 점 따라 그리기 Board 상에 존재하지 않을 경우 손까지 선이 따라가는 것을 방지시켜줘야 할 것이다.
            * 그렇기 위해서 우선 손 위치가 게임 보드 상에 존재하는지 아니면 다른 버튼 보드 상에 위치하는지를 먼저 Hittest후
            * 게임 보드상에 존재하는 경우 그대로 게임을 진행하며 다른 버튼 보드 상에 존재하는 경우 TrackHandLocation 메소드를 호출하여
            * _CrayonElement가 게임 보드 바깥으로 따라 그려지는 것을 방지해야 할 것으로 예상된다.
            */
            if (this._CurrentStroke == null)
            {
                this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[0];
            }

            if (this._StrokeDotIndex == this._CurrentStroke.Count && this._StrokeIndex == this._CurrentCharacter.StrokeDotIndex.Count)
            {
                //모든 점이 클릭된 상태
                btn_next_Click(btn_next, new RoutedEventArgs());
            }
            else
            {
                Point nextDot;              //다음 선택될 점
                int lastPoint;

                nextDot = this._CurrentCharacter.DotList[this._CurrentStroke[this._StrokeDotIndex]];

                DepthImagePoint point = this._KinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(position, DepthImageFormat.Resolution640x480Fps30);
                point.X = (int)(point.X * _layoutRoot.ActualWidth / this._KinectDevice.DepthStream.FrameWidth);
                point.Y = (int)(point.Y * _layoutRoot.ActualHeight / this._KinectDevice.DepthStream.FrameHeight);
                Point handPoint = new Point(point.X, point.Y);

                Point dotDiff = new Point(nextDot.X - handPoint.X, nextDot.Y - handPoint.Y);
                double length = Math.Sqrt(dotDiff.X * dotDiff.X + dotDiff.Y * dotDiff.Y);

                if (this._StrokeDotIndex <= 0)
                {
                    //해당 획의 첫 Dot일 경우 0을 그냥 저장시킴
                    this._CrayonElement = new Polyline();
                    this._CrayonElement.StrokeThickness = 6;
                    this._CrayonElement.Stroke = this._brush;
                    lastPoint = this._StrokeDotIndex;
                }
                else
                {
                    //해당 획의 첫 Dot이 아닐 경우 현재 그려진 _CrayonElement 중 가장 나중 선의 끝 포인트 -1 의 위치를 받아옴
                    //이 때 -1을 하는 이유는 가장 Crayon의 가장 끝 포인트는 손의 위치를 의미한다. 그렇기 때문에 -1을 하면 손과 직접적으로 연결된
                    //Dot의 Point를 갖고 오게 된다.
                    lastPoint = this._CrayonElement.Points.Count - 1;
                }

                if (length < 25)            //손의 위치가 점으로부터 일정 거리내에 존재하는 경우
                {
                    if (lastPoint > 0)
                    {
                        this._CrayonElement.Points.RemoveAt(lastPoint);
                    }

                    this._CrayonElement.Points.Add(new Point(nextDot.X, nextDot.Y));
                    this._CrayonElement.Points.Add(new Point(nextDot.X, nextDot.Y));

                    this._StrokeDotIndex++;
                    if (this._StrokeDotIndex == this._CurrentStroke.Count) //해당 획을 전부 다 그렸을 경우
                    {
                        this._StrokeIndex++;
                        if (this._StrokeIndex == this._CurrentCharacter.StrokeDotIndex.Count) // 모든 획 까지 다 그린 경우
                        {
                            //아무것도 안한다
                            btn_next_Click(btn_next, new RoutedEventArgs());
                        }
                        else    //아직 안그린 획이 존재하는 경우
                        {
                            this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[this._StrokeIndex];     //획을 갱신하고
                            this._StrokeDotIndex = 0;       //점 카운트를 초기화
                            ExtendNextDotCircle(this._CurrentStroke[this._StrokeDotIndex]);
                        }
                    }
                    else            //현재 획 내에 안 그린 점이 존재하는 경우
                    {
                        /*
                         * 그 다음 점을 크게 만들어 주는 구문
                         */
                        ExtendNextDotCircle(this._CurrentStroke[this._StrokeDotIndex]);
                    }
                }
                else        //손의 위치가 점에서 멀리 떨어져 있을 경우
                {
                    if (lastPoint > 0)      //이는 이미 선택된 점이 존재하는 경우. 즉, 점과 손이 이미 _CrayonElement로 연결되어 있다.
                    {
                        //그러므로 손이 움직인 위치로 _CrayonElement를 갱신해 줄 필요가 있다.
                        Point lineEndPoint = this._CrayonElement.Points[lastPoint];
                        this._CrayonElement.Points.RemoveAt(lastPoint);
                        lineEndPoint.X = handPoint.X;
                        lineEndPoint.Y = handPoint.Y;
                        this._CrayonElement.Points.Add(lineEndPoint);
                    }
                }
                this.PuzzleBoardElement.Children.Remove(this._CrayonElement);
                this.PuzzleBoardElement.Children.Add(this._CrayonElement);
            }
        }

        private void PrepareLearnContent(CharacterListLibrary.CHARACTERTYPE type)
        {
            this._CharacterIndex = 0;
            this._CharacterList = CharacterListLibrary.getCharacters(type);

            this._BrushPallete = new []{Brushes.Black, Brushes.BlueViolet, Brushes.Crimson, Brushes.ForestGreen, Brushes.HotPink, 
                                        Brushes.Khaki, Brushes.Yellow, Brushes.Violet, Brushes.Orange, Brushes.Navy, Brushes.Aqua};
            this._BrushPicker = new Random();
            
            SettingNextPuzzle();
        }

        private void SettingNextPuzzle()
        {
            if (this._CharacterIndex < this._CharacterList.Count)
            {
                this._brush = this._BrushPallete[this._BrushPicker.Next(this._BrushPallete.Length - 1)];

                this._CurrentCharacter = this._CharacterList[this._CharacterIndex++];
                this._StrokeDotIndex = 0;
                this._StrokeIndex = 0;
                this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[this._StrokeIndex];
                DrawCharacterDots(this._CurrentCharacter);

                ExtendNextDotCircle(this._StrokeDotIndex);
            }
        }

        private void SettingPrevPuzzle()
        {
            if (this._CharacterIndex > 1)
            {
                this._brush = this._BrushPallete[this._BrushPicker.Next(this._BrushPallete.Length - 1)];

                this._CurrentCharacter = this._CharacterList[this._CharacterIndex-2];
                this._CharacterIndex--;
                this._StrokeDotIndex = 0;
                this._StrokeIndex = 0;
                this._CurrentStroke = this._CurrentCharacter.StrokeDotIndex[this._StrokeIndex];
                DrawCharacterDots(this._CurrentCharacter);

                ExtendNextDotCircle(this._StrokeDotIndex);
            }
        }

        private void RemoveCurrentPuzzle()
        {
            PuzzleBoardElement.Children.Clear();
            if(this._CrayonElement != null)
                this._CrayonElement.Points.Clear();
        }

        private void DrawCharacterDots(CharacterBase character)
        {
            PuzzleBoardElement.Children.Clear();

            if (character != null)
            {
                for (int i = 0; i < character.DotList.Count; i++)
                {
                    //점을 만들어주는 구문
                    Grid dotContainer = new Grid();
                    dotContainer.Width = 25;
                    dotContainer.Height = 25;
                    dotContainer.Children.Add(new Ellipse() { Fill = Brushes.Gray });

                    //Poisition the UI element centered on the dot point
                    Canvas.SetTop(dotContainer, character.DotList[i].Y - (dotContainer.Height / 2));
                    Canvas.SetLeft(dotContainer, character.DotList[i].X - (dotContainer.Width / 2));
                    PuzzleBoardElement.Children.Add(dotContainer);

                    _img_path = this._CurrentCharacter.getPath();

                    ImageSourceConverter imgConv = new ImageSourceConverter();
                    img_word.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _img_path.ToString());
                }
            }
        }

        private void ExtendNextDotCircle(int dotIdx)
        {
            if (this._NextDot == null)
            {
                this._NextDot = new Ellipse();
                this._NextDot.Width = 50;
                this._NextDot.Height = 50;
                this._NextDot.Fill = Brushes.Tomato;
            }
            else if (PuzzleBoardElement.Children.Contains(this._NextDot) == true)
            {
                PuzzleBoardElement.Children.Remove(this._NextDot);
            }

            Point p = this._CurrentCharacter.DotList[dotIdx];
            Canvas.SetTop(this._NextDot, p.Y - (this._NextDot.Width / 2));
            Canvas.SetLeft(this._NextDot, p.X - (this._NextDot.Height / 2));
            PuzzleBoardElement.Children.Add(this._NextDot);
        }
        #endregion PuzzleMethods

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
            animation.Duration = TimeSpan.FromSeconds(2);

            Storyboard.SetTargetName(animation, "GradientStop1");
            Storyboard.SetTargetProperty(animation, new PropertyPath(GradientStop.OffsetProperty));

            DoubleAnimation animation2 = new DoubleAnimation();
            animation2.From = 1.0;
            animation2.To = 0.0;
            animation2.Duration = TimeSpan.FromSeconds(2);

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

            if (currentBtn.Equals(btn_prev))
            {
                this._soundManager.PlayAudio(AudioList.Lists.이전문제);
            }
            else if (currentBtn.Equals(btn_next))
            {
                this._soundManager.PlayAudio(AudioList.Lists.다음문제);
            }
            else if (currentBtn.Equals(btn_back))
            {
                this._soundManager.PlayAudio(AudioList.Lists.뒤로가기);
            }
        }

        private void HandEnterButtonHandler(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            ApplyProgressAnimationOnButton(btn);
        }
        
        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            if (this._CharacterIndex >= this._CharacterList.Count)
            {
                //모든 글자에 대한 테스트가 끝난 경우
                //우선 클리어 메시지 출력 후
                btn_back_Click(btn_back, new RoutedEventArgs());
            }
            else
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();

                image_notify.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/img_next.png");
                panel1.Visibility = Visibility.Hidden;

                panel_image.Visibility = Visibility.Visible;
                
                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(2);
                dispatcherTimer.Tick += delegate
                {
                    image_notify.Source = null;
                    panel_image.Visibility = Visibility.Hidden;
                    panel1.Visibility = Visibility.Visible;

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();

                //아직 테스트할 글자가 남은 경우
                RemoveCurrentPuzzle();
                SettingNextPuzzle();
            }
        }

        private void btn_prev_Click(object sender, RoutedEventArgs e)
        {
            if (this._CharacterIndex <= 1)
            {
                btn_back_Click(btn_back, new RoutedEventArgs());
            }
            else
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();

                image_notify.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/img_prev.png");
                panel1.Visibility = Visibility.Hidden;

                panel_image.Visibility = Visibility.Visible;

                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(2);
                dispatcherTimer.Tick += delegate
                {
                    image_notify.Source = null;
                    panel_image.Visibility = Visibility.Hidden;
                    panel1.Visibility = Visibility.Visible;

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();

                //이전 테스트 꺼내오기
                RemoveCurrentPuzzle();
                SettingPrevPuzzle();
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            this.LearnContentCloseHandler(this, new EventArgs());
        }
        #endregion ButtonMethods
    }
}
