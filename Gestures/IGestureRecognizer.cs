using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MuseLabExhibitApplication.Gestures
{
    interface IGestureRecognizer
    {
        void Update(Skeleton skeleton);
        void Reset();
    }
}
