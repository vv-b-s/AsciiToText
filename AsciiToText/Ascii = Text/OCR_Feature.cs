/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;*/
using System.Threading.Tasks;

/*using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;*/

//using TinyIoC;
using Tesseract;
//using Tesseract.Droid;
using XLabs.Ioc;
//using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

using Ascii___Text;


// http://thatcsharpguy.com/post/tesseract-ocr-xamarin/
namespace OCR_Feature
{
    public class OCR
    {
        private readonly ITesseractApi _tesseractApi;
        private readonly IDevice _device;

        public string pictureText { set; get; }

        private OCR()
        {
            _tesseractApi = Resolver.Resolve<ITesseractApi>();
            _device = Resolver.Resolve<IDevice>();
        }

        public static OCR CeatePicture()            // Instantiates class
        {
            OCR picture = new OCR();
            picture.GetString();
            return picture;
        }

        public void GetString() => PB_Clicked();
        

        async void PB_Clicked()
        {
            try
            {
                if (!_tesseractApi.Initialized)
                    await _tesseractApi.Init("eng");

                var photo = await TakePic();
                if (photo != null)
                {
                    var imageBytes = new byte[photo.Source.Length];
                    photo.Source.Position = 0;
                    photo.Source.Read(imageBytes, 0, (int)photo.Source.Length);
                    photo.Source.Position = 0;

                    var tessResult = await _tesseractApi.SetImage(imageBytes);
                    if (tessResult)
                    {
                       MainActivity.OCRtext = _tesseractApi.Text;
                    }
                }
            }
            catch(TaskCanceledException)
            {
                MainActivity.OCRtext = "";
            }
        }

        private async Task<MediaFile> TakePic()
        {
            var mediaStorageOptions = new CameraMediaStorageOptions
            {
                DefaultCamera = CameraDevice.Rear
            };

            var mediaFile = await _device.MediaPicker.TakePhotoAsync(mediaStorageOptions);

            return mediaFile;
        }

        
    }
}