using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DCRSystem.Models
{
    public class Validator
    {
        public static Boolean compareTwoDate(String dateInput ,String dateLast)
        {
          
            try
            {
                DateTime dt_date0 = DateTime.ParseExact(dateInput, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime dt_date1 = DateTime.ParseExact(dateLast, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (dt_date0 > dt_date1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERRRORRRRRRRR");
                return false;
            }
        }
        public static Boolean isNotPriorDate(String dateInput)
        {

            try
            {

                DateTime inputDate = DateTime.ParseExact(dateInput, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                return DateTime.Now.CompareTo(inputDate.Add(new TimeSpan(2, 0, 0))) <= 0;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static Boolean isNewlyHired(String dateInput)
        {

            try
            {

                DateTime inputDate = DateTime.ParseExact(dateInput, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                return (  DateTime.Now - inputDate).TotalDays <= 30;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static Boolean isValidDate(String dateInput)
        {

            try
            {

                DateTime inputDate = DateTime.ParseExact(dateInput, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }
}