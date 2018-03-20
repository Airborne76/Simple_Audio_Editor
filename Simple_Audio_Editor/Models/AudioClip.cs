using Simple_Audio_Editor.Helpers;
using Simple_Audio_Editor.Services;
using Simple_Audio_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Simple_Audio_Editor.Models
{
    public class AudioClip : Observable
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
                return (EndTime.timeDouble - StartTime.timeDouble) / StartTime.fulltime;
            }
        }
        public double ThirdTimeLength
        {
            get
            {
                if (EndTime.fulltime - EndTime.timeDouble >= 0)
                {
                    return (EndTime.fulltime - EndTime.timeDouble) / StartTime.fulltime;
                }
                else
                {
                    return 0;
                }
            }
        }
        private MediaSource mediaStream;
        public MediaSource MediaStream
        {
            get { return mediaStream; }
            set { Set(ref mediaStream, value); }
        }
        public int ID { get; set; }

        private Visibility progressVisibility = Visibility.Collapsed;
        public Visibility ProgressVisibility
        {
            get { return progressVisibility; }
            set { Set(ref progressVisibility, value); }
        }

        private double fileSaveProgress = 0;
        public double FileSavedProgress
        {
            get { return fileSaveProgress; }
            set { Set(ref fileSaveProgress, value); }
        }

        private MediaComposition composition;

        public AudioClip()
        {
            ClipAudio();
        }
        public RoutedEventHandler LoadedEventHandler
        {
            get
            {
                return (e, s) =>
                {
                    if (MediaStream != null)
                    {
                        (e as MediaPlayerElement).Source = MediaStream;
                    }
                };
            }
        }



        public async void ClipAudio()
        {
            if (MainViewModel.Current != null)
            {
                var clip = await MediaClip.CreateFromFileAsync(MainViewModel.Current.audioFile);
                clip.TrimTimeFromStart = StartTime.timeSpanFromStart;
                clip.TrimTimeFromEnd = EndTime.timeSpanFromEnd;
                composition = new MediaComposition();
                composition.Clips.Add(clip);

                MediaStream = MediaSource.CreateFromMediaStreamSource(composition.GenerateMediaStreamSource(MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High)));
                Debug.WriteLine(MediaStream.State);
            }
        }

        public ICommand Save
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (composition != null)
                    {
                        var picker = new Windows.Storage.Pickers.FileSavePicker();
                        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
                        picker.FileTypeChoices.Add("MP3 files", new List<string>() { ".mp3" });
                        //picker.FileTypeChoices.Add("MP3 files", new List<string>() { ".mp4" });
                        picker.SuggestedFileName = "TrimmedClip.mp3";
                        StorageFile file = await picker.PickSaveFileAsync();

                        //(await file.Properties.GetMusicPropertiesAsync())
                        if (file != null)
                        {
                            var saveOperation = composition.RenderToFileAsync(file, MediaTrimmingPreference.Precise, MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High));
                            ProgressVisibility = Visibility.Visible;
                            saveOperation.Progress = new AsyncOperationProgressHandler<TranscodeFailureReason, double>((info, progress) =>
                            {
                                UIDispatcher.RunAsync(() =>
                                {
                                    //Debug.WriteLine(string.Format("Saving file... Progress: {0:F0}%", progress));
                                    FileSavedProgress = progress;
                                });
                            });
                            saveOperation.Completed = new AsyncOperationWithProgressCompletedHandler<TranscodeFailureReason, double>(async (info, status) =>
                            {
                                MusicProperties fileinfo = await file.Properties.GetMusicPropertiesAsync();
                                if (MainViewModel.Current.MusicInfo != null)
                                {
                                    var musicProperties = MainViewModel.Current.MusicInfo.musicProperties;
                                    fileinfo.Album = musicProperties.Album;
                                    fileinfo.AlbumArtist = musicProperties.AlbumArtist;
                                    fileinfo.Artist = musicProperties.AlbumArtist;
                                    fileinfo.Subtitle = musicProperties.Subtitle;
                                    fileinfo.Title = musicProperties.Title;
                                    fileinfo.Year = musicProperties.Year;
                                }
                                await fileinfo.SavePropertiesAsync();

                                UIDispatcher.RunAsync(() =>
                                {
                                    new ToastNotificationsService().ShowToastNotificationSample();
                                    ProgressVisibility = Visibility.Collapsed;
                                });
                            });
                        }
                    }
                });
            }
        }

        public ICommand Delete
        {
            get
            {
                return new RelayCommand(() =>
               {
                   if (composition != null)
                   {
                       //var clip = await MediaClip.CreateFromFileAsync(MainViewModel.Current.audioFile);
                       //clip.TrimTimeFromStart = StartTime.timeSpanFromStart;
                       //clip.TrimTimeFromEnd = EndTime.timeSpanFromEnd;
                       //composition = new MediaComposition();
                       //composition.Clips.Add(clip);

                       //MediaPlayer mediaPlayer = new MediaPlayer();
                       //mediaPlayer.Source = MediaSource.CreateFromMediaStreamSource(composition.GenerateMediaStreamSource());

                       //if (MainViewModel.Current != null)
                       //{
                       //    mediaPlayer.Volume = MainViewModel.Current.MediaVolume;
                       //}
                       //mediaPlayer.Play();
                       if (MainViewModel.Current != null)
                       {
                           if (MainViewModel.Current.audioClips.Where(i => i.ID == ID).Count() > 0)
                           {


                               MainViewModel.Current.audioClips.Remove(MainViewModel.Current.audioClips.Where(i => i.ID == ID).FirstOrDefault());
                               MediaStream.Dispose();
                           }
                       }

                   }
               });
            }
        }
    }
}
