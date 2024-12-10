﻿using DocumentFormat.OpenXml.Vml;

namespace BackEnd.Models.OutputModels
{
    public class AdminHomeDetailsModel
    {
        public RealEstatePropertyHomeDetails RealEstatePropertyHomeDetails { get; set; } = new RealEstatePropertyHomeDetails();
        public RequestHomeDetails RequestHomeDetails { get; set; } = new RequestHomeDetails();
        public int TotalCustomers { get; set; }
        public int TotalAgents { get; set; }
    }

    public class RealEstatePropertyHomeDetails
    {
        public int Total { get; set; }
        public int TotalSale { get; set; }
        public int TotalRent { get; set; }
        public int TotalLastMonth { get; set; }
        public int[] TotalCreatedPerMonth { get; set; }
        public int MaxAnnual { get; set; }
        public int MinAnnual { get; set; }
    }

    public class RequestHomeDetails
    {
        public int Total { get; set; }
        public int TotalArchived { get; set; }
        public int TotalClosed { get; set; }
        public int TotalActive { get; set; }
        public int TotalLastMonth { get; set; }
        public int TotalSale { get; set; }
        public int TotalRent { get; set; }
        public int[] TotalCreatedPerMonth { get; set; }
        public int MaxAnnual { get; set; }
        public int MinAnnual { get; set; }
    }
}
