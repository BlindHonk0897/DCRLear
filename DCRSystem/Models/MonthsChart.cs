using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DCRSystem.Models
{
    [DataContract]
    public class MonthsChart
    {
        public MonthsChart(string label, double y)
        {
            this.Label = label;
            this.Y = y;
        }

        public MonthsChart(string label, bool isCumulativeSum, string indexLabel)
        {
            this.Label = label;
            this.IsCumulativeSum = isCumulativeSum;
            this.IndexLabel = indexLabel;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<double> Y = null;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "isCumulativeSum")]
        public bool IsCumulativeSum = false;

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "indexLabel")]
        public string IndexLabel = null;
    }
}