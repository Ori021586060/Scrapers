using System;
using System.Collections.Generic;
using System.Text;

namespace ScraperModels.Models.OnmapDto
{
    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Scale CurrentScale { get; set; }
        public Point Clone()
        {
            return new Point() {
                Latitude = Latitude,
                Longitude = Longitude,
                CurrentScale = CurrentScale
            };
        }
        public void SetZ(Scale scale)
        {
            CurrentScale = scale;
        }
        public void ToRight()
        {
            //Latitude = Latitude,
            Longitude = Longitude + CurrentScale.Width;
        }
        
        public void ToNextLine(Scale scale)
        {
            Latitude = Latitude + scale.Height / 2 + CurrentScale.Height / 2;
            double lng = (double)BorderModel.M1.Longitude;
            Longitude = lng;

            SetZ(scale);
        }

        public bool Consist(Point point, Scale scale)
        {
            var result = true;

            if ((point.Latitude > Latitude + scale.Height / 2) || (point.Latitude < Latitude - scale.Height / 2) ||
                    (point.Longitude < Longitude - scale.Width / 2) || (point.Longitude > Longitude + scale.Width / 2)) result = false;

            return result;
        }
        public bool OutRightBorder()
        {
            var result = false;

            if (BorderModel.M4.Longitude < Longitude - CurrentScale.Width / 2)
                result = true;

            return result;
        }
        
        public bool OutTopBorder()
        {
            var result = false;

            if (BorderModel.M2.Latitude < Latitude - CurrentScale.Width / 2)
                result = true;

            return result;
        }
    }
}
