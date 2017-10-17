using System;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Xamarin.Forms;
using Android.Views;
using System.IO;
using Android.Support.Design.Widget;
using Apo_Chan.Custom;
using Apo_Chan.Droid;
using Android.Content.Res;

[assembly: ExportRenderer(typeof(FloatingActionButtonView), typeof(FloatingActionButtonViewRenderer))]
namespace Apo_Chan.Droid
{
    public class FloatingActionButtonViewRenderer : ViewRenderer<FloatingActionButtonView, FrameLayout>
    {

        public async static void Init()
        {
            var temp = DateTime.Now;
        }

        private const int MARGIN_DIPS = 16;
        private const int FAB_HEIGHT_NORMAL = 56;
        private const int FAB_HEIGHT_MINI = 40;
        private const int FAB_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_NORMAL;
        private const int FAB_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_NORMAL;
        private const int FAB_MINI_FRAME_HEIGHT_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_MINI;
        private const int FAB_MINI_FRAME_WIDTH_WITH_PADDING = (MARGIN_DIPS * 2) + FAB_HEIGHT_MINI;
        private readonly Android.Content.Context context;
        private readonly FloatingActionButton fab;
        private int[][] states = new int[][] {
            new int[] { Android.Resource.Attribute.StateEnabled}, // enabled
            new int[] {-Android.Resource.Attribute.StateEnabled}, // disabled
            new int[] {-Android.Resource.Attribute.StateChecked}, // unchecked
            new int[] { Android.Resource.Attribute.StatePressed}, // pressed
        };

        private int[] colors = new int[] {
            Android.Graphics.Color.Blue, // enabled
            Android.Graphics.Color.Black, // disabled
            Android.Graphics.Color.Green, // unchecked
            Android.Graphics.Color.White // pressed
        };

        public FloatingActionButtonViewRenderer()
        {
            context = Xamarin.Forms.Forms.Context;

            float d = context.Resources.DisplayMetrics.Density;
            var margin = (int)(MARGIN_DIPS * d); // margin in pixels

            fab = new FloatingActionButton(context);
            var lp = new FrameLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            lp.Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal;
            lp.LeftMargin = margin;
            lp.TopMargin = margin;
            lp.BottomMargin = margin;
            lp.RightMargin = margin;
            fab.LayoutParameters = lp;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FloatingActionButtonView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || this.Element == null)
                return;

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= HandlePropertyChanged;

            if (this.Element != null)
            {
                this.Element.PropertyChanged += HandlePropertyChanged;
            }

            Element.Show = Show;
            Element.Hide = Hide;

            SetFabImage(Element.ImageName);
            SetFabSize(Element.Size);

            fab.Click += Fab_Click;
            fab.RippleColor = Element.ColorRipple.ToAndroid();
            colors[0] = Element.ColorNormal.ToAndroid(); // enabled
            colors[3] = Element.ColorNormal.ToAndroid(); // pressed
            fab.BackgroundTintList = new ColorStateList(states, colors);

            var frame = new FrameLayout(context);
            frame.RemoveAllViews();
            frame.AddView(fab);

            SetNativeControl(frame);
        }

        public void Show(bool animate = true)
        {
            fab.Show();
        }

        public void Hide(bool animate = true)
        {
            fab.Hide();
        }

        void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {
                Tracker.UpdateLayout();
            }
            else if (e.PropertyName == FloatingActionButtonView.ColorNormalProperty.PropertyName)
            {
                colors[0] = Element.ColorNormal.ToAndroid(); // enabled
                fab.BackgroundTintList = new ColorStateList(states, colors);
            }
            else if (e.PropertyName == FloatingActionButtonView.ColorPressedProperty.PropertyName)
            {
                colors[3] = Element.ColorNormal.ToAndroid(); // pressed
                fab.BackgroundTintList = new ColorStateList(states, colors);
            }
            else if (e.PropertyName == FloatingActionButtonView.ColorRippleProperty.PropertyName)
            {
                fab.RippleColor = Element.ColorRipple.ToAndroid();
            }
            else if (e.PropertyName == FloatingActionButtonView.ImageNameProperty.PropertyName)
            {
                SetFabImage(Element.ImageName);
            }
            else if (e.PropertyName == FloatingActionButtonView.SizeProperty.PropertyName)
            {
                SetFabSize(Element.Size);
            }
            else if (e.PropertyName == FloatingActionButtonView.HasShadowProperty.PropertyName)
            {
                //fab.HasShadow = Element.HasShadow;
            }
        }

        void SetFabImage(string imageName)
        {
            if (!string.IsNullOrWhiteSpace(imageName))
            {
                try
                {
                    var drawableNameWithoutExtension = Path.GetFileNameWithoutExtension(imageName).ToLower();
                    var resources = context.Resources;
                    var imageResourceName = resources.GetIdentifier(drawableNameWithoutExtension, "drawable", context.PackageName);
                    fab.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeResource(context.Resources, imageResourceName));
                }
                catch (Exception ex)
                {
                    throw new FileNotFoundException("There was no Android Drawable by that name.", ex);
                }
            }
        }

        void SetFabSize(FloatingActionButtonSize size)
        {
            if (size == FloatingActionButtonSize.Mini)
            {
                fab.Size = FloatingActionButton.SizeMini;
                Element.WidthRequest = FAB_MINI_FRAME_WIDTH_WITH_PADDING;
                Element.HeightRequest = FAB_MINI_FRAME_HEIGHT_WITH_PADDING;
            }
            else
            {
                fab.Size = FloatingActionButton.SizeNormal;
                Element.WidthRequest = FAB_FRAME_WIDTH_WITH_PADDING;
                Element.HeightRequest = FAB_FRAME_HEIGHT_WITH_PADDING;
            }
        }

        void Fab_Click(object sender, EventArgs e)
        {
            Element.SendClicked();
        }
    }
}
