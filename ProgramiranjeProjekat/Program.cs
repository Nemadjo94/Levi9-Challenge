using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProgramiranjeProjekat
{
    class Program
    {
        #region method for entering & converting the json file
        public static string enterJson()
        {
            try
            {
                Console.Write("\nPlease enter json file path: ");//Enter value
                String path = Console.ReadLine();//Read line
                dynamic obj = JObject.Parse(File.ReadAllText(path));//Read file
                Console.WriteLine("Json loaded succesfully...\n");

                String route = obj.ToString();//Convert json to string
                                              //Routes routes = JsonConvert.DeserializeObject<Routes>(obj);//Convert
                var routes = JsonConvert.DeserializeObject<dynamic>(route);
                //Console.WriteLine(routes.lines[0].name.ToString());
                return route;

            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: " + exc.Message.ToString() + "\n");
                return enterJson();
            }
        }
        #endregion
        #region methods for latitude and longitude
        //Latitude
        public static double latitude()
        {
            try
            {
                Console.Write("Please enter latitude: ");
                double lat = Convert.ToDouble(Console.ReadLine());
                if (lat > 90 || lat < -90)
                {
                    Console.WriteLine("Error: Latitude must be in range from -90 to 90.");
                    return latitude();
                }
                else return lat;

            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: " + exc.Message.ToString() + "\n");
                return latitude();
            }
        }
        //Longitude
        public static double longitude()
        {
            try
            {
                Console.Write("Please enter longitude: ");
                double lon = Convert.ToDouble(Console.ReadLine());
                if (lon > 180 || lon < -180)
                {
                    Console.WriteLine("Error: Longitude must be in range from -180 to 180.");
                    return longitude();
                }
                else return lon;

            }
            catch (Exception exc)
            {
                Console.WriteLine("Error: " + exc.Message.ToString() + "\n");
                return longitude();
            }
        }
        #endregion
        #region method for current time input
        public static void timeInput()
        {
            Console.Write("\nPlease enter your current time in this format 'HH:mm' : ");
            String time = Console.ReadLine();
            try
            {
                DateTime myTime;
                if (DateTime.TryParse(time, out myTime))
                {
                    time = myTime.ToString("HH:mm");
                    Console.WriteLine("\nYour current time is: " + time + "\n");
                }

                //Validate hours
                string hour = time.Substring(0, 2);
                int hourInt = int.Parse(hour);
                if (hourInt >= 24)
                {
                    Console.WriteLine("Error: An hour cannot be longer than 24...");
                    timeInput();
                }
                //Validate minutes
                string minutes = time.Substring(time.Length - 2);

                int minutesInt = int.Parse(minutes);
                if (minutesInt >= 60)
                {
                    Console.WriteLine("Error: A minute cannot be longer than 60...");
                    timeInput();
                }
            }
            catch (Exception exc)
            {
                //Console.WriteLine("Error: " + exc.Message.ToString() + "\n");
                Console.WriteLine("Error: Input is not valid, try again.\n");
                timeInput();
            }
        }
        #endregion
        #region methods for calculating the distance between two GPS coordinates and walking distance
        public static double DistanceBetweenCoordinates(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }
        public static string WalkingTime(double x)
        {
            x = (x / 4) * 60;
            x = Math.Ceiling(x);
            string xString = Convert.ToString(x);
            return xString;
        }
        #endregion
        #region method for comparing the inputed lat and long with the ones from json file

        public static double GetClosestLat(double lat, string json)
        {
            var test = JsonConvert.DeserializeObject<dynamic>(json);//deserialize json            
            double[] lat_list = new double[test.stops.Count];//Array with the length of all the bus stations       
            for (int i = 0; i < lat_list.Length; i++)//add lat values to array
            {
                lat_list[i] = test.stops[i].lat;
            }
            double closest = lat_list.OrderBy(item => Math.Abs(lat - item)).First();//Is equal to the closest number           
            return closest;
        }

        public static double GetClosestLon(double lon, string json)
        {
            dynamic test = JsonConvert.DeserializeObject<dynamic>(json);//deserialize json
            double[] lon_list = new double[test.stops.Count];//Array with the length of all the bus stations       
            for (int i = 0; i < lon_list.Length; i++)//add lat values to array
            {
                lon_list[i] = test.stops[i].lon;
            }
            double closest = lon_list.OrderBy(item => Math.Abs(lon - item)).First();//Is equal to the closest number
            return closest;
        }
        #endregion
        #region method for printing the closest bus stations name and the bus lanes
        public static void GetBusStopName(double lat, double lon, string json)
        {

            dynamic test = JsonConvert.DeserializeObject<dynamic>(json);//deserialize json
            for (int i = 0; i < test.stops.Count; i++)
            {

                double stopsLat = Convert.ToDouble(test.stops[i].lat);
                double stopsLon = Convert.ToDouble(test.stops[i].lon);

                if (lat.Equals(stopsLat) || lon.Equals(stopsLon))
                {

                    Console.Write(test.stops[i].name);
                    //Console.Write(" Lines that go from it: ");
                   // foreach (var x in test.stops[i].lines)
                   // {
                   //     Console.Write(x.ToString() + ", ");
                   // }
                    break;//Break after first one
                }
            }

        }
        #endregion
        #region method for printing the bus lane which user must take
        public static void GetBusLane(double startLat, double startLon, double endLat, double endLon, string json)
        {


            dynamic test = JsonConvert.DeserializeObject<dynamic>(json);//deserialize json
            string[] line = new string[3];
            string[] line2 = new string[3];
            string[] commonLines = new string[3];

            double stopsLat;
            double stopsLon;

            for (int i = 0; i < test.stops.Count; i++)
            {

                stopsLat = Convert.ToDouble(test.stops[i].lat);
                stopsLon = Convert.ToDouble(test.stops[i].lon);

                if (startLat.Equals(stopsLat) || startLon.Equals(stopsLon))
                {
                    line = new string[test.stops[i].lines.Count];

                    for (int j = 0; j < test.stops[i].lines.Count; j++)
                    {

                        line[j] = test.stops[i].lines[j];
                    }

                }
                if (endLat.Equals(stopsLat) || endLon.Equals(stopsLon))
                {
                    line2 = new string[test.stops[i].lines.Count];

                    for (int j = 0; j < test.stops[i].lines.Count; j++)
                    {

                        line2[j] = test.stops[i].lines[j];
                    }
                }

            }

            bool noCommonLines = true;

            for (int i = 0; i < line.Length; i++)
            {
                for (int j = 0; j < line2.Length; j++)
                {
                    if (line[i].Equals(line2[j]))
                    {
                        commonLines[j] = line2[j];
                        noCommonLines = false;
                        Console.Write("\nTake this bus line/s: ");
                        Console.Write(line2[j]);
                        Console.Write(" to reach your destination");
                    }
                    
                }
            }

            

            string resenje = "";
            string presedanje = "";
            string linija = "";
            string[] bothLines = line.Concat(line2).ToArray();

            if (noCommonLines)
            {
                for (int i = 0; i < test.stops.Count; i++)
                {
                    string[] temp = new string[test.stops[i].lines.Count];                  
                    bool uslov1 = false;
                    bool uslov2 = false;

                    for (int j = 0; j < temp.Length; j++)
                    {
                        temp[j] = test.stops[i].lines[j];

                        for (int y = 0; y < line.Length; y++)
                        {
                            if (temp[j].Equals(line[y]))
                            {
                                uslov1 = true;
                                linija = line[y];
                                
                            }
                        }
                        for (int y = 0; y < line2.Length; y++)
                        {
                            if (temp[j].Equals(line2[y]))
                            {
                                uslov2 = true;
                                presedanje = line2[y];
                                
                            }
                        }
                        if (uslov1 && uslov2)
                        {
                            resenje = test.stops[i].name;
                        }
                    }
                }
                Console.WriteLine("\nTake line: " + linija + ". Exit at: " + resenje + " and take the: " + presedanje + " line.");
            }
        
    








        }
        #endregion

        static void Main(string[] args)
        {
            string json = enterJson();
            do
            {               
                //Get time
                //timeInput();

                //Starting point
                Console.WriteLine("\nPlease enter your starting coordinates...");
                double startingLat = latitude();                
                double startingLong = longitude();
                
                //Ending point
                Console.WriteLine("\nPlease enter your ending coordinates...");
                double endingLat = latitude();
                double endingLong = longitude();

                //Write out the GPS coordinates
                //Console.WriteLine("\nYour starting points are: " + startingLat.ToString() + " Latitude, " + startingLong.ToString() + " Longitude");
                //Console.WriteLine("\nYour ending points are: " + endingLat.ToString() + " Latitude, " + endingLong.ToString() + " Longitude");

                #region closest starting point
                //Closest starting point
                double closestStartLat = GetClosestLat(startingLat, json);               
                double closestStartLong = GetClosestLon(startingLong, json);

                //Get bus stop and bus lanes names printed
                Console.Write("\nGo to: ");
                GetBusStopName(closestStartLat, closestStartLong, json);

                //Console.WriteLine("\nClosest starting Latitude is: " + closestStartLat.ToString());
                //Console.WriteLine("\nClosest starting Longitude is: " + closestStartLong.ToString());
                //Distance between stating point and closest point               
                double distance = DistanceBetweenCoordinates(startingLat, startingLong, closestStartLat, closestStartLong);
                  if (distance < 1)
                {
                    double distanceMin = distance * 1000;
                    string distanceString = distanceMin.ToString();
                    distanceString = distanceString.Split('.')[0];
                    Console.WriteLine("\nDistance: " + distanceString + "m");
                }
                else Console.WriteLine("\nDistance: " + distance.ToString() + "km");

                //Walking time
                string walkingTime = WalkingTime(distance);
                Console.WriteLine("Walking time: " + walkingTime.ToString() + "min");
                #endregion

                

                #region closest ending point
                //Closest starting point
                double closestEndLat = GetClosestLat(endingLat, json);
                double closestEndLong = GetClosestLon(endingLong, json);
                //Get bus lane
                GetBusLane(closestStartLat, closestStartLong, closestEndLat, closestEndLong, json);

                //Get bus stop and bus lanes names printed
                Console.Write("\nThen exit at: ");
                GetBusStopName(closestEndLat, closestEndLong, json);

                //Console.WriteLine("\nClosest end Latitude is: " + closestEndLat.ToString());
                //Console.WriteLine("\nClosest end Longitude is: " + closestEndLong.ToString());
                //Distance between stating point and closest point
                double distance2 = DistanceBetweenCoordinates(endingLat, endingLong, closestEndLat, closestEndLong);
                if (distance2 < 1)
                {
                    double distanceMin = distance2 * 1000;
                    string distanceString = distanceMin.ToString();
                    distanceString = distanceString.Split('.')[0];
                    Console.WriteLine("\nDistance to goal: " + distanceString + "m");
                }
                else Console.WriteLine("\nDistance to goal: " + distance2.ToString() + "km");

                //Walking time
                string walkingTime2 = WalkingTime(distance2);
                Console.WriteLine("Walking time: " + walkingTime2.ToString() + "min");
                #endregion

                


            } while (true);
        }
    }
}
