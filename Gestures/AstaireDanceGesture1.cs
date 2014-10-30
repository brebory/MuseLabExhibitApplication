using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace MuseLabExhibitApplication.Gestures
{
    class AstaireDanceGesture1Recognizer : IGestureRecognizer
    {
                readonly int WINDOW_SIZE = 50;

        private IGestureSegment[] _segments;

        private int _currentSegment = 0;
        private int _frameCount = 0;

        private float _throttleTime = TimeSpan.TicksPerSecond;
        private DateTime _lastRecognized;

        public event EventHandler GestureRecognized;

        public AstaireDanceGesture1Recognizer()
        {
            AstaireDanceGesture1Segment1 adg1Segment = new AstaireDanceGesture1Segment1();
            AstaireDanceGesture1Segment2 adg2Segment = new AstaireDanceGesture1Segment2();

            _segments = new IGestureSegment[]{
                adg1Segment,
                adg2Segment
            };

            _lastRecognized = DateTime.Now;
        }

        public void Update(Skeleton skeleton)
        {
            GesturePartResult result = _segments[_currentSegment].Update(skeleton);

            if (result == GesturePartResult.Succeeded)
            {
                if (_currentSegment + 1 < _segments.Length)
                {
                    // Advance to the next segment, and reset the frame count
                    _currentSegment++;
                    _frameCount = 0;
                }
                else
                {
                    ThrottleRunGestureRecognized();
                }
            }
            else if (_frameCount == WINDOW_SIZE)
            {
                Reset();
            }
            else
            {
                _frameCount++;
            }
        }

        public void Reset()
        {
            _currentSegment = 0;
            _frameCount = 0;
            _lastRecognized = DateTime.Now;
        }

        private void ThrottleRunGestureRecognized()
        {
            if (DateTime.Compare(_lastRecognized.AddSeconds(1.0), DateTime.Now) <= 0)
            {
                if (GestureRecognized != null)
                {
                    _lastRecognized = DateTime.Now;
                    GestureRecognized(this, new EventArgs());
                    Reset();
                }
            }
        }
    }
}
