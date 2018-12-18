using UnityEngine;

namespace Drawing
{
    public class Drawing
    {

        public static Texture2D lineTex;

        public static void DrawLine(Rect rect) { DrawLine(rect, GUI.contentColor, 1.0f); }
        public static void DrawLine(Rect rect, Color color) { DrawLine(rect, color, 1.0f); }
        public static void DrawLine(Rect rect, float width) { DrawLine(rect, GUI.contentColor, width); }
        public static void DrawLine(Rect rect, Color color, float width) { DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y + rect.height), color, width); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB) { DrawLine(pointA, pointB, GUI.contentColor, 1.0f); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color) { DrawLine(pointA, pointB, color, 1.0f); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, float width) { DrawLine(pointA, pointB, GUI.contentColor, width); }
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            ///
            /// Draws a line on the screen using the GUI, rather than Unity's debugger. Allows for lines in the game view.
            ///

            // TODO: The overrides that take Vector3s can get REAL weird if one point is behind the camera.

            // A zero-length line will cause issues (GUI matrix will scale to 0), so skip drawing.
            if ((pointA - pointB).magnitude < 1 )
            {
                return;
            }
            // Ensure points are integer values. Decimals will also cause issues.
            pointA = new Vector2((int)pointA.x, (int)pointA.y);
            pointB = new Vector2((int)pointB.x, (int)pointB.y);

            // Save the current GUI matrix, since we're going to make changes to it.
            Matrix4x4 matrix = GUI.matrix;

            // Generate a single pixel texture if it doesn't exist
            if (!lineTex) { lineTex = new Texture2D(1, 1); }

            // Store current GUI color, so we can switch it back later,
            // and set the GUI color to the color parameter
            Color savedColor = GUI.color;
            GUI.color = color;

            // Determine the angle of the line.
            float angle = Vector3.Angle(pointB - pointA, Vector2.right);

            // Vector3.Angle always returns a positive number.
            // If pointB is above pointA, then angle needs to be negative.
            if (pointA.y > pointB.y) { angle = -angle; }
            // Use ScaleAroundPivot to adjust the size of the line.
            // We could do this when we draw the texture, but by scaling it here we can use
            //  non-integer values for the width and length (such as sub 1 pixel widths).
            // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
            //  is centered on the origin at pointA.
            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));

            // Set the rotation for the line.
            //  The angle was calculated with pointA as the origin.
            GUIUtility.RotateAroundPivot(angle, pointA);

            // Finally, draw the actual line.
            // We're really only drawing a 1x1 texture from pointA.
            // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
            //  render with the proper width, length, and angle.
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

            // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
            GUI.matrix = matrix;
            GUI.color = savedColor;
        }
        public static void DrawLine(Vector3 pointA, Vector3 pointB, Color color, float width)
        {
            
            Camera c = Camera.main;
            Vector2 pointA_s = c.WorldToScreenPoint(pointA);
            Vector2 pointB_s = c.WorldToScreenPoint(pointB);
            DrawLine(screen_flip(pointA_s), screen_flip(pointB_s), color, width);
        }
        public static void DrawLine(Vector3 pointA, Vector3 pointB, float width)
        {
            Camera c = Camera.main;
            Vector2 pointA_s = c.WorldToScreenPoint(pointA);
            Vector2 pointB_s = c.WorldToScreenPoint(pointB);
            DrawLine(screen_flip(pointA_s), screen_flip(pointB_s), width);
        }
        public static void DrawLine(Vector3 pointA, Vector3 pointB, Color color)
        {
            Camera c = Camera.main;
            Vector2 pointA_s = c.WorldToScreenPoint(pointA);
            Vector2 pointB_s = c.WorldToScreenPoint(pointB);
            DrawLine(screen_flip(pointA_s), screen_flip(pointB_s), color);
        }
        public static void DrawLine(Vector3 pointA, Vector3 pointB)
        {
            Camera c = Camera.main;
            Vector2 pointA_s = c.WorldToScreenPoint(pointA);
            Vector2 pointB_s = c.WorldToScreenPoint(pointB);
            DrawLine(screen_flip(pointA_s), screen_flip(pointB_s));
        }

        private static Vector2 screen_flip(Vector2 in_coord)
        {
            Camera c = Camera.main;
            Vector2 out_coord = new Vector2();

            out_coord.x = in_coord.x;
            out_coord.y = c.pixelHeight - in_coord.y;
            return out_coord;
        }

    }
}
