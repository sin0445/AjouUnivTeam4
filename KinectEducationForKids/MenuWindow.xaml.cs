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

namespace KinectEducationForKids
{
    /// <summary>
    /// MenuWindow.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class MenuWindow : UserControl
    {
        public event EventHandler<EventArgs> MenuCloseHandler;

        #region MemberVariables
        private Skeleton[] _Skeletons;
        #endregion MemeberVariables

        public MenuWindow()
        {
            InitializeComponent();
        }

        private void KinectDevice_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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

                            //Skeleton skeleton = GetPrimarySkeleton(this._Skeletons);

                            //if (skeleton == null)
                            //{
                            //    //만일 인식된 스켈레톤이 없는 경우 손 커서를 숨기게 된다.
                            //    //HandCursorElement.Visibility = Visibility.Collapsed;
                            //}
                            //else
                            //{
                            //    //Joint primaryHand = GetPrimaryHand(skeleton);
                            //    //TrackHand(primaryHand);
                            //    //TrackHandLocation(primaryHand);
                            //    //손에 따른 UI 변경 메소드 호출
                            //}
                        }
                    }
                }
            }
        }

        #region KinectDevice
        protected const string KinectDevicePropertyName = "KinectDevice";
        public static readonly DependencyProperty KinectDeviceProperty = DependencyProperty.Register(KinectDevicePropertyName, typeof(KinectSensor), typeof(MenuWindow), new PropertyMetadata(null, KinectDeviceChanged));

        public static void KinectDeviceChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            MenuWindow viewer = (MenuWindow)owner;

            if (e.OldValue != null)
            {
                KinectSensor sensor;
                sensor = (KinectSensor)e.NewValue;
                sensor.SkeletonFrameReady -= viewer.KinectDevice_SkeletonFrameReady;
            }

            if (e.NewValue != null)
            {
                viewer.KinectDevice = (KinectSensor)e.NewValue;
                viewer.KinectDevice.SkeletonFrameReady += viewer.KinectDevice_SkeletonFrameReady;
            }
        }

        public KinectSensor KinectDevice
        {
            get { return (KinectSensor)GetValue(KinectDeviceProperty); }
            set { SetValue(KinectDeviceProperty, value); }
        }
        #endregion KinectDevice
    }
}
