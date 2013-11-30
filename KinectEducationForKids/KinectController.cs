using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectEducationForKids
{
    public class KinectController
    {
        #region MemberVariables
        public KinectSensor _KinectDevice;
        #endregion MemberVariables

        public void DiscoverKinectSensor()
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

        public void UninitializeKinectSensor()
        {
            this._KinectDevice = null;
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
                        this._KinectDevice.SkeletonStream.Enable(new TransformSmoothParameters());
                        this._KinectDevice.Start();
                    }
                }
            }
        }
        #endregion Property
    }
}
