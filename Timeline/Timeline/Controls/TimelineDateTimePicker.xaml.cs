using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;

namespace Timeline.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TimelineDateTimePicker : ContentView
	{
		public TimelineDateTimePicker ()
		{
			InitializeComponent();
            this.LayoutChanged += TimelineDateTimePicker_LayoutChanged;
		}

        private void TimelineDateTimePicker_LayoutChanged(object sender, EventArgs e)
        {
            //get screen size
            double h = DeviceDisplay.ScreenMetrics.Height;
            double w = DeviceDisplay.ScreenMetrics.Width;
            double sx;
            double sy;
            (sx, sy) = GetScreenCoordinates(abs);
            AbsoluteLayout.SetLayoutBounds(root, new Rectangle(-sx, -sy, h, w));
        }

        public (double X, double Y) GetScreenCoordinates(VisualElement view)
        {
            // A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
            // and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
            // The coordinates returned refer to the top left corner of the view.

            // Initialize with the view's "local" coordinates with respect to its parent
            double screenCoordinateX = view.X;
            double screenCoordinateY = view.Y;

            // Get the view's parent (if it has one...)
            if (view.Parent.GetType() != typeof(App))
            {
                VisualElement parent = (VisualElement)view.Parent;


                // Loop through all parents
                while (parent != null)
                {
                    // Add in the coordinates of the parent with respect to ITS parent
                    screenCoordinateX += parent.X;
                    screenCoordinateY += parent.Y;

                    // If the parent of this parent isn't the app itself, get the parent's parent.
                    if (parent.Parent.GetType() == typeof(App))
                        parent = null;
                    else
                        parent = (VisualElement)parent.Parent;
                }
            }

            // Return the final coordinates...which are the global SCREEN coordinates of the view
            return (screenCoordinateX, screenCoordinateY);
        }
    }
}