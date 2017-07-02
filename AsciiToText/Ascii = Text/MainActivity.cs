using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware;


using Translate;
using OCR_Feature;

using TinyIoC;
using Tesseract;
using Tesseract.Droid;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;
using Android.Text;
//using XLabs.Platform.Services.Media;

namespace Ascii___Text
{
    [Activity(Label = "Ascii to Text", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        #region UI controllers
        static TextView LabelText, LabelAscii;
        static EditText TextBox, AsciiBox;
        Spinner spinner;
        Button PhotoBT;
        #endregion

        private int _base = 0;                                        //gets the position of the spinner
        public static string OCRtext { set; get; }                  // Stores the OCR text

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Objects Declaration
            LabelText = FindViewById<TextView>(Resource.Id.LabelText);
            LabelAscii = FindViewById<TextView>(Resource.Id.LabelAscii);
            TextBox = FindViewById<EditText>(Resource.Id.TextBox);
            AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);
            spinner = FindViewById<Spinner>(Resource.Id.spinner);
            PhotoBT = FindViewById<Button>(Resource.Id.photoBT);

            // Code itself

            #region OCR                                       // http://thatcsharpguy.com/post/tesseract-ocr-xamarin/

            int numCameras = Camera.NumberOfCameras;        // used to turn off the "Take a photo" feature if there's no camera available | https://stackoverflow.com/questions/1944117/check-if-device-has-a-camera

            if (numCameras > 0)
            {
                var container = TinyIoCContainer.Current;

                container.Register(AndroidDevice.CurrentDevice);
                container.Register<ITesseractApi>((cont, parapeters) =>
                {
                    return new TesseractApi(ApplicationContext, AssetsDeployment.OncePerInitialization);
                });

                Resolver.ResetResolver();
                Resolver.SetResolver(new TinyResolver(container));
            }
            else PhotoBT.Visibility = ViewStates.Invisible;


            #endregion

            #region Spinner
            //https://developer.xamarin.com/guides/android/user_interface/spinner/

            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);                 // Creates the event for spinner
            var enumValues = Enum.GetValues(typeof(Translator.Base));                                                         // https://stackoverflow.com/questions/37354738/spinner-with-enum-values-in-xamarin?rq=1
            var arrayForAdapter = enumValues.Cast<Translator.Base>().Select(e => e.ToString()).ToArray();
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, arrayForAdapter);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerItem);
            spinner.Adapter = adapter;          // Displays the item on the spinner itself

            #endregion Spinner

            TextBox.TextChanged += OnTextBoxTextChange;
            AsciiBox.TextChanged += OnAsciiBoxTextChange;

            PhotoBT.Click += OnPhotoBTCLick;                           // Gets the OCR text


            PhotoBT.LongClick += OnPhotoBTLongClick;                       // Sends the OCR text to the focused box
        }

        private void OnTextBoxTextChange(object sender, TextChangedEventArgs e)
        {
            if (TextBox.IsFocused)
            {
                LabelText.Text = (TextBox.Text == "") ? "Enter your text here:" : "Your text is:";
                LabelAscii.Text = (TextBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";
                AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, (Translator.Base)_base);
                Translator.InputText = TextBox.Text;

                if (TextBox.Text == "")
                    AsciiBox.Text = "";
            }
        }

        private void OnAsciiBoxTextChange(object sender, TextChangedEventArgs e)
        {
            if (AsciiBox.IsFocused)
            {
                LabelText.Text = (AsciiBox.Text == "") ? "Enter your text here:" : "Your text is:";
                LabelAscii.Text = (AsciiBox.Text == "") ? "Or enter your Ascii code here:" : "Your text in Ascii is:";

                if (Translatable())
                    TextBox.Text = Translator.ConvertTo(Translator.Type.Text, AsciiBox.Text, (Translator.Base)_base);

                Translator.InputText = TextBox.Text;

                if (AsciiBox.Text == "")
                    TextBox.Text = "";
            }
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            _base = e.Position;

            // Connecting Fields
            var TextBox = FindViewById<EditText>(Resource.Id.TextBox);
            var AsciiBox = FindViewById<EditText>(Resource.Id.AsciiBox);

            //Conversion Code
            switch (e.Position)
            {
                case (int)Translator.Base.Binary:
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Binary);      break;
                case (int)Translator.Base.Decimal:
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Decimal);     break;
                case (int)Translator.Base.Hexadecimal:
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Hexadecimal); break;
                case (int)Translator.Base.Octadecimal:
                    AsciiBox.Text = Translator.ConvertTo(Translator.Type.Ascii, TextBox.Text, Translator.Base.Octadecimal); break;
            }
        }

        private void OnPhotoBTCLick(object sender, EventArgs e)
        {
            OCR.CeatePicture();
            TextBox.Text = "After taking the photo wait for a few seconds and hold the \"Take a photo\" button to view the text.";
            AsciiBox.Text = "";
        }

        private void OnPhotoBTLongClick(object sender, View.LongClickEventArgs e)
        {
            if (TextBox.IsFocused)
                TextBox.Text = OCRtext;
            else
                AsciiBox.Text = OCRtext;
        }

        private bool Translatable()                                    // Checks if Ascii can be converted into text
        {
            switch ((Translator.Base)_base)
            {
                case Translator.Base.Binary: // Base 2
                    return !Regex.IsMatch(AsciiBox.Text, @"[^01\s]");

                case Translator.Base.Decimal:
                    return !Regex.IsMatch(AsciiBox.Text, @"[^0-9\s]");

                case Translator.Base.Hexadecimal:
                    return !Regex.IsMatch(AsciiBox.Text, @"[^a-fA-F0-9\s]");

                case Translator.Base.Octadecimal:
                    return !Regex.IsMatch(AsciiBox.Text, @"[^0-9\s]");
            }
            return false;
        }
    }
}