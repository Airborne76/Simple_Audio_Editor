using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Simple_Audio_Editor.Convert;
using Simple_Audio_Editor.Helpers;
using Simple_Audio_Editor.Models;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Simple_Audio_Editor.ViewModels
{
    public class MainViewModel : Observable
    {
        public MainViewModel()
        {
        }
        private IMediaPlaybackSource _source;

        public ObservableCollection<TimePoint> timePoints = new ObservableCollection<TimePoint>();

        private bool UserChangeTime = false;

        public IMediaPlaybackSource Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }


        public void silder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            UserChangeTime = true;
        }
        public void silder_Pointerreleased(object sender, PointerRoutedEventArgs e)
        {
            mediaPlayer.PlaybackSession.Position = TimeSpan;
            Debug.WriteLine(TimeSpan);
            UserChangeTime = false;
        }
        private long pointTime = 0;
        public void processSilder_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (mediaPlayer.Source!=null&&Maxposition!=0)
            {
                pointTime = DateTime.Now.Ticks / 10000;
            }            
        }


        public void processSilder_Pointerreleased(object sender, PointerRoutedEventArgs e)
        {
            if (mediaPlayer.Source != null && Maxposition != 0)
            {
                if (DateTime.Now.Ticks / 10000 - pointTime < 300)
                {
                    TimeSpan timeSpanstart = TimeSpanSwtich.DoubleToTimeSpan((sender as Slider).Value);
                    TimeSpan timespanend = TimeSpanSwtich.DoubleToTimeSpan(Maxposition)-timeSpanstart;
                    timePoints.Add(new TimePoint() { timeSpanFromStart = timeSpanstart, timeSpanFromEnd = timespanend });
                    Debug.WriteLine($"{ timeSpanstart}{ timespanend}");

                }
                pointTime = 0;
            }
        }





        private TimeSpan timespan=TimeSpan.Zero;
        public TimeSpan TimeSpan
        {
            get { return timespan; }
            set
            {
                //if (!UserChangeTime)
                //{
                    Set(ref timespan, value);
                //}                
                if (UserChangeTime)
                {
                    //Set(ref timespan, value);
                    mediaPlayer.PlaybackSession.Position = value;
                }
            }
        }

        public MediaPlayer mediaPlayer = new MediaPlayer();

        public void Initialize()
        {
            mediaPlayer.AutoPlay = false;
            mediaPlayer.Volume = MediaVolume/100;

            mediaPlayer.PlaybackSession.PositionChanged += (m, e) =>
            {
                UIDispatcher.RunAsync(() =>
                {
                    if (!UserChangeTime)
                    {
                        TimeSpan = mediaPlayer.PlaybackSession.Position;
                    }                   
                    //Mediaposition = mediaPlayer.PlaybackSession.Position.TotalMinutes;
                    //Debug.WriteLine(Mediaposition);
                });
            };

            mediaPlayer.MediaEnded += (m, e) =>
            {
                UIDispatcher.RunAsync(() =>
                {
                    TogglePlay();
                    mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
                });
            };

        }

        private bool isplay = false;

        private SymbolIcon playstatus = new SymbolIcon() { Symbol=Symbol.Play};

        public SymbolIcon Playstatus
        {
            get { return playstatus; }
            set { Set(ref playstatus, value); }
        }

        private double mediaVolume=50;

        public double MediaVolume
        {
            get { return mediaVolume; }
            set
            {
                Set(ref mediaVolume, value);
                mediaPlayer.Volume = mediaVolume/100;
            }
        }

        private double mediaposition = 0;

        public double Mediaposition
        {
            get { return mediaposition; }
            set
            {
                Set(ref mediaposition, value);
            }
        }

        private double maxposition = 0;

        public double Maxposition
        {
            get { return maxposition; }
            set
            {
                Set(ref maxposition, value);
            }
        }


        public ICommand SwitchPlay
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (mediaPlayer.Source != null)
                    {
                        Debug.WriteLine(mediaPlayer.PlaybackSession.Position);
                        Debug.WriteLine(mediaPlayer.Volume);
                        Debug.WriteLine(mediaPlayer.PlaybackSession.NaturalDuration.TotalMinutes);
                        TogglePlay();
                    }                    
                });
            }
        }

        public void TogglePlay()
        {
            if (!isplay)
            {
                isplay = true;
                Playstatus = new SymbolIcon() { Symbol = Symbol.Pause };
                mediaPlayer.Play();

            }
            else
            {
                isplay = false;
                Playstatus = new SymbolIcon() { Symbol = Symbol.Play };
                mediaPlayer.Pause();
            }
        }

        public ICommand Select
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    //if (fileInputNode != null)
                    //{
                    //    fileInputNode.Dispose();
                    //}
                    FileOpenPicker filePicker = new FileOpenPicker();
                    filePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
                    filePicker.FileTypeFilter.Add(".mp3");
                    filePicker.FileTypeFilter.Add(".wma");
                    filePicker.FileTypeFilter.Add(".wav");
                    filePicker.ViewMode = PickerViewMode.Thumbnail;
                    StorageFile file = await filePicker.PickSingleFileAsync();
                    if (file == null)
                    {
                        return;
                    }
                    var info = await file.Properties.GetMusicPropertiesAsync();
                    //Source = MediaSource.CreateFromStorageFile(file);
                    mediaPlayer.Source= MediaSource.CreateFromStorageFile(file);
                    await Task.Delay(1000);
                    Maxposition = mediaPlayer.PlaybackSession.NaturalDuration.TotalMinutes;                    
                    //mediaPlayer.PlaybackSession

                    //MusicInfo = $"title:{info.Title}\r\n AlbumArtist:{info.AlbumArtist}";
                });
            }
        }




    }
}
