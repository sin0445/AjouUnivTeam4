﻿using System;
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
    /// Win_quiz_content.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Win_quiz_content : UserControl
    {
        public event EventHandler<EventArgs> QuizContentCloseHandler;

        #region MemberVariables
        private MainWindow _mainWindow;
        private KinectController _KinectController;
        private KinectSensor _KinectDevice;
        private Grid _layoutRoot;
        private Skeleton[] _Skeletons;

        private UIElement _lastElement;
        private int _ticks;
        private DispatcherTimer _buttonTimer;
        private const int _hoverTime = 20;
        private SoundManager _soundManager;
        private List<Button> _animatedBtnList;

        private QuizElements _quizElements;

        // 첫번째 문제인지 확인하는 flag
        private bool flag;

        #endregion MemberVariables

        #region Constructor
        public Win_quiz_content(MainWindow win, KinectController control, QuizElementListLibrary.QUIZTYPE type, SoundManager sound)
        {
            InitializeComponent();
            flag = true;

            this._mainWindow = win;
            this._layoutRoot = this._mainWindow.LayoutRoot;
            this._KinectController = control;
            this._soundManager = sound;
            this._animatedBtnList = new List<Button>();
            this._soundManager.PlayAudio(AudioList.Lists.퀴즈배경);

            this.Loaded += (s, e) => { InitQuizContentWindow(); };
            this.Unloaded += (s, e) => { UninitQuizContentWindow(); };

            _quizElements = QuizElementListLibrary.getQuizElements(type);
            next_quiz();
        }
        
        private void InitQuizContentWindow()
        {
            this._KinectDevice = this._KinectController._KinectDevice;
            this._KinectDevice.SkeletonFrameReady += LearnContentWindow_SkeletonFrameReady;
            this._Skeletons = new Skeleton[this._KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
        }
        private void UninitQuizContentWindow()
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
            Point handPoint = GetJointPoint(hand);
            if (handPoint.X <= img_quiz.ActualWidth && handPoint.Y <= img_quiz.ActualHeight)
            {
                //Do Nothing
            }
            else if(handPoint.X > img_quiz.ActualWidth && handPoint.Y <= img_quiz.ActualHeight)
            {
                TrackHandLocationOnButton(hand);
            }
            else if(handPoint.Y > img_quiz.ActualHeight)
            {
                if (this._animatedBtnList.Count > 0)
                {
                    foreach (Button btn in this._animatedBtnList)
                    {
                        btn.Background = new SolidColorBrush(Colors.White);
                    }
                    this._animatedBtnList.Clear();
                }
                TrackHandLocationOnQuiz(hand);
            }
        }
        private void TrackHandLocationOnButton(Joint hand)
        {
            UIElement element;
            //손이 버튼 위에 있는 경우
            //계속 버튼위에 있는 경우(Timer 체크 후 일정 시간 이상 지나면 현재 윈도우 hidden 후 메뉴창 add) 
            //새로운 버튼위에 있는 경우(Timer 초기화 후 lastElement에 현재 버튼 등록)

            //손이 버튼위에 없는 경우
            //버튼에서 벗어난 경우(lastElement null, Timer stop후 null)
            //원래 밖에 있었던 경우(그냥 무시)

            if ((element = (UIElement)GetHitImage(hand, btn_next)) != null)         //StartBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //StartBtn에 계속하여 손을 대고 있는 경우
                {
                    if (this._buttonTimer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();

                        this.btn_next_Click(btn_next, new RoutedEventArgs());
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
                    if (this._buttonTimer == null)    //만일 타이머가 없으면 생성시킨다
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)     //타이머가 있으나 특정 시간 이상 손을 댄 경우
                    {
                        RemoveTimer();
                        this.btn_back_Click(btn_back, new RoutedEventArgs());
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

                if (this._buttonTimer != null)                //타이머가 있는 경우에도 이를 제거
                {
                    RemoveTimer();
                }

                foreach (Button btn in this._animatedBtnList)
                {
                    btn.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void TrackHandLocationOnQuiz(Joint hand)
        {
            UIElement element;
            //손이 버튼 위에 있는 경우
            //계속 버튼위에 있는 경우(Timer 체크 후 일정 시간 이상 지나면 현재 윈도우 hidden 후 메뉴창 add) 
            //새로운 버튼위에 있는 경우(Timer 초기화 후 lastElement에 현재 버튼 등록)

            //손이 버튼위에 없는 경우
            //버튼에서 벗어난 경우(lastElement null, Timer stop후 null)
            //원래 밖에 있었던 경우(그냥 무시)

            if ((element = (UIElement)GetHitImage(hand, btn_1)) != null)         //StartBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //StartBtn에 계속하여 손을 대고 있는 경우
                {
                    if (this._buttonTimer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)
                    {
                        //윈도우 전환
                        RemoveTimer();

                        this.btn_1_Click(btn_1, new RoutedEventArgs());
                    }
                }
                else                    //새롭게 StartBtn에 손을 올린 경우
                {
                    if (this._buttonTimer != null)
                        RemoveTimer();

                    CreateTimer();
                }
                _lastElement = element;
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_2)) != null)     //ExitBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //계속해서 ExitBtn에 손을 대고 있는 경우
                {
                    if (this._buttonTimer == null)    //만일 타이머가 없으면 생성시킨다
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)     //타이머가 있으나 특정 시간 이상 손을 댄 경우
                    {
                        RemoveTimer();
                        this.btn_2_Click(btn_2, new RoutedEventArgs());
                        //프로그램 종료
                    }
                }
                else                         //새롭게 ExitBtn에 손을 댄 경우
                {
                    if (this._buttonTimer != null)            //만일 타이머가 기존에 존재하는 경우 이를 제거한후
                    {
                        RemoveTimer();
                    }
                    CreateTimer();                      //다시 타이머를 생성한다
                }
                _lastElement = element;                 //그리고 이전 element에 ExitBtn을 등록
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_3)) != null)     //ExitBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //계속해서 ExitBtn에 손을 대고 있는 경우
                {
                    if (this._buttonTimer == null)    //만일 타이머가 없으면 생성시킨다
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)     //타이머가 있으나 특정 시간 이상 손을 댄 경우
                    {
                        RemoveTimer();
                        this.btn_3_Click(btn_3, new RoutedEventArgs());
                        //프로그램 종료
                    }
                }
                else                         //새롭게 ExitBtn에 손을 댄 경우
                {
                    if (this._buttonTimer != null)            //만일 타이머가 기존에 존재하는 경우 이를 제거한후
                    {
                        RemoveTimer();
                    }
                    CreateTimer();                      //다시 타이머를 생성한다
                }
                _lastElement = element;                 //그리고 이전 element에 ExitBtn을 등록
            }
            else if ((element = (UIElement)GetHitImage(hand, btn_4)) != null)     //ExitBtn을 클릭하는 로직
            {
                if (this._lastElement != null && element.Equals(this._lastElement))     //계속해서 ExitBtn에 손을 대고 있는 경우
                {
                    if (this._buttonTimer == null)    //만일 타이머가 없으면 생성시킨다
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= _hoverTime)     //타이머가 있으나 특정 시간 이상 손을 댄 경우
                    {
                        RemoveTimer();
                        this.btn_4_Click(btn_4, new RoutedEventArgs());
                        //프로그램 종료
                    }
                }
                else                         //새롭게 ExitBtn에 손을 댄 경우
                {
                    if (this._buttonTimer != null)            //만일 타이머가 기존에 존재하는 경우 이를 제거한후
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

                if (this._buttonTimer != null)                //타이머가 있는 경우에도 이를 제거
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

        #region TimerMethods
        private void CreateTimer()
        {
            this._ticks = 0;
            this._buttonTimer = new DispatcherTimer();
            this._buttonTimer.Interval = TimeSpan.FromSeconds(0.1);
            this._buttonTimer.Tick += new EventHandler(OnTimerTick);
            this._buttonTimer.Start();
        }

        private void RemoveTimer()
        {
            this._buttonTimer.Stop();
            this._buttonTimer.Tick -= OnTimerTick;
            this._ticks = 0;
            this._buttonTimer = null;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this._ticks++;
        }
        #endregion TimerMethods

        #region QuizMethods

        // 다음 문제 출제 함수
        private void next_quiz()
        {
            // 모든 문제 출제시 프로그램 종료
            if (_quizElements.isGameEnd())
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();

                image_notify.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/img_back.jpg");
                panel1.Visibility = Visibility.Hidden;

                panel_image.Visibility = Visibility.Visible;

                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += delegate
                {
                    image_notify.Source = null;
                    panel_image.Visibility = Visibility.Hidden;
                    panel1.Visibility = Visibility.Visible;

                    this.QuizContentCloseHandler(this, new EventArgs());

                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
            }

            else
            {
                if (flag)
                {
                    flag = !flag;
                }
                else
                {
                    ImageSourceConverter imgConv = new ImageSourceConverter();

                    image_notify.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/img_next.jpg");
                    panel1.Visibility = Visibility.Hidden;

                    panel_image.Visibility = Visibility.Visible;

                    var dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                    dispatcherTimer.Tick += delegate
                    {
                        image_notify.Source = null;
                        panel_image.Visibility = Visibility.Hidden;
                        panel1.Visibility = Visibility.Visible;

                        dispatcherTimer.Stop();
                    };

                    dispatcherTimer.Start();
                }

                // 문제 출력 함수
                _quizElements.SetQuiz();

                // 화면 출력 함수
                MakingQuiz();
            }
        }

        // 화면 출력 함수
        private void MakingQuiz()
        {
            ImageSourceConverter imgConv = new ImageSourceConverter();

            // 정답 element Image 출력
            img_quiz.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _quizElements.QuizList[0].ToString());
            
            quiz1.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _quizElements.QuizList[1].ToString());
            quiz2.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _quizElements.QuizList[2].ToString());
            quiz3.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _quizElements.QuizList[3].ToString());
            quiz4.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/" + _quizElements.QuizList[4].ToString());
        }

        private void btn_1_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(0, quiz1, btn_1);
        }

        private void btn_2_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(1, quiz2, btn_2);
        }

        private void btn_3_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(2, quiz3, btn_3);
        }

        private void btn_4_Click(object sender, RoutedEventArgs e)
        {
            checking_answer(3, quiz4, btn_4);
        }

        private void checking_answer(int user_answer, Image button, Button btn)
        {
            this._soundManager.PlayAudio(AudioList.Lists.물방울효과);
            // 사용자 선택 답과 정답 비교
            if (user_answer.ToString().Equals(_quizElements.QuizList[5]))
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();

                string temp_source = button.Source.ToString();
                button.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/correct.png");

                Brush background = btn.Background;
                btn.Background = Brushes.White;

                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += delegate
                {
                    this._soundManager.PlayAudio(this._quizElements.getAnswerName());
                    button.Source = (ImageSource)imgConv.ConvertFromString(temp_source);
                    btn.Background = background;
                    next_quiz();
                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
            }

            else
            {
                ImageSourceConverter imgConv = new ImageSourceConverter();

                string temp_source = button.Source.ToString();
                button.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/wrong.png");

                Brush background = btn.Background;
                btn.Background = Brushes.White;

                var dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
                dispatcherTimer.Tick += delegate
                {
                    button.Source = (ImageSource)imgConv.ConvertFromString(temp_source);
                    btn.Background = background;
                    dispatcherTimer.Stop();
                };

                dispatcherTimer.Start();
            }
        }

        #endregion QuizMethods

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
            if (this._buttonTimer != null)
                RemoveTimer();
            CreateTimer();

            foreach (Button btn in this._animatedBtnList)
            {
                btn.Background = new SolidColorBrush(Colors.White);
            }

            HandEnterButtonHandler(currentBtn, new RoutedEventArgs());
            this._animatedBtnList.Add(currentBtn);

            if (currentBtn.Equals(btn_next))
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

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            ImageSourceConverter imgConv = new ImageSourceConverter();

            image_notify.Source = (ImageSource)imgConv.ConvertFromString("pack://application:,,/Images/img_back.jpg");
            panel1.Visibility = Visibility.Hidden;

            panel_image.Visibility = Visibility.Visible;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += delegate
            {
                image_notify.Source = null;
                panel_image.Visibility = Visibility.Hidden;
                panel1.Visibility = Visibility.Visible;

                this.QuizContentCloseHandler(this, new EventArgs());

                dispatcherTimer.Stop();
            };

            this._soundManager.PauseBackground();
            dispatcherTimer.Start();
        }

        private void btn_next_Click(object sender, RoutedEventArgs e)
        {
            next_quiz();
        }
        #endregion ButtonMethods
    }
}
