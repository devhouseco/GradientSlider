using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Devhouse.GradientSlider.Abstractions;
using Devhouse.GradientSlider.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.Widget.TextView;

[assembly: ExportRenderer(typeof(CustomGradientSlider), typeof(GradientSlider))]
namespace Devhouse.GradientSlider.Droid
{
    public class GradientSlider : SliderRenderer
    {
        public static void Init(Context context) { PackageName = context.PackageName; }

        private static string PackageName
        {
            get;
            set;
        }

        Android.Views.View thumbView;
        private bool isInitilized = false;
        CustomGradientSlider control = null;

        public GradientSlider(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            control = (CustomGradientSlider)Element;
        }

        private void Element_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var value = Element.Value;

            if (control.HasSegment)
            {
                value = (int)(Math.Round(Element.Value / control.Interval) * control.Interval);
                Element.SetValue(Slider.ValueProperty, value);
            }

            var thumb = GetThumb((int)Element.Value);
            Control.SetThumb(thumb);
            int thumbTop = thumb.IntrinsicHeight - Control.Height;
            thumb.SetBounds(thumb.Bounds.Left, thumbTop + 7, thumb.Bounds.Left + thumb.IntrinsicWidth, thumb.Bounds.Bottom);
            var gradientWidth = ((Control.Width - 45) / Element.Maximum) * value;
            var pd = (LayerDrawable)Control.ProgressDrawable;
            pd.SetLayerWidth(1, (int)gradientWidth);

            if (control.HasSegment.Equals(true))
                ChangeIntervalIcon();
        }

        void ChangeIntervalIcon()
        {
            var pd = (LayerDrawable)Control.ProgressDrawable;
            for (int i = 2; i < pd.NumberOfLayers; i++)
            {
                var layer = pd.FindDrawableByLayerId(998 + i);
                layer = (i - 2) * control.Interval < Element.Value ? Android.App.Application.Context.GetDrawable(Resource.Drawable.whitePoint) : Android.App.Application.Context.GetDrawable(Resource.Drawable.bluePoint);
                pd.SetDrawableByLayerId(998 + i, layer);
            }
        }

        public Drawable GetThumb(int progress)
        {
            ((TextView)thumbView.FindViewById(Resource.Id.thumbText)).SetText(progress.ToString(), BufferType.Normal);

            thumbView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);
            Bitmap bitmap = Bitmap.CreateBitmap(thumbView.MeasuredWidth, thumbView.MeasuredHeight, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            thumbView.Layout(0, 0, thumbView.MeasuredWidth, thumbView.MeasuredHeight);
            thumbView.Draw(canvas);

            return new BitmapDrawable(Resources, bitmap);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (!isInitilized)
                InitilizeSeekBar(Control.Width);

            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
                return;

            if (Control == null)
                return;

            var thumb = GetThumb((int)Element.Value);

            SeekBar seekbar = Control;
            seekbar.SetThumb(thumb);
            seekbar.SetPadding((int)20.0f.DpToPixels(Context), thumb.IntrinsicHeight / 2, (int)20.0f.DpToPixels(Context), thumb.IntrinsicHeight / 2);
            seekbar.SplitTrack = false;
            seekbar.SetMinimumHeight(12);
            int thumbTop = thumb.IntrinsicHeight - seekbar.Height;
            thumb.SetBounds(thumb.Bounds.Left, thumbTop + 7, thumb.Bounds.Left + thumb.IntrinsicWidth, thumb.Bounds.Bottom);
        }

        void InitilizeSeekBar(int width)
        {

            if (Control != null)
            {
                //convert details from CustomGradientSlider to android values
                var slider = Element as CustomGradientSlider;
                var startColor = control.TrackStartColor.ToAndroid();
                //var centerColor = Android.Graphics.Color.Orange;
                var endColor = control.TrackEndColor.ToAndroid();
                var cornerRadiusInPx = ((float)(slider.HeightRequest / 2.0)).DpToPixels(Context);
                var heightPx = (12.0f).DpToPixels(Context);

                var gradientWidth = (width / (int)Element.Maximum) * Element.Value;

                //create minimum track
                var p = new GradientDrawable(GradientDrawable.Orientation.LeftRight, new int[] { startColor, endColor });
                p.SetCornerRadius(cornerRadiusInPx);
                //var progress = new ClipDrawable(p, GravityFlags.Left, ClipDrawableOrientation.Horizontal);

                //create maximum track
                var background = new GradientDrawable();
                background.SetColor(control.TrackColor.ToAndroid());
                background.SetCornerRadius(cornerRadiusInPx);

                var shapes = new List<Drawable>();

                if (control.HasSegment)
                {
                    for (int i = 0; i < ((int)(Element.Maximum / control.Interval)) + 1; i++)
                    {
                        var shapeDrawable = Android.App.Application.Context.GetDrawable(Resource.Drawable.bluePoint);
                        shapes.Add(shapeDrawable);
                    }
                }

                var drawables = new List<Drawable> { background, p };
                drawables.AddRange(shapes);
                var pd = new LayerDrawable(drawables.ToArray());

                pd.SetLayerHeight(0, (int)heightPx);
                pd.SetLayerHeight(1, (int)heightPx);
                pd.SetLayerWidth(1, (int)gradientWidth);


                if (control.HasSegment)
                {
                    var segmentDelay = (width - (int)40.0f.DpToPixels(Context)) / (int)(Element.Maximum / (int)control.Interval);

                    for (int i = 2; i < drawables.Count; i++)
                    {
                        pd.SetLayerHeight(i, (int)5.0f.DpToPixels(Context));
                        pd.SetLayerWidth(i, (int)5.0f.DpToPixels(Context));
                        pd.SetLayerInset(i, i == 2 ? 10 : i == drawables.Count - 1 ? ((i - 2) * segmentDelay) - (int)10.0f.DpToPixels(Context) : (i - 2) * segmentDelay, (int)4.0f.DpToPixels(Context), 0, 0);
                        pd.SetId(i, 998 + i);
                    }
                }

                Control.ProgressDrawable = pd;
                Control.Progress = Control.Progress + 1;
                Control.Progress = Control.Progress - 1;
                Element.ValueChanged += Element_ValueChanged;
                LayoutInflater inflater = (LayoutInflater)Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService);
                thumbView = inflater.Inflate(Resource.Layout.layout_seekbar_thumb, null, false);

                var thumbText = ((TextView)thumbView.FindViewById(Resource.Id.thumbText));
                thumbText.TextSize = 16f;
                thumbText.SetTextColor(Android.Graphics.Color.White);

                var thumbViewIcon = ((LinearLayout)thumbView.FindViewById(Resource.Id.seekBarIcon));
                thumbViewIcon.SetBackgroundDrawable(Android.App.Application.Context.GetDrawable(Resource.Drawable.Oval));

                isInitilized = true;
                ChangeIntervalIcon();
            }
        }
    }


    public static class FloatExtensions
    {
        public static float DpToPixels(this float valueInDp, Context context)
        {
            DisplayMetrics metrics = context.Resources.DisplayMetrics;
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, valueInDp, metrics);
        }
    }
}
