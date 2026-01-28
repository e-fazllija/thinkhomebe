using BackEnd.Entities;

namespace BackEnd.Models.OutputModels
{
    public class DashboardResponse
    {
        public string Role { get; set; } = string.Empty;
        public string ScopeAgencyId { get; set; } = string.Empty;
        public string ScopeUserId { get; set; } = string.Empty;
        public int TotalCustomers { get; set; }
        public int TotalAgents { get; set; }
        public RealEstatePropertyHomeDetails RealEstatePropertyHomeDetails { get; set; } = new RealEstatePropertyHomeDetails();
        public RequestHomeDetails RequestHomeDetails { get; set; } = new RequestHomeDetails();
        public CalendarDetails CalendarDetails { get; set; } = new CalendarDetails();
        public SalesDetails SalesDetails { get; set; } = new SalesDetails();
        public AgencySummary? AgencySummary { get; set; }
        public AgentSelfDetails? AgentSelf { get; set; }
    }

    public class AgencySummary
    {
        public int TotalCustomers { get; set; }
        public int TotalAgents { get; set; }
    }

    public class AgentSelfDetails
    {
        public int PropertiesTotal { get; set; }
        public int PropertiesActive { get; set; }
        public int PropertiesArchivedOrSold { get; set; }
        public int RequestsTotal { get; set; }
        public int RequestsActive { get; set; }
    }

    public class CalendarDetails
    {
        public int Total { get; set; }
        public int Today { get; set; }
        public int ThisWeek { get; set; }
        public int ThisMonth { get; set; }
        public int Confirmed { get; set; }
        public int Cancelled { get; set; }
        public int Postponed { get; set; }
        public int Upcoming { get; set; }
        public int LinkedToProperties { get; set; }
        public int LinkedToRequests { get; set; }
        public IDictionary<string, int> ByType { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> ByLocation { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> ByAgent { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> ByDayOfWeek { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> CreatedPerMonth { get; set; } = new Dictionary<string, int>();
        public List<UpcomingAppointment> UpcomingAppointments { get; set; } = new List<UpcomingAppointment>();
    }

    public class UpcomingAppointment
    {
        public int Id { get; set; }
        public string NomeEvento { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? LuogoEvento { get; set; }
        public DateTime DataInizioEvento { get; set; }
        public DateTime DataFineEvento { get; set; }
        public bool Confirmed { get; set; }
        public bool Cancelled { get; set; }
        public bool Postponed { get; set; }
        public string AgentName { get; set; } = string.Empty;
        public string? PropertyTitle { get; set; }
    }

    public class AgentDetail
    {
        public string Name { get; set; } = string.Empty;
        public int PropertiesManaged { get; set; }
        public int Acquisitions { get; set; }
        public int AppointmentsEvasi { get; set; }
        public int AppointmentsDisdetti { get; set; }
        public int AppointmentsConfermati { get; set; }
        public int AppointmentsEffettuati { get; set; }
        public int TotalAppointments { get; set; }
    }

    public class SalesDetails
    {
        public int TotalSold { get; set; }
        public int SoldThisMonth { get; set; }
        public int SoldLastMonth { get; set; }
        public double TotalSalesValue { get; set; }
        public double TotalSalesValueThisMonth { get; set; }
        public double TotalCommissions { get; set; }
        public double TotalCommissionsThisMonth { get; set; }
        public IDictionary<string, int> SoldPerMonth { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, double> SalesValuePerMonth { get; set; } = new Dictionary<string, double>();
    }

    public class AgentStatsDetails
    {
        public int TotalPropertiesManaged { get; set; }
        public int ActivePropertiesManaged { get; set; }
        public int TotalAcquisitions { get; set; }
        public int AcquisitionsThisMonth { get; set; }
        public int TotalAppointments { get; set; }
        public int AppointmentsEvasi { get; set; }
        public int AppointmentsDisdetti { get; set; }
        public int AppointmentsConfermati { get; set; }
    }

    public class LoadedProperty
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public bool Active { get; set; }
    }

    public class DashboardDataResponse
    {
        public string Role { get; set; } = string.Empty;
        public string ScopeAgencyId { get; set; } = string.Empty;
        public string ScopeUserId { get; set; } = string.Empty;
        public int TotalCustomers { get; set; }
        public int TotalAgents { get; set; }
        public RealEstatePropertyHomeDetails RealEstatePropertyHomeDetails { get; set; } = new RealEstatePropertyHomeDetails();
        public RequestHomeDetails RequestHomeDetails { get; set; } = new RequestHomeDetails();
        public AgentStatsDetails AgentStats { get; set; } = new AgentStatsDetails();
        public List<AgentDetail> AgentDetails { get; set; } = new List<AgentDetail>();
        public List<LoadedProperty> LoadedProperties { get; set; } = new List<LoadedProperty>();
        public AgencySummary? AgencySummary { get; set; }
        public AgentSelfDetails? AgentSelf { get; set; }
    }
}

