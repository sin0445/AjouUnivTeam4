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
        private KinectSensor _KinectDevice;
        private Skeleton[] _Skeletons;

        private DispatcherTimer _timer;
        private int _ticks;
        private UIElement _lastElement;

        private MenuWindow _menuWindow;
        #endregion Member Variables

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { DiscoverKinectSensor(); };
            this.Unloaded += (s, e) => { this._KinectDevice = null; };
        }

        #region CoreMethods
        private void DiscoverKinectSensor()
        {
            KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.KinectDevice == null)
                    {
                        this.KinectDevice = e.Sensor;
                    }
                    break;

                case KinectStatus.Disconnected:
                    if (this.KinectDevice == e.Sensor)
                    {
                        this.KinectDevice = null;
                        this.KinectDevice = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

                        if (this.KinectDevice == null)
                        {
                            //Notify the user that the sensor is disconnected
                        }
                    }
                    break;
            }
        }

        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
            DepthImagePoint dp = this.KinectDevice.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, this.KinectDevice.DepthStream.Format);
            Point point = new Point();

            point.X = (int)(dp.X * this.LayoutRoot.RenderSize.Width / this.KinectDevice.DepthStream.FrameWidth);
            point.Y = (int)(dp.Y * this.LayoutRoot.RenderSize.Height / this.KinectDevice.DepthStream.FrameHeight);

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
                Window window = Application.Current.MainWindow;

                Point point = GetJointPoint(hand);

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
            if ((element = (UIElement)GetHitImage(hand, GameStartBtn)) != null)
            {
                if (this._lastElement != null && element.Equals(this._lastElement))
                {
                    if (this._timer != null)
                    {
                        CreateTimer();
                    }
                    else if(this._ticks >= 20)
                    {
                        //윈도우 전환

                        this._menuWindow = new MenuWindow();
                        this._menuWindow.MenuCloseHandler += MenuClose;
                        Main.Visibility = Visibility.Hidden;
                        LayoutRoot.Children.Add(_menuWindow);
                    }
                }
                else
                {
                    CreateTimer();
                    _lastElement = element;
                }
            }
            else if ((element = (UIElement)GetHitImage(hand, GameExitBtn)) != null)
            {
                if (this._lastElement != null && element.Equals(this._lastElement))
                {
                    if (this._timer == null)
                    {
                        CreateTimer();
                    }
                    else if (this._ticks >= 20)
                    {
                        LayoutRoot.Children.Clear();
                        Close();
                        //윈도우 전환
                    }
                }
                else
                {
                    if (this._timer != null)
                    {
                        RemoveTimer();
                    }
                    CreateTimer();
                    _lastElement = element;
                }
            }
            else
            {
                if (this._lastElement != null)
                    this._lastElement = null;

                if (this._timer != null)
                {
                    RemoveTimer();
                }
            }
        }
        #endregion TrackingMethods

        private void CreateTimer()
        {
            this._ticks = 0;
            this._timer = new DispatcherTimer();
            this._timer.Interval = TimeSpan.FromSeconds(0.1);
            this._timer.Tick += new EventHandler(OnTimerTick);
        }

        private void RemoveTimer()
        {
            this._ticks = 0;
            this._timer.Stop();
            this._timer = null;
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            this._ticks++;
        }

        private void MenuClose(object sender, EventArgs e)
        {
            Main.Visibility = Visibility.Visible;
            LayoutRoot.Children.Remove(this._menuWindow);
            this._menuWindow = null;
        }

        #region Property
        public KinectSensor KinectDevice
        {
            //Uninitialize
            get { return this._KinectDevice; }
            set
            {
                if (this._KinectDevice != value)
                {
                    if (this._KinectDevice != null)
                    {
                        this._KinectDevice.Stop();
                        this._KinectDevice.SkeletonStream.Disable();
                        this._KinectDevice.SkeletonFrameReady -= KinectDevice_SkeletonFrameReady;
                        this._Skeletons = null;
                        this._KinectDevice = null;
                    }
                    this._KinectDevice = value;
                }

                //Initialize
                if (this._KinectDevice != null)
                {
                    if (this._KinectDevice.Status == KinectStatus.Connected)
                    {
                        //this.KinectDevice = value;
                        this._KinectDevice.SkeletonStream.Enable();
                        this._Skeletons = new Skeleton[this.KinectDevice.SkeletonStream.FrameSkeletonArrayLength];
                        this._KinectDevice.SkeletonFrameReady += KinectDevice_SkeletonFrameReady;
                        this._KinectDevice.Start();
                    }
                }
            }
        }
        #endregion Property
    }
}
