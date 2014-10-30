using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MuseLabExhibitApplication.Gestures
{
    interface IGestureSegment
    {
        GesturePartResult Update(Skeleton skeleton);
    }
}
