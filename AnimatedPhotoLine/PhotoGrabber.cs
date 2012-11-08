using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AnimatedPhotoLine
{
    class PhotoGrabber
    {
        private List<string> _photosList;

        /// <summary>
        /// C. Valdes
        /// Returns a list of Strings that contains the paths to all the large .jpg files
        /// </summary>
        /// <returns>Array of String paths to jpg images from all directories</returns>
        public string[] PhotosArray
        {
            get
            {
                return _photosList.ToArray();
            }
        }

        /// <summary>
        /// Constructor: Creates a list of strings for all the files that end in .jpg
        /// </summary>
        /// <param name="directory">String path to photos</param>
        public PhotoGrabber(string directory)
        {
            _photosList = new List<string>();

            try
            {
                string[] _photos_array = Directory.GetFiles(directory, "*.jpg");
                foreach (string s in _photos_array)
                {
                    _photosList.Add(s);                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        
    }
}
