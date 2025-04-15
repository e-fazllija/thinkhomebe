using System.ComponentModel.DataAnnotations;

namespace BackEnd.Entities
{
    public class DocumentsTab : EntityBase
    {
        public bool IdentificationDocument { get; set; }
        public string? IdentificationDocumentUrl { get; set; }

        public bool TaxCodeOrHealthCard { get; set; }
        public string? TaxCodeOrHealthCardUrl { get; set; }

        public bool MarriageCertificateSummary { get; set; }
        public string? MarriageCertificateSummaryUrl { get; set; }

        public bool DeedOfOrigin { get; set; }
        public string? DeedOfOriginUrl { get; set; }

        public bool SystemsComplianceDeclaration { get; set; }
        public string? SystemsComplianceDeclarationUrl { get; set; }

        public bool ElectricalElectronicSystem { get; set; }
        public string? ElectricalElectronicSystemUrl { get; set; }

        public bool PlumbingSanitarySystem { get; set; }
        public string? PlumbingSanitarySystemUrl { get; set; }

        public bool GasSystem { get; set; }
        public string? GasSystemUrl { get; set; }

        public bool HeatingAirConditioningSystem { get; set; }
        public string? HeatingAirConditioningSystemUrl { get; set; }

        public bool LiftingSystem { get; set; }
        public string? LiftingSystemUrl { get; set; }

        public bool FireSafetySystem { get; set; }
        public string? FireSafetySystemUrl { get; set; }

        public bool BoilerMaintenanceLog { get; set; }
        public string? BoilerMaintenanceLogUrl { get; set; }

        public bool HabitabilityCertificate { get; set; }
        public string? HabitabilityCertificateUrl { get; set; }

        public bool StructuralIntegrityCertificate { get; set; }
        public string? StructuralIntegrityCertificateUrl { get; set; }

        public bool BuildingCadastralComplianceReport { get; set; }
        public string? BuildingCadastralComplianceReportUrl { get; set; }

        public bool LandRegistry { get; set; }
        public string? LandRegistryUrl { get; set; }

        public bool CadastralSurveyAndFloorPlan { get; set; }
        public string? CadastralSurveyAndFloorPlanUrl { get; set; }

        public bool CadastralMapExtract { get; set; }
        public string? CadastralMapExtractUrl { get; set; }

        public bool FloorPlanWithSubsidiaryUnits { get; set; }
        public string? FloorPlanWithSubsidiaryUnitsUrl { get; set; }

        public bool EnergyPerformanceCertificate { get; set; }
        public string? EnergyPerformanceCertificateUrl { get; set; }

        public bool MortgageLienRegistrySearch { get; set; }
        public string? MortgageLienRegistrySearchUrl { get; set; }

        public bool Condominium { get; set; }
        public string? CondominiumUrl { get; set; }

        public bool CondominiumBylaws { get; set; }
        public string? CondominiumBylawsUrl { get; set; }

        public bool MillesimalTables { get; set; }
        public string? MillesimalTablesUrl { get; set; }

        public bool LatestFinancialStatementAndBudget { get; set; }
        public string? LatestFinancialStatementAndBudgetUrl { get; set; }

        public bool LastTwoCondominiumMeetingMinutes { get; set; }
        public string? LastTwoCondominiumMeetingMinutesUrl { get; set; }

        public bool SignedStatementFromAdministrator { get; set; }
        public string? SignedStatementFromAdministratorUrl { get; set; }

        public bool ChamberOfCommerceBusinessRegistrySearch { get; set; }
        public string? ChamberOfCommerceBusinessRegistrySearchUrl { get; set; }

        public bool PowerOfAttorney { get; set; }
        public string? PowerOfAttorneyUrl { get; set; }

        public bool UrbanPlanningComplianceCertificate { get; set; }
        public string? UrbanPlanningComplianceCertificateUrl { get; set; }

        public bool LeaseAgreement { get; set; }
        public string? LeaseAgreementUrl { get; set; }

        public bool LastMortgagePaymentReceipt { get; set; }
        public string? LastMortgagePaymentReceiptUrl { get; set; }

        public bool TaxDeductionDocumentation { get; set; }
        public string? TaxDeductionDocumentationUrl { get; set; }

        public bool PurchaseOffer { get; set; }
        public string? PurchaseOfferUrl { get; set; }

        public bool CommissionAgreement { get; set; }
        public string? CommissionAgreementUrl { get; set; }

        public bool PreliminarySaleAgreement { get; set; }
        public string? PreliminarySaleAgreementUrl { get; set; }

        public bool DeedOfSale { get; set; }
        public string? DeedOfSaleUrl { get; set; }

        public bool MortgageDeed { get; set; }
        public string? MortgageDeedUrl { get; set; }

        public bool MiscellaneousDocuments { get; set; }
        public string? MiscellaneousDocumentsUrl { get; set; }
        public string? RealEstatePropertyId { get; set; }
    }
}
