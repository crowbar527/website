using CrowbarWebsite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Anywhere.ArcGIS.Common;
using Anywhere.ArcGIS.Operation;

namespace CrowbarWebsite.Models
{
    public class StaticCamera
    {
        private string _street = "", _area = "", _type = "";
        private Coord _startpos, _endpos;
        private DateTime _installed;
        private QueryResponse<Point> _crashinfo;

        public string Street => _street;
        public string Area => _area;

        internal string GetInternalName()
        {
            return $"{_street}.{_area}";
        }

        public Coord StartPos => _startpos;
        public Coord EndPos => _endpos;
        public DateTime Installed => _installed;
        public string Type => _type;
        public QueryResponse<Point> crashInfo => _crashinfo;

        public void setCrashInfo(QueryResponse<Point> data)
        {
            _crashinfo = data;
        }

        public static StaticCamera FromXML(XmlReader xmlReader)
        {
            var sc = new StaticCamera();
            var items = xmlReader.ReadElement("camera");
            if (items.Count == 0)
                return null;
            sc._type = items["type"];
            sc._street = items["street"];
            sc._area = items["area"];
            sc._startpos = new Coord(double.Parse(items["startpos.lat"]), double.Parse(items["startpos.long"]));
            sc._endpos = new Coord(double.Parse(items["endpos.lat"]), double.Parse(items["endpos.long"]));
            sc._installed = DateTime.ParseExact(items["installed"], "MMM-yyyy", null);
            return sc;
        }
    }


}
