using UnityEngine;


namespace MyInput
{
    public static class MyInput
    {
        public static Vector2 ProcessInputAxes(bool debug = false)
        {
            ///
            /// Cleans up the raw axis inputs.
            ///

            Vector2 input_axes;
            // Gather inputs
            input_axes.x = Input.GetAxisRaw("Horizontal");
            input_axes.y = Input.GetAxisRaw("Vertical");
            // Turns weird square input into good input? Would be worth writing an input debugger.
            input_axes = Vector2.ClampMagnitude(input_axes, 1.0f);

            if (debug)
            {
                Vector3 exaggerated_input = new Vector3(input_axes.x, 0, input_axes.y) * 5;
                Debug.DrawRay(exaggerated_input, new Vector3(0, 1, 0), Color.blue);
                //Debugging.Debugging.Debug3DText("Input", exaggerated_input);
            }


            return input_axes;
        }


        public static Vector3 CoordsOrientedAboveCamera(Vector2 coords, bool debug = false)
        {
            ///
            /// Orients the given coordinates to the active camera.
            /// A camera looking "north" will not affect the coordinates.
            /// A camera looking "west" will rotate the coordinates -90 degrees. 
            ///

            Camera c = Camera.main;
            Vector3 cam_forward_flat = new Vector3(c.transform.forward.x, 0, c.transform.forward.z).normalized;

            // Get the rotation needed to rotate +Z to the cam forward vector. Apply the same rotation to the input coordinates.
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(Vector3.forward, cam_forward_flat);

            Vector3 coords_in_3d = new Vector3(coords.x, 0, coords.y);
            coords_in_3d = rot * coords_in_3d;

            ///-------------------------
            ///--- DEBUG DRAWING -------
            ///-------------------------
            if (debug)
            {
                Debug.DrawRay(coords_in_3d * 5, new Vector3(0,1,0), Color.green);
                Debugger.Debug3DText("Camera Oriented Input", coords_in_3d * 5);
                Debugger.Debug3DLine(coords_in_3d * 5, new Vector3(0,0,0));
            }

            return new Vector2(coords_in_3d.x, coords_in_3d.z);
        }


        public static Vector3 CoordsThroughCameraToPlane(Vector2 coords, Plane tgt_plane, bool debug = false)
        {
            ///
            /// Projects the given coordinates through the active camera's screen plane to the target plane, returning the intersection.
            ///
            bool intersect = false;         // Whether the coordinates were mapped to the plane.
            Vector2 in_pixel;               // Coords mapped to a pixel on the screen.
            float ray_distance = 10.0f;

            // Get input as a screen pixel.
            Camera c = Camera.main;
            int c_hwidth = c.pixelWidth / 2;        // Half width
            int c_hheight = c.pixelHeight / 2;      // Half height
            in_pixel.x = coords.x * 100 + c_hwidth;
            in_pixel.y = coords.y * 100 + c_hheight;

            // Raycast through the pixel and find the distance to the plane. It's possible to miss the plane.
            Ray in_as_ray = c.ScreenPointToRay(in_pixel);
            if (tgt_plane.Raycast(in_as_ray, out ray_distance))
            {
                intersect = true;
            }
            // TODO: What do we do if we miss the plane? Error out?
            // TODO: Try casting "backwards" (camera could be facing away from plane)

            ///-------------------------
            ///--- DEBUG DRAWING -------
            ///-------------------------
            if (debug)
            {
                if (intersect)
                {
                    // If an intersection occurred, draw both the camera ray and a marker of the intersection.
                    //Debug.Log("intersect!");
                    Debug.DrawRay(in_as_ray.GetPoint(ray_distance), new Vector3(0.0f, 1.0f, 0.0f), Color.green);
                    Debug.DrawRay(in_as_ray.origin, in_as_ray.direction * ray_distance, Color.yellow);
                    Debugger.Debug3DText("Camera Projected Input", in_as_ray.GetPoint(ray_distance));
                }
                else
                {
                    // If no intersection occurred, draw the camera ray.
                    Debug.DrawRay(in_as_ray.origin, in_as_ray.direction * ray_distance, Color.red);
                    Debugger.Debug3DText("Camera Projected Input (NO COLLISION)", in_as_ray.GetPoint(ray_distance));
                }

            }

            if (intersect)
            {
                return in_as_ray.GetPoint(ray_distance);
            }
            else
            {
                // TODO: Error out? Return a null value? Return infinity?
                return Vector3.zero;
            }
        }
    }
}