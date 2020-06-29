using System;
using CoreAnimation;
using CoreGraphics;
using Devhouse.GradientSlider.Abstractions;
using Devhouse.GradientSlider.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomGradientSlider), typeof(GradientSlider))]
namespace Devhouse.GradientSlider.iOS
{
    public class GradientSlider : SliderRenderer
    {
        new public static void Init() { DateTime temp = DateTime.Now; }
        CustomGradientSlider control = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            control = (CustomGradientSlider)Element;
        }

        public override void Draw(CGRect rect)
        {
            rect.Height = (nfloat)Element.HeightRequest;

            var value = (int)(Math.Round(Control.Value / control.Interval) * control.Interval);
            nfloat gradientWidth;

            if (control.HasSegment)
            {
                Control.SetValue(value, false);
                gradientWidth = (Control.Frame.Size.Width / (int)Element.Maximum) * value;
            }
            else
            {
                value = (int)Control.Value;
                gradientWidth = (Control.Frame.Size.Width / (int)Element.Maximum) * value;
            }

            try
            {
                Control.SetMinTrackImage(GetGradientImage(gradientWidth, rect.Size, new CGColor[] { control.TrackStartColor.ToCGColor(), control.TrackEndColor.ToCGColor() }), UIControlState.Normal);
                Control.SetMaxTrackImage(GetGradientImage(Control.Frame.Size.Width, rect.Size, new CGColor[] { control.TrackColor.ToCGColor(), control.TrackColor.ToCGColor() }), UIControlState.Normal);
                Control.SetThumbImage(GetThumbImage(new CGRect(0, 0, 30, 30), (int)Control.Value), UIControlState.Normal);
            }
            catch
            {
                Control.MinimumTrackTintColor = control.TrackStartColor.ToUIColor();
                Control.MaximumTrackTintColor = control.TrackColor.ToUIColor();
            }
            //Control.Continuous = false;
            Control.ValueChanged += Control_ValueChanged;
            base.Draw(rect);
        }

        private void Control_ValueChanged(object sender, EventArgs e)
        {
            var value = (int)(Math.Round(Control.Value / control.Interval) * control.Interval);
            nfloat gradientWidth;

            if (control.HasSegment)
            {
                Control.SetValue(value, false);
                gradientWidth = (Control.Frame.Size.Width / (int)Element.Maximum) * value;
            }
            else
            {
                value = (int)Control.Value;
                gradientWidth = (Control.Frame.Size.Width / (int)Element.Maximum) * value;
            }

            try
            {
                Control.SetMinTrackImage(GetGradientImage(gradientWidth, new CGSize(Control.Frame.Size.Width, Element.HeightRequest), new CGColor[] { control.TrackStartColor.ToCGColor(), control.TrackEndColor.ToCGColor() }), UIControlState.Normal);
                Control.SetMaxTrackImage(GetGradientImage(Control.Frame.Size.Width, new CGSize(Control.Frame.Size.Width, Element.HeightRequest), new CGColor[] { control.TrackColor.ToCGColor(), control.TrackColor.ToCGColor() }), UIControlState.Normal);
                Control.SetThumbImage(GetThumbImage(new CGRect(0, 0, 30, 30), value), UIControlState.Normal);
            }
            catch
            {
                Control.MinimumTrackTintColor = control.TrackStartColor.ToUIColor();
                Control.MaximumTrackTintColor = control.TrackColor.ToUIColor();
            }
        }

        private UIImage GetGradientImage(nfloat width, CGSize size, CGColor[] colors)
        {
            var gradientLayer = new CAGradientLayer();
            gradientLayer.Frame = new CGRect(0, 0, width, size.Height);
            gradientLayer.CornerRadius = gradientLayer.Frame.Height / 2;
            gradientLayer.MasksToBounds = true;
            gradientLayer.Colors = colors;
            gradientLayer.StartPoint = new CGPoint(x: 0.0, y: 0.5);
            gradientLayer.EndPoint = new CGPoint(x: 1.0, y: 0.5);

            if (control.HasSegment)
            {
                var segmentDelay = (size.Width - 10) / (Element.Maximum / control.Interval);

                for (int i = 0; i <= Element.Maximum / control.Interval; i++)
                {
                    var segmentLayer = new CALayer();
                    var myImage = i * control.Interval < Control.Value ? new UIImage(control.MaxIntervaImageSource) : new UIImage(control.MinIntervaImageSource);
                    segmentLayer.Contents = myImage.CGImage;
                    segmentLayer.Frame = new CGRect(i == 0 ? 10 : segmentDelay * i, (size.Height - 5) / 2, 5, 5);
                    gradientLayer.AddSublayer(segmentLayer);
                }
            }

            UIGraphics.BeginImageContextWithOptions(size, gradientLayer.Opaque, 0.0f);
            var context = UIGraphics.GetCurrentContext();
            gradientLayer.RenderInContext(context);
            var image = UIGraphics.GetImageFromCurrentImageContext().CreateResizableImage(new UIEdgeInsets(top: 0, left: size.Height, bottom: 0, right: size.Height));
            UIGraphics.EndImageContext();
            return image;
        }

        public UIImage GetThumbImage(CGRect rect, int value)
        {
            var mainThumb = new CALayer();
            var myImage = new UIImage("Oval.png");
            mainThumb.Contents = myImage.CGImage;
            mainThumb.Frame = rect;
            var textlayer = new CATextLayer();
            textlayer.FontSize = 16;
            textlayer.TextAlignmentMode = CATextLayerAlignmentMode.Center;
            textlayer.String = value.ToString();
            textlayer.Wrapped = true;
            textlayer.TextTruncationMode = CATextLayerTruncationMode.End;
            //textlayer.BackgroundColor = UIColor.White.CGColor;
            textlayer.ForegroundColor = UIColor.White.CGColor;
            var yCenter = (rect.Height - textlayer.FontSize) / 2 - textlayer.FontSize / 10;
            textlayer.Frame = new CGRect(rect.X, yCenter, rect.Width, rect.Height);
            mainThumb.AddSublayer(textlayer);

            UIGraphics.BeginImageContextWithOptions(rect.Size, mainThumb.Opaque, 0.0f);
            var context = UIGraphics.GetCurrentContext();
            mainThumb.RenderInContext(context);
            var image = UIGraphics.GetImageFromCurrentImageContext().CreateResizableImage(new UIEdgeInsets(top: 0, left: rect.Size.Height, bottom: 0, right: rect.Size.Height));
            UIGraphics.EndImageContext();
            return image;
        }
    }
}
