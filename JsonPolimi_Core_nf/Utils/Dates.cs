using System;

namespace JsonPolimi_Core_nf.Utils
{
    public static class Dates
    {
        public static DateTime? DataFromString(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;
            if (data == "null")
                return null;

            if (data.Contains("T"))
            {
                //2019-07-27T17:13:23.5409603+02:00
                var a1 = data.Split('T');

                //2019-07-27
                var a2 = a1[0].Split('-');

                //17:13:23.5409603+02:00
                var a3 = a1[1].Split('+');

                //17:13:23.5409603
                var a4 = a3[0].Split('.');

                //17:13:23
                var a5 = a4[0].Split(':');

                return new DateTime(Convert.ToInt32(a2[0]), Convert.ToInt32(a2[1]), Convert.ToInt32(a2[2]), Convert.ToInt32(a5[0]), Convert.ToInt32(a5[1]), Convert.ToInt32(a5[2]));
            }

            if (data.Contains("."))
            {
                //2019-07-29 18:26:55.034083
                data = data.Split('.')[0];

                //2019-07-29 18:26:55
                var b1 = data.Split(' ');

                //2019-07-29
                var b2 = b1[0].Split('-');

                //18:26:55
                var b3 = b1[1].Split(':');

                return new DateTime(Convert.ToInt32(b2[0]), Convert.ToInt32(b2[1]), Convert.ToInt32(b2[2]), Convert.ToInt32(b3[0]), Convert.ToInt32(b3[1]), Convert.ToInt32(b3[2]));
            }

            //27/07/2019 11:42:24
            var s1 = data.Split(' ');

            //27/07/2019
            var s2 = s1[0].Split('/');

            //11:42:24
            var s3 = s1[1].Split(':');

            return new DateTime(Convert.ToInt32(s2[2]), Convert.ToInt32(s2[1]), Convert.ToInt32(s2[0]), Convert.ToInt32(s3[0]), Convert.ToInt32(s3[1]), Convert.ToInt32(s3[2]));
        }
    }
}