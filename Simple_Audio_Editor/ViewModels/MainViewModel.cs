﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Simple_Audio_Editor.Convert;
using Simple_Audio_Editor.Helpers;
using Simple_Audio_Editor.Models;
using Simple_Audio_Editor.Views;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Simple_Audio_Editor.ViewModels
{
    public class MainViewModel : Observable
    {
        public MainViewModel()
        {
            Current = this;
        }

        public static MainViewModel Current;
        private IMediaPlaybackSource _source;

        public StorageFile audioFile;
        public ObservableCollection<TimePoint> timePoints = new ObservableCollection<TimePoint>();
        public ObservableCollection<AudioClip> audioClips = new ObservableCollection<AudioClip>();
        private int ID = 0;
       
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
            //Debug.WriteLine(TimeSpan);
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
                    timePoints.Add(new TimePoint() { timeSpanFromStart = timeSpanstart, timeSpanFromEnd = timespanend,fulltime=Maxposition });
                    if (timePoints.Count==1)
                    {
                        Debug.WriteLine(timeSpanstart.ToLyricTimeString());
                    }
                    if (timePoints.Count==2)
                    {
                        if (!(timePoints[0].timeDouble==timePoints[1].timeDouble))
                        {
                            ObservableCollection<TimePoint> points = new ObservableCollection<TimePoint>();
                            foreach (var point in timePoints)
                            {
                                points.Add(point);
                            }
                            audioClips.Add(new AudioClip() { Clips = points ,ID=ID});
                            ID++;
                            timePoints.Clear();
                        }
                        else
                        {
                            timePoints.RemoveAt(1);

                        }
                    }
                    //Debug.WriteLine($"{ timeSpanstart}{ timespanend}");
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

        private bool isSourceLoaded=false;
        public bool IsSourceLoaded
        {
            get { return isSourceLoaded; }
            set
            {
                Set(ref isSourceLoaded, value);
                if (value)
                {
                    SelectedVisibility = Visibility.Collapsed;
                    PlayVisibility = Visibility.Visible;
                }
            }
        }

        private Visibility selectedVisibility=Visibility.Visible;
        public Visibility SelectedVisibility
        {
            get { return selectedVisibility; }
            set { Set(ref selectedVisibility, value); }
        }

        private Visibility playVisibility = Visibility.Collapsed;
        public Visibility PlayVisibility
        {
            get { return playVisibility; }
            set { Set(ref playVisibility, value); }
        }

        private Visibility musicVisibility = Visibility.Collapsed;
        public Visibility MusicInfoVisibility
        {
            get { return musicVisibility; }
            set { Set(ref musicVisibility, value); }
        }

        private MusicInfo musicInfo = new MusicInfo();
        public MusicInfo MusicInfo
        {
            get { return musicInfo; }
            set
            {
                Set(ref musicInfo, value);
                if (value.bitmapImage!=null)
                {
                    MusicInfoVisibility = Visibility.Visible;
                }
            }
        }

        public ICommand GetMusicInfo
        {
            get
            {
                return new RelayCommand(async() =>
                {
                    if (MusicInfo.musicProperties!=null)
                    {
                        var dialog = new MusicInfoDialog(MusicInfo);
                        await dialog.ShowAsync();
                    }
                });
            }
        }

        public void Initialize()
        {
            mediaPlayer.AutoPlay = false;
            mediaPlayer.Volume = MediaVolume/100;
            //mediaPlayer.PlaybackSession
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

        public async Task LoadFileAsync(StorageFile file)
        {
            audioFile = file;
            using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem))
            {
                var info = await file.Properties.GetMusicPropertiesAsync();
                MusicInfo musicInfo = new MusicInfo();
                if (thumbnail != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.SetSource(thumbnail);
                    musicInfo.bitmapImage = bitmapImage;
                }
                if (info != null)
                {
                    musicInfo.musicProperties = info;
                }
                MusicInfo = musicInfo;
            }
            mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);
            await Task.Run(async () =>
            {
                while (mediaPlayer.PlaybackSession.NaturalDuration.TotalMinutes == 0)
                {
                    await Task.Delay(1);
                }
            });
            Maxposition = mediaPlayer.PlaybackSession.NaturalDuration.TotalMinutes;
            IsSourceLoaded = true;
            audioClips.Clear();
            timePoints.Clear();

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
                    await LoadFileAsync(file);
                });
            }
        }

        private ICommand _getStorageItemsCommand;
        public ICommand GetStorageItemsCommand => _getStorageItemsCommand ?? (_getStorageItemsCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnGetStorageItem));

        public async void OnGetStorageItem(IReadOnlyList<IStorageItem> items)
        {
            //foreach (var item in items)
            //{
            //    Debug.WriteLine(item.Name);
            //    item
            //    //TODO WTS: Process storage item
            //}
            if (items.Count>0)
            {
                if (items[0]!=null)
                {
                    var file = items[0] as StorageFile;
                    if (file.FileType.Equals(".mp3")|| file.FileType.Equals(".wma")|| file.FileType.Equals(".wav"))
                    {
                        await LoadFileAsync(file);
                    }

                }
            }

        }

    }
}
