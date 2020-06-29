using System;
using Xamarin.Forms;

namespace Devhouse.GradientSlider.Abstractions
{
    public class CustomGradientSlider : Slider
    {
        #region BindableProperties

        public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(
        propertyName: nameof(TrackColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomGradientSlider),
        defaultValue: Color.LightGray);

        public static readonly BindableProperty TrackStartColorProperty =
        BindableProperty.Create(
        propertyName: nameof(TrackStartColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomGradientSlider),
        defaultValue: Color.LightSkyBlue);

        public static readonly BindableProperty TrackEndColorProperty =
        BindableProperty.Create(
        propertyName: nameof(TrackEndColor),
        returnType: typeof(Color),
        declaringType: typeof(CustomGradientSlider),
        defaultValue: Color.SkyBlue);

        public static readonly BindableProperty HasSegmentProperty =
        BindableProperty.Create(
        propertyName: nameof(HasSegment),
        returnType: typeof(bool),
        declaringType: typeof(CustomGradientSlider),
        defaultValue: false);

        public static readonly BindableProperty IntervalProperty =
        BindableProperty.Create(
        propertyName: nameof(Interval),
        returnType: typeof(double),
        declaringType: typeof(CustomGradientSlider),
        defaultValue: default(double));

        public static readonly BindableProperty MinIntervaImageSourceProperty =
        BindableProperty.Create(
        propertyName: nameof(MinIntervaImageSource),
        returnType: typeof(string),
        declaringType: typeof(CustomGradientSlider));

        public static readonly BindableProperty MaxIntervaImageSourceProperty =
        BindableProperty.Create(
        propertyName: nameof(MaxIntervaImageSource),
        returnType: typeof(string),
        declaringType: typeof(CustomGradientSlider));

        #endregion

        #region Get Set's

        public Color TrackColor
        {
            get { return (Color)GetValue(TrackColorProperty); }
            set { SetValue(TrackColorProperty, value); }
        }

        public Color TrackStartColor
        {
            get { return (Color)GetValue(TrackStartColorProperty); }
            set { SetValue(TrackStartColorProperty, value); }
        }

        public Color TrackEndColor
        {
            get { return (Color)GetValue(TrackEndColorProperty); }
            set { SetValue(TrackEndColorProperty, value); }
        }

        public bool HasSegment
        {
            get { return (bool)GetValue(HasSegmentProperty); }
            set { SetValue(HasSegmentProperty, value); }
        }

        public double Interval
        {
            get { return (double)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }

        public string MinIntervaImageSource
        {
            get { return (string)GetValue(MinIntervaImageSourceProperty); }
            set { SetValue(MinIntervaImageSourceProperty, value); }
        }

        public string MaxIntervaImageSource
        {
            get { return (string)GetValue(MaxIntervaImageSourceProperty); }
            set { SetValue(MaxIntervaImageSourceProperty, value); }
        }

        #endregion
    }
}
