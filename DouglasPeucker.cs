using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics
{
    public class DouglasPeucker
    {
        private List<Point> _pointsList;
        private double _errorBound;
        public DouglasPeucker(List<Point> pointsList, double errorBound)
        {
            _pointsList = new List<Point>();
            if (pointsList != null)
            {
                Point last = pointsList.First();
                _pointsList.Add(last);

                for (int i = 1; i < pointsList.Count; i++)
                {
                    if (!last.Equals(pointsList[i]))
                    {
                        _pointsList.Add(pointsList[i]);
                        last = pointsList[i];
                    }
                }
            }
            _errorBound = errorBound;
        }

        public List<Point> Compress()
        {
            if (_pointsList == null || _pointsList.Count <= 2)
            {
                return _pointsList;
            }

            List<Point> result = CompressHelper(_pointsList);
            result.Add(_pointsList.Last());

            return result;
        }

        private List<Point> CompressHelper(List<Point> pointsList)
        {
            if (pointsList.Count < 2)
            {
                return pointsList;
            }

            List<Point> result = new List<Point>();

            Line line = new Line() { p1 = pointsList.First(), p2 = pointsList.Last() };

            double maxDistance = 0;
            int maxIndex = 0;

            for (int i = 1; i < pointsList.Count - 1; i++)
            {
                var distance = Distance(pointsList[i], line);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxIndex = i;
                }
            }

            if (maxDistance <= _errorBound)
            {
                result.Add(pointsList.First());
            }
            else
            {
                var r1 = CompressHelper(pointsList.GetRange(0, maxIndex + 1));
                var r2 = CompressHelper(pointsList.GetRange(maxIndex, pointsList.Count - maxIndex));
                result.AddRange(r1);
                result.Add(pointsList[maxIndex]);
                result.AddRange(r2);
            }
            
            return result;
        }

        private double Distance(Point p, Line line)
        {
            var p1 = line.p1;
            var p2 = line.p2;
            return Math.Abs(
                    ((p2.Lng - p1.Lng) * p.Lat + (p1.Lat - p2.Lat) * p.Lng + (p1.Lng - p2.Lng) * p1.Lat + (p2.Lat - p1.Lat) * p1.Lng) /
                    Math.Sqrt((p2.Lng - p1.Lng) * (p2.Lng - p1.Lng) + (p1.Lat - p2.Lat) * (p1.Lat - p2.Lat))
                );
        }
    }

    public class Point
    {
        public double Lat { set; get; }
        public double Lng { set; get; }
        public bool Equals(Point p)
        {
            return Lat == p.Lat && Lng == p.Lng;
        }
    }

    class Line
    {
        public Point p1 { set; get; }
        public Point p2 { set; get; }
    }
}
