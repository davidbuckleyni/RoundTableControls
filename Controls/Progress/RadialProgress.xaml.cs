using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RoundTableUkCustomControls.Controls.Progress
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(true)]
    [Category("Appearance")]
    public partial class RadialProgress : ContentPage
    {
        SKPaintSurfaceEventArgs args;
        ProgressUtils progressUtils = new ProgressUtils();
      
        public int StepCount { get; set; }

        public int TotalRecords { get; set; }

        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
                                                        propertyName: "ProgressText",
                                                        returnType: typeof(string),
                                                        declaringType: typeof(RadialProgress),
                                                        defaultValue: "",
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        propertyChanged: ProgressTextPropertyChanged);


        public static readonly BindableProperty ProgressMinValue = BindableProperty.Create(
                                                        propertyName: "ProgressMinValue",
                                                        returnType: typeof(int),
                                                        declaringType: typeof(RadialProgress),
                                                        defaultValue: "",
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        propertyChanged: ProgressMinValuePropertyChanged);

        public static readonly BindableProperty ProgressValue = BindableProperty.Create(
                                                     propertyName: "ProgressValue",
                                                     returnType: typeof(int),
                                                     declaringType: typeof(RadialProgress),
                                                     defaultValue: "",
                                                     defaultBindingMode: BindingMode.TwoWay,
                                                     propertyChanged: ProgressValuePropertyChanged);


        public static readonly BindableProperty ProgressMaxValue = BindableProperty.Create(
                                                        propertyName: "ProgressMaxValue",
                                                        returnType: typeof(int),
                                                        declaringType: typeof(RadialProgress),
                                                        defaultValue: "",
                                                        defaultBindingMode: BindingMode.TwoWay,
                                                        propertyChanged: ProgressMaxValuePropertyChanged);


        public EventHandler<Progress> progress;
        public int ProgressPercentage;

        public RadialProgress()
        {
            InitializeComponent();
            initiateProgressUpdate();

        }
        private void SetStatus(object sender, Progress e)
        {
            
        }
        public EventHandler ClickedCommand
        {
            get { return null; }
         //   set { this.buttonItem.Clicked += value; }
        }

        public string ProgressText
        {
            get { return base.GetValue(TitleTextProperty).ToString(); }
            set { base.SetValue(TitleTextProperty, value); }
        }
        private static void ProgressMinValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RadialProgress)bindable;
         //   control.ProgressText = newValue.ToString();
        }
        private static void ProgressValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RadialProgress)bindable;
            control.animateProgress((int)newValue);

        }
        private static void ProgressMaxValuePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RadialProgress)bindable;
          //  control.ProgressText = newValue.ToString();
        }
        
        private static void ProgressTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (RadialProgress)bindable;
            control.ProgressText = newValue.ToString();
        }
      

        // Initializing the canvas & drawing over it
        // Check here https://stackoverflow.com/questions/52893416/xamarin-forms-async-task-signature-return-type-of-eventhandler-doesnt-matc
        async void OnCanvasViewPaintSurfaceAsync(object sender, SKPaintSurfaceEventArgs args1)

        {
            args = args1;
            await drawGaugeAsync();

        }

        // Event Handler for Switch Toggle
        void switchToggledAsync(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            initiateProgressUpdate();
        }


        void sliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (canvas != null)
            {
                // Invalidating surface due to data change
                canvas.InvalidateSurface();
            }
        }


        // Animating the Progress of Radial Gauge
        async void animateProgress(int progress)
        {
            sw_listToggle.IsEnabled = false;
            sweepAngleSlider.Value = 1;

            // Looping at data interval of 5
            for (int i = 0; i < progress; i = i + 5)
            {
                sweepAngleSlider.Value = i;
                await Task.Delay(3);
            }
            ProgressPercentage = progress;
            sweepAngleSlider.Value = progress;
            sw_listToggle.IsEnabled = true;

        }

        void initiateProgressUpdate()
        {
            if (sw_listToggle.IsToggled)
                animateProgress(progressUtils.getSweepAngle(TotalRecords, TotalRecords));
            else
                animateProgress(progressUtils.getSweepAngle(TotalRecords / 100, TotalRecords));
        }

        public async Task drawGaugeAsync()
        {
            // Radial Gauge Constants
            int uPadding = 150;
            int side = 500;
            int radialGaugeWidth = 25;

            // Line TextSize inside Radial Gauge
            int lineSize1 = 220;
            int lineSize2 = 70;
            int lineSize3 = 80;

            // Line Y Coordinate inside Radial Gauge
            int lineHeight1 = 100;
            int lineHeight2 = 200;
            int lineHeight3 = 300;

            // Start & End Angle for Radial Gauge
            float startAngle = -220;
            float sweepAngle = 260;

        

                // Getting Canvas Info 
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;
                progressUtils.setDevice(info.Height, info.Width);
                canvas.Clear();

                // Getting Device Specific Screen Values
                // -------------------------------------------------

                // Top Padding for Radial Gauge
                float upperPading = progressUtils.getFactoredHeight(uPadding);

                /* Coordinate Plotting for Radial Gauge
                *
                *    (X1,Y1) ------------
                *           |   (XC,YC)  |
                *           |      .     |
                *         Y |            |
                *           |            |
                *            ------------ (X2,Y2))
                *                  X
                *   
                *To fit a perfect Circle inside --> X==Y
                *       i.e It should be a Square
                */

                // Xc & Yc are center of the Circle
                int Xc = info.Width / 2;
                float Yc = progressUtils.getFactoredHeight(side);

                // X1 Y1 are lefttop cordiates of rectange
                int X1 = (int)(Xc - Yc);
                int Y1 = (int)(Yc - Yc + upperPading);

                // X2 Y2 are rightbottom cordiates of rectange
                int X2 = (int)(Xc + Yc);
                int Y2 = (int)(Yc + Yc + upperPading);

         
                //  Empty Gauge Styling
                SKPaint paint1 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.FromHex("#e0dfdf").ToSKColor(),                   // Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Filled Gauge Styling
                SKPaint paint2 = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = Color.FromHex("#05c782").ToSKColor(),                   // Overlay Colour of Radial Gauge
                    StrokeWidth = progressUtils.getFactoredWidth(radialGaugeWidth), // Overlay Width of Radial Gauge
                    StrokeCap = SKStrokeCap.Round                                   // Round Corners for Radial Gauge
                };

                // Defining boundaries for Gauge
                SKRect rect = new SKRect(X1, Y1, X2, Y2);


                //canvas.DrawRect(rect, paint1);
                //canvas.DrawOval(rect, paint1);

                // Rendering Empty Gauge
                SKPath path1 = new SKPath();
                path1.AddArc(rect, startAngle, sweepAngle);
                canvas.DrawPath(path1, paint1);

                // Rendering Filled Gauge
                SKPath path2 = new SKPath();
                path2.AddArc(rect, startAngle, (float)sweepAngleSlider.Value);
                canvas.DrawPath(path2, paint2);

                //---------------- Drawing Text Over Gauge ---------------------------

                // Achieved Minutes
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#676a69");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize1);
                    skPaint.Typeface = SKTypeface.FromFamilyName(
                                        "Arial",
                                        SKFontStyleWeight.Bold,
                                        SKFontStyleWidth.Normal,
                                        SKFontStyleSlant.Upright);

                    // Drawing Achieved Minutes Over Radial Gauge
                    if (sw_listToggle.IsToggled)
                        canvas.DrawText(TotalRecords + "", Xc, Yc + progressUtils.getFactoredHeight(lineHeight1), skPaint);
                    else
                        canvas.DrawText(TotalRecords + "", Xc, Yc + progressUtils.getFactoredHeight(lineHeight1), skPaint);
                }

                // Achieved Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#676a69");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize2);
                    canvas.DrawText("Minutes", Xc, Yc + progressUtils.getFactoredHeight(lineHeight2), skPaint);
                }

                // Goal Minutes Text Styling
                using (SKPaint skPaint = new SKPaint())
                {
                    skPaint.Style = SKPaintStyle.Fill;
                    skPaint.IsAntialias = true;
                    skPaint.Color = SKColor.Parse("#e2797a");
                    skPaint.TextAlign = SKTextAlign.Center;
                    skPaint.TextSize = progressUtils.getFactoredHeight(lineSize3);

                    // Drawing Text Over Radial Gauge
                    if (sw_listToggle.IsToggled)
                        canvas.DrawText(ProgressText + TotalRecords + " record(s)", Xc, Yc + progressUtils.getFactoredHeight(lineHeight3), skPaint);
                    else
                    {
                        canvas.DrawText(ProgressText + TotalRecords / 100 + " record(s)", Xc, Yc + progressUtils.getFactoredHeight(lineHeight3), skPaint);
                    }
                }

           
        }
    }
}