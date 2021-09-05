using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrowbarWebsite.Models
{
    public class Coord
    {
        private double _lat = 0, _long = 0;

        public double Latitude => _lat;
        public double Longtitude => _long;

        public Coord(double latitude, double longtitude)
        {
            _lat = latitude;
            _long = longtitude;
        }
    }
}
