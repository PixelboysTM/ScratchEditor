using System;
using System.Windows;

namespace ScratchEditor.misc.math
{
    public class Collisions
    {
        public static bool RectRect(Rect r1, Rect r2)
        {
            if (r1.X + r1.Width >= r2.X &&    // r1 right edge past r2 left
                r1.X <= r2.X + r2.Width &&    // r1 left edge past r2 right
                r1.Y + r1.Height >= r2.Y &&    // r1 top edge past r2 bottom
                r1.Y <= r2.Y + r2.Height) {    // r1 bottom edge past r2 top
                return true;
            }
            return false;
        }
        public static bool GeometryPoint(Point[] vertices, double px, double py) {
            bool collision = false;

            // go through each of the vertices, plus
            // the next vertex in the list
            int next = 0;
            for (int current=0; current<vertices.Length; current++) {

                // get next vertex in list
                // if we've hit the end, wrap around to 0
                next = current+1;
                if (next == vertices.Length) next = 0;

                // get the PVectors at our current position
                // this makes our if statement a little cleaner
                Point vc = vertices[current];    // c for "current"
                Point vn = vertices[next];       // n for "next"

                // compare position, flip 'collision' variable
                // back and forth
                if (((vc.Y >= py && vn.Y < py) || (vc.Y < py && vn.Y >= py)) &&
                    (px < (vn.X-vc.X)*(py-vc.Y) / (vn.Y-vc.Y)+vc.X)) {
                    collision = !collision;
                }
            }
            return collision;
        }
        
        public static bool PointCircle(double px, double py, double cx, double cy, double r) {

            // get distance between the point and circle's center
            // using the Pythagorean Theorem
            double distX = px - cx;
            double distY = py - cy;
            double distance = Math.Sqrt( (distX*distX) + (distY*distY) );

            // if the distance is less than the circle's
            // radius the point is inside!
            if (distance <= r) {
                return true;
            }
            return false;
        }

        public static bool PointCircle(Point circleCenter, Point mousePos, double radius)
        {
            return PointCircle(mousePos.X, mousePos.Y, circleCenter.X, circleCenter.Y, radius);
        }
    }
}