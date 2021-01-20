using System;
using System.IO;
using System.Data;
using System.Linq;
//using System.Collections.Generic;
using System.Text;
//Request library
using System.Net;


namespace DCAHighsConsole
{
    class Program
    {
        static DataTable GetTable()
        {
            DataTable DCATemps = new DataTable();
            DCATemps.Columns.Add("DateStamp", typeof(string));
            DCATemps.Columns.Add("Temp", typeof(int));
            String line;
            String Date;
            //Pass the file path and file name to the StreamReader constructor
            string[] Years = { "2018", "2019", "2020", "2021" }; // "2019", "2020",
            foreach (var Year in Years)
            {
                //# files = requests.get('https://www.ncei.noaa.gov/data/global-hourly/access/2019/').text
                //string filelocation = @"C:\Users\Tom\Documents\C#scripts\Weather\Reagan" + Year + ".csv";
                //StreamReader sr = new StreamReader(filelocation);
                string weatherURI = @"https://www.ncei.noaa.gov/data/global-hourly/access/" + Year + "/72405013743.csv";
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(weatherURI);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(stream))
                {
                    //Read the first line of text
                    line = sr.ReadLine();
                    line = sr.ReadLine();
                    //Continue to read until you reach end of file
                    while (line != null)
                    {
                        //write the lie to console window
                        line = line.Replace("\",\"", "`");
                        string[] lineArray = line.Split('`');
                        Date = (lineArray[1].Split('T')[0]);
                        string Temp = (lineArray[13].Split(',')[0]);
                        if (Temp != "+9999")
                        {
                            float TempC = (int.Parse(Temp.Replace('+', '0')));
                            DCATemps.Rows.Add(Date, TempC);
                        }
                        //Console.WriteLine(lineArray[1].Split('T')[1] + lineArray[13]);
                        //Read the next line
                        line = sr.ReadLine();
                    }
                    //close the file
                    sr.Close();
                    //Console.ReadLine();
                }
            }
            return DCATemps;
        }
 
        static void Main()
        {
            DataTable DCATemps = GetTable();
            var HighTemps = DCATemps.AsEnumerable()
                .GroupBy(r => r.Field<string>("DateStamp"))
                .Select(grp =>
                    new {
                        DateStamp = grp.Key,
                        MaxTemp = grp.Max(e => e.Field<int>("Temp"))
                    });
            foreach (var HighTemp in HighTemps)
            { Console.WriteLine("{0}, {1}, {2}", HighTemp.DateStamp, HighTemp.MaxTemp/10, HighTemp.MaxTemp/10*9/5+32); }
            Console.ReadLine();
        }
    }
}
