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
using System.ComponentModel;
using System.Windows.Media.Animation;

namespace AnimatedPhotoLine
{
    /// <summary>
    /// Interaction logic for FlippingPhotoCard.xaml
    /// </summary>
    public partial class FlippingPhotoCard : ScatterViewItem
    {
        //instance vars
        public bool Reversed = false;
        public Storyboard FlipToBack;
        public Storyboard FlipToFront;
        private List<string> _score;
        private int _arrayIndex;
        public string _imageString;
        public event PropertyChangedEventHandler PropertyChanged;
        //public Thumbnail _thumbnail;


        //propertiesm, the getters and setters, the way to reaach ivs
        public List<string> TagList
        {
            set
            {
                _score = value;
                OnPropertyChanged("Score");
            }

            get { return _score; }
        }

        public string ImageString
        {

            get { return _imageString; }
            set
            {
                _imageString = value;
            }
        }

        public int ArrayIndex
        {
            get { return _arrayIndex; }
            set
            {
                _arrayIndex = value;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
          
        public string FrontImage
        {
            set
            {
                imgFront.Source = new BitmapImage(new Uri(value));
            }

            get
            {
                return imgFront.Source.ToString();
            }
        }

        public string BackImage
        {
            set
            {
                imgBack.Source = new BitmapImage(new Uri(value));
            }

            get
            {
                return imgBack.Source.ToString();
            }
        }

        //public string ThumbImage
        //{
        //    set
        //    {
        //        _thumbnail.ThumbImage.Source = new BitmapImage(new Uri(value));
        //    }

        //    get
        //    {
        //        return _thumbnail.ThumbImage.Source.ToString();
        //    }
        //}

        //public Thumbnail Thumbnail
        //{
        //    get
        //    {
        //        return _thumbnail;
        //    }
        //}

        //constructor: method that makes itself
        public FlippingPhotoCard()
        {
            InitializeComponent();
            FlipToBack = this.FindResource("sbFlip") as Storyboard;
            FlipToBack.Completed += new EventHandler(sbFlip_Completed);
            FlipToFront = this.FindResource("sbReverse") as Storyboard;
            FlipToFront.Completed += new EventHandler(sbReverse_Completed);
            //_thumbnail = new Thumbnail();
            _score = new List<string>();
            this.Name = "copy";
            //_thumbnail.MyPhotoCard = this;
        }

        //method
        public FlippingPhotoCard DeepClone()
        {
            FlippingPhotoCard copy = new FlippingPhotoCard();
            copy.imgFront.Source = this.imgFront.Source;
            copy.imgFront.Stretch = Stretch.Uniform;
            copy.imgBack.Source = this.imgBack.Source;
            //copy.ImageDescription.Content = this.ImageDescription.Content;
            //copy._score = this._score;
            //Thumbnail t = new Thumbnail();
            //t.ThumbImage.Source = this.imgFront.Source;
            //copy._thumbnail = t;
            //copy._thumbnail.MyPhotoCard = this;
            return copy;
        }

        //method
        void sbReverse_Completed(object sender, EventArgs e)
        {
            Reversed = false;
        }
        //m
        void sbFlip_Completed(object sender, EventArgs e)
        {
            Reversed = true;
        }

        //m
        public void Flip()
        {
            if (!Reversed)
            {
                FlipToBack.Begin();
            }
        }

        public void Reverse()
        {
            if (Reversed)
                FlipToFront.Begin();
        }

        #region PhotoCard button definitions
        //being used
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ScatterView svParent = this.Parent as ScatterView;

            svParent.Items.Remove(this);
        }
        //bring used
        private void VoteButton_Click(object sender, RoutedEventArgs e)
        {
            this.Flip();
        }

        //this is being used
        private void BackToImageButton_Click(object sender, RoutedEventArgs e)
        {
            this.Reverse();
        }

        //being used
        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //is used
        private void TagButton_Click(object sender, RoutedEventArgs e)
        {
            SurfaceButton tagButton = sender as SurfaceButton;
            if (this.TagList.Contains(tagButton.Content.ToString()))
            {
                this.TagList.Remove(tagButton.Content.ToString());
                this.Background = Brushes.Gray;
            }
            else
            {
                this.TagList.Add(tagButton.Content.ToString());
                tagButton.Background = Brushes.CornflowerBlue;
            }
        }
#endregion    
    }
}
