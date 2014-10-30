using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MuseLabExhibitApplication.Gestures
{
    public class ArmsCrossedGestureSegment1 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Right arm extended
            if ((skeleton.Joints[JointType.HandRight].Position.X >
                 skeleton.Joints[JointType.ElbowRight].Position.X) &&
                (Math.Abs(skeleton.Joints[JointType.HandRight].Position.Y -
                 skeleton.Joints[JointType.ElbowRight].Position.Y) < 0.2))
            {
                // Left arm extended
                if ((skeleton.Joints[JointType.HandLeft].Position.X <
                     skeleton.Joints[JointType.ElbowLeft].Position.X) &&
                    (Math.Abs(skeleton.Joints[JointType.HandLeft].Position.Y -
                     skeleton.Joints[JointType.ElbowLeft].Position.Y) < 0.2))
                {
                    return GesturePartResult.Succeeded;
                }
            }
            return GesturePartResult.Failed;
        }
    }

    public class ArmsCrossedGestureSegment2 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hands Crossed
            if (skeleton.Joints[JointType.HandRight].Position.X <
                skeleton.Joints[JointType.HandLeft].Position.X)
            {
                if (skeleton.Joints[JointType.ElbowRight].Position.X >
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
                return GesturePartResult.Paused;
            }
            else
            {
                if ((skeleton.Joints[JointType.HandRight].Position.X <
                     skeleton.Joints[JointType.ElbowRight].Position.X) &&
                    (skeleton.Joints[JointType.HandLeft].Position.X >
                     skeleton.Joints[JointType.ElbowLeft].Position.X))
                {
                    return GesturePartResult.Paused;
                }
            }
            return GesturePartResult.Failed;
        }
    }
}
