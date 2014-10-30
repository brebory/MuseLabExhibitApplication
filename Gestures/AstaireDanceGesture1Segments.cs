using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MuseLabExhibitApplication.Gestures
{

    class AstaireDanceGesture1Segment1 : IGestureSegment
    {
        double epsilon = 0.2;

        public GesturePartResult Update(Skeleton skeleton)
        {
            return GesturePartResult.Succeeded;
        }
    }

    class AstaireDanceGesture1Segment2 : IGestureSegment
    {
        double epsilon = 0.1;
        double kneeFootVerticalTolerance = 0.1;
        double handElbowHorizontalTolerance = 0.1;

        public GesturePartResult Update(Skeleton skeleton)
        {
            var joints = skeleton.Joints;
            // If the right foot is the leftmost joint...
            var comparisionJoints = from joint in skeleton.Joints where joint.JointType != JointType.FootRight select joint;
            if (comparisionJoints.All(joint => joint.Position.X > skeleton.Joints[JointType.FootRight].Position.X))
            {
                // If the right foot and the right knee are close to level...
                if ((Math.Abs(joints[JointType.FootRight].Position.Y - joints[JointType.KneeRight].Position.Y)) < epsilon + kneeFootVerticalTolerance)
                {
                    // If the hands and elbows are straight down...
                    if ((Math.Abs(joints[JointType.ElbowRight].Position.X - joints[JointType.HandRight].Position.X) < epsilon + handElbowHorizontalTolerance) &&
                        (Math.Abs(joints[JointType.ElbowLeft].Position.X - joints[JointType.HandLeft].Position.X) < epsilon + handElbowHorizontalTolerance))
                    {
                        return GesturePartResult.Succeeded;
                    }
                    else
                    {
                        return GesturePartResult.Paused;
                    }
                }

                else
                {
                    return GesturePartResult.Paused;
                }
            }
            return GesturePartResult.Failed;
        }
    }
}
