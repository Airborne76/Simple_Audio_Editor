using Simple_Audio_Editor.Helpers;
using Simple_Audio_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Playback;
using Windows.Storage;

namespace Simple_Audio_Editor.Models
{
    public class AudioClip: Observable
    {
        public ObservableCollection<TimePoint> Clips { get; set; }
        public TimePoint StartTime
        {
            get
            {
                return Clips[0].timeDouble < Clips[1].timeDouble ? Clips[0] : Clips[1];
            }
        }
        public TimePoint EndTime
        {
            get
            {
                return Clips[0].timeDouble > Clips[1].timeDouble ? Clips[0] : Clips[1];
            }
        }
        public double FirstTimeLength
        {
            get
            {
                return StartTime.timeDouble / StartTime.fulltime;
            }
        }
        public double SecondTimeLength
        {
            get
            {
                return (EndTime.timeDouble- StartTime.timeDouble)/ StartTime.fulltime;
            }
        }
        public double ThirdTimeLength
        {
            get
            {
                if (EndTime.fulltime - EndTime.timeDouble>=0)
                {
                    return (EndTime.fulltime - EndTime.timeDouble) / StartTime.fulltime;
                }
                else
                {
                    return 0;
                }
            }
        }

        private MediaComposition composition;

        public ICommand Clip
        {
            get
            {
                return new RelayCommand(async() =>
               {
                   if (MainViewModel.Current!=null)
                   {
                       var clip = await MediaClip.CreateFromFileAsync(MainViewModel.Current.audioFile);
                       clip.TrimTimeFromStart = StartTime.timeSpanFromStart;
                       clip.TrimTimeFromEnd = EndTime.timeSpanFromEnd;
                       composition = new MediaComposition();
                       composition.Clips.Add(clip);
                       MediaPlayer mediaPlayer = new MediaPlayer();
                       mediaPlayer.Source = MediaSource.CreateFromMediaStreamSource(composition.GeneratePreviewMediaStreamSource(0, 0));
                       mediaPlayer.Play();

                       // var picker = new Windows.Storage.Pickers.FileSavePicker();
                       // picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
                       // picker.FileTypeChoices.Add("MP3 files", new List<string>() { ".mp3" });
                       // //picker.FileTypeChoices.Add("MP3 files", new List<string>() { ".mp4" });
                       // picker.SuggestedFileName = "TrimmedClip.mp3";                       
                       // StorageFile file = await picker.PickSaveFileAsync();
                       //if (file != null)
                       //{
                       //    var saveOperation = composition.RenderToFileAsync(file, MediaTrimmingPreference.Precise);
                       //}

                   }
               });
            }
        }
    }
}
