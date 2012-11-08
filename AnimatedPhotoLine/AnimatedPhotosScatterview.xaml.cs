using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading;

namespace AnimatedPhotoLine
{
    /// <summary>
    /// Interaction logic for AnimatedPhotosScatterview.xaml
    /// </summary>
    public partial class AnimatedPhotosScatterview : ScatterView
    {
        #region Variables & Constants

        private const int FLIPPINGPHOTOCARDWIDTHandHEIGHT = 300;

        private int off_screen_counter = 0;
        private bool _ok_to_update = true;
        public Random _random = new Random();

        private DispatcherTimer _timer = new DispatcherTimer();
        private DispatcherTimer _fade_timer = new DispatcherTimer();
        private Dispatcher mainDispatcher = Dispatcher.CurrentDispatcher;
        private DispatcherTimer _change_timer = new DispatcherTimer();

        private List<ScatterViewItem> _fade_list = new List<ScatterViewItem>();
        private string[] _photosArray;

        #endregion

        public AnimatedPhotosScatterview()
        {
            InitializeComponent();

            //set to size of display
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Width = SystemParameters.PrimaryScreenWidth;

            FindPhotosAndStartImageContainerCreation();

            StartAnimationTimers();
        }

        private void FindPhotosAndStartImageContainerCreation()
        {
            string currentDir = Environment.CurrentDirectory;
            int index = currentDir.IndexOf("bin");
            currentDir = currentDir.Substring(0, index);
            string imagesPath = currentDir + "Resources\\PhotosForAnimation\\";

            PhotoGrabber photoGrabber = new PhotoGrabber(imagesPath);
            _photosArray = photoGrabber.PhotosArray;
            //add imageSVIs to scatter ScatterView
            for (int i = 0; i < 13; i++)
            {
                CreateImageContainer(_photosArray[i]);
            }
        }

        private void StartAnimationTimers()
        {            
            // Create a timer to update the imageSVIs every 10 milliseconds.
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            _timer.Tick += UpdateImagePositions;
            _timer.Start();

            //Create a timer that will update image containers every two seconds
            _change_timer = new DispatcherTimer();
            _change_timer.Interval = new TimeSpan(0, 0, 0, 2, 0);
            _change_timer.Tick += new EventHandler(_change_timer_Tick);
            _change_timer.Start();            
        }

        /// <summary>
        /// Create FlippingPhotoCards from image path string.
        /// </summary>
        /// <param name="photoPath"></param>
        public void CreateImageContainer(string photoPath)
        {
            BitmapImage bitmapImageForFrontAndBack = new BitmapImage(new Uri(photoPath));
            double w = bitmapImageForFrontAndBack.Width;
            double h = bitmapImageForFrontAndBack.Height;

            //photocard
            FlippingPhotoCard imageCard = new FlippingPhotoCard();
            imageCard.imgFront.Source = bitmapImageForFrontAndBack;
            //imageCard.imgFront.Stretch = Stretch.Uniform;
            //imageCard.imgBack.Source = bitmapImageForFrontAndBack;
            imageCard.Name = "play";
            imageCard.CanScale = false;
            imageCard.ContainerManipulationStarted += new ContainerManipulationStartedEventHandler(svi_ContainerManipulationStarted);
            imageCard.Center = new Point(FLIPPINGPHOTOCARDWIDTHandHEIGHT * this.Items.Count + 1, this.Height / 2);

            imageCard.Orientation = 90;

            this.Items.Add(imageCard);
        }

        /// <summary>
        /// H. Wang, C. Valdes
        /// For each tick of the _timer, images are moved to the right if their name is "play" by incrementing their center x coordinate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateImagePositions(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                ScatterViewItem imageView = (ScatterViewItem)this.Items.GetItemAt(i);
                if (imageView.Name.Equals("play"))
                {
                    Point position = imageView.Center;
                    double newX = (position.X + 1);
                    // Set the new position of the imageViewer
                    Point newPosition = new Point(newX, position.Y);

                    if (newX > (this.Width + imageView.Width + 200))//beyond screen
                    {//get rid of old imageSVI and grab a new one
                        if (!_ok_to_update)
                        {
                            if (_fade_list.Contains(imageView))
                                _fade_list.Remove(imageView);

                            imageView.Center = new Point(-150, this.Height / 2);
                        }
                        else
                        {
                            _ok_to_update = false;
                            ThreadStart start = delegate()
                            {
                                int m = _random.Next(_photosArray.Length);
                                BitmapImage bitmapImageForImageFront = new BitmapImage(new Uri(_photosArray[m]));
                            };
                            new Thread(start).Start();
                        }
                    }
                    else//move to right
                    {
                        newX = (position.X + 1);
                        imageView.Center = newPosition;
                    }
                }
            }

        }

        /// <summary>
        /// Makes it so images are updated every 2 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _change_timer_Tick(object sender, EventArgs e)
        {
            _ok_to_update = true;
        }

        /// <summary>
        /// Called on photos in the river. Copies the one you touched 
        /// (shows up underneath original) to replace it in the river.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void svi_ContainerManipulationStarted(object sender, ContainerManipulationStartedEventArgs e)
        {
            FlippingPhotoCard originalSVI = sender as FlippingPhotoCard;
            this.Items.Add(TransferCopyToNewScatterView(originalSVI));
        }

        /// <summary>
        /// Takes an SVI, copies everything about it and 
        /// returns the copy as a "Copy." Won't be animated as if in river
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public FlippingPhotoCard TransferCopyToNewScatterView(FlippingPhotoCard original)
        {
            FlippingPhotoCard copyCard = (FlippingPhotoCard)original.DeepClone();
            original.Name = "Copy";
            original.ContainerManipulationStarted -= new ContainerManipulationStartedEventHandler(svi_ContainerManipulationStarted);
            //original.ContainerManipulationCompleted += new ContainerManipulationCompletedEventHandler(CopiedSVI_ContainerManipulationCompleted);
            copyCard.Name = "play";
            copyCard.BorderThickness = new Thickness(0);
            copyCard.CanScale = false;
            copyCard.Center = original.Center;
            copyCard.Orientation = original.Orientation;
            copyCard.ContainerManipulationStarted += new ContainerManipulationStartedEventHandler(svi_ContainerManipulationStarted);

            return copyCard;
        }

        
        }
    }

