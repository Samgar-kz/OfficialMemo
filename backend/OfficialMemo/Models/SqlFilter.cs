using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficialMemo.Context;
using OfficialMemo.Models.Dbo;
using System.Globalization;
using Serialization = System.Text.Json.Serialization;

namespace OfficialMemo.Models;

public enum Operator
{
    Contains,
    Equals,
    NotContains,
    NotEquals,
    Between,
    NotBetween,
    GreaterThan,
    LessThan,
    NotGreaterThan,
    NotLessThan,
}

public class FilterRule<T>
{
    [Serialization.JsonPropertyName("operator")]
    public Operator Operator { get; set; } = Operator.Contains;

    [Serialization.JsonPropertyName("operand")]
    public T? Operand { get; set; }

    [Serialization.JsonPropertyName("left")]
    public T? Left { get; set; }

    [Serialization.JsonPropertyName("right")]
    public T? Right { get; set; }
}

public class FilterRules
{
    [Serialization.JsonPropertyName("processGuid")]
    public FilterRule<string?>? ProcessGuid { get; set; }

    [Serialization.JsonPropertyName("startDate")]
    public FilterRule<DateTime?>? StartDate { get; set; }

    [Serialization.JsonPropertyName("dueToDate")]
    public FilterRule<DateTime?>? DueToDate { get; set; }

    [Serialization.JsonPropertyName("regNum")]
    public FilterRule<string?>? RegNum { get; set; }

    [Serialization.JsonPropertyName("documentType")]
    public FilterRule<string?>? DocumentType { get; set; }

    [Serialization.JsonPropertyName("clientName")]
    public FilterRule<string?>? ClientName { get; set; }

    [Serialization.JsonPropertyName("language")]
    public FilterRule<string?>? Language { get; set; }

    [Serialization.JsonPropertyName("clientType")]
    public FilterRule<string>? ClientType { get; set; }

    [Serialization.JsonPropertyName("branch")]
    public FilterRule<string?>? Branch { get; set; }

    [Serialization.JsonPropertyName("branchCode")]
    public FilterRule<string?>? BranchCode { get; set; }

    [Serialization.JsonPropertyName("signerName")]
    public FilterRule<string?>? SignerName { get; set; }

    [Serialization.JsonPropertyName("recipientName")]
    public FilterRule<string?>? RecipientName { get; set; }

    [Serialization.JsonPropertyName("isAnswer")]
    public FilterRule<string?>? IsAnswer { get; set; }

    [Serialization.JsonPropertyName("executorName")]
    public FilterRule<string?>? ExecutorName { get; set; }

    [Serialization.JsonPropertyName("registerCode")]
    public FilterRule<string?>? RegisterCode { get; set; }

    [Serialization.JsonPropertyName("summary")]
    public FilterRule<string?>? Summary { get; set; }

    [Serialization.JsonPropertyName("confidenceType")]
    public FilterRule<string?>? ConfidenceType { get; set; }

    [Serialization.JsonPropertyName("processStatus")]
    public FilterRule<string?>? ProcessStatus { get; set; }

    [Serialization.JsonPropertyName("currentStep")]
    public FilterRule<string?>? CurrentStep { get; set; }

    [Serialization.JsonPropertyName("initiatorCode")]
    public FilterRule<string?>? InitiatorCode { get; set; }

    [Serialization.JsonPropertyName("initiatorName")]
    public FilterRule<string?>? InitiatorName { get; set; }

    [Serialization.JsonPropertyName("finishDate")]
    public FilterRule<DateTime?>? FinishDate { get; set; }

    [Serialization.JsonPropertyName("status")]
    public FilterRule<string?>? Status { get; set; }

    [Serialization.JsonPropertyName("iin")]
    public FilterRule<string?>? Iin { get; set; }

    [Serialization.JsonPropertyName("userCode")]
    public FilterRule<string?>? UserCode { get; set; }

    [Serialization.JsonPropertyName("approveType")]
    public FilterRule<string?>? ApproveType { get; set; }

    [Serialization.JsonPropertyName("feedBackTool")]
    public FilterRule<string?>? FeedBackTool { get; set; }

    [Serialization.JsonPropertyName("requiredTime")]
    public FilterRule<string?>? RequiredTime { get; set; }

    [Serialization.JsonPropertyName("searchText")]
    public FilterRule<string?>? SearchText { get; set; }

    [Serialization.JsonPropertyName("documentStatus")]
    public FilterRule<string?>? DocumentStatus { get; set; }
}

public class Names
{
    public string? Name { get; set; }
}

public static class ProcessReportExtensions
{
    public static IQueryable<ProcessReportDbo> FilterDocumentStatus(this IQueryable<ProcessReportDbo> dbSet,
        FilterRules filterRules)
    {
        if (!string.IsNullOrEmpty(filterRules.DocumentStatus?.Operand) &&
            filterRules.DocumentStatus?.Operator == Operator.Contains)
        {
            if (filterRules.DocumentStatus!.Operand!.Contains("approved"))
                dbSet = dbSet.Where(e => e.IsApproved);

            else if (filterRules.DocumentStatus!.Operand!.Contains("signed"))
                dbSet = dbSet.Where(e => e.IsSigned);
        }

        return dbSet;
    }

    public static IQueryable<ProcessReportDbo> Filter(this IQueryable<ProcessReportDbo> dbSet,
        FilterRules filterRules)
    {
        if (!string.IsNullOrEmpty(filterRules.SearchText?.Operand) &&
            filterRules.SearchText?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                (t.InitiatorName != null &&
                 t.InitiatorName.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.ExecutorName != null && t.ExecutorName.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.SignerName != null && t.SignerName.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                || (t.Summary != null && t.Summary.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.CurrentStep != null && t.CurrentStep.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                || (t.ClientName != null && t.ClientName.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                || (t.RecipientName != null &&
                    t.RecipientName.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                || (t.RegNum != null && t.RegNum.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.ConfidenceType != null && t.ConfidenceType.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.Status != null && t.Status.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.ProcessStatus != null && t.ProcessStatus.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                //|| (t.Branch != null && t.Branch.ToLower().Contains(filterRules.SearchText.Operand!.ToLower()!))
                || (t.FeedBackTool != null &&
                    t.FeedBackTool.ToLower().StartsWith(filterRules.SearchText.Operand!.ToLower()!))
            );
        if (!string.IsNullOrEmpty(filterRules.DocumentStatus?.Operand) &&
            filterRules.DocumentStatus?.Operator == Operator.Contains)
        {
            if (filterRules.DocumentStatus!.Operand!.Contains("bookmark"))
            {
                dbSet = dbSet.Where(t => t.IsBookmark == true);
            }
        }

        if (!string.IsNullOrEmpty(filterRules.ClientType?.Operand) &&
            filterRules.ClientType?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.ClientType != null && t.ClientType.ToLower().Contains(filterRules.ClientType.Operand!.ToLower()!));

        if (!string.IsNullOrEmpty(filterRules.ConfidenceType?.Operand) &&
            filterRules.ConfidenceType?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.ConfidenceType != null && t.ConfidenceType!.Contains(filterRules.ConfidenceType.Operand!));

        if (!string.IsNullOrEmpty(filterRules.RequiredTime?.Operand) && filterRules.RequiredTime?.Operand != "All")
        {
            if (filterRules.RequiredTime!.Operand!.Contains("overdue"))
            {
                dbSet = dbSet.Where(t =>
                    t.DueToDate!.Value.Date < DateTime.Today && t.ProcessStatus!.ToLower() != "completed");
            }

            if (filterRules.RequiredTime!.Operand!.Contains("closeToOverdue"))
            {
                dbSet = dbSet.Where(t =>
                    t.DueToDate!.Value.Date >= DateTime.Today && t.DueToDate.Value.Date <= DateTime.Today.AddDays(2) &&
                    t.ProcessStatus!.ToLower() != "completed");
            }
        }
        else
        {
            if (filterRules.DueToDate!.Operator == Operator.Contains)
            {
                if (!string.IsNullOrEmpty(filterRules.DueToDate?.Operand?.ToString()))
                    dbSet = dbSet.Where(t =>
                        t.DueToDate.HasValue && t.DueToDate!.Value!.AddHours(6).Date ==
                        filterRules.DueToDate!.Operand!.Value.Date!);
                else if (!string.IsNullOrEmpty(filterRules.DueToDate?.Left?.ToString()))
                    dbSet = dbSet.Where(t =>
                        t.DueToDate.HasValue && t.DueToDate!.Value!.AddHours(6).Date ==
                        filterRules.DueToDate!.Left!.Value.Date!);
                else if (!string.IsNullOrEmpty(filterRules.DueToDate?.Right?.ToString()))
                    dbSet = dbSet.Where(t =>
                        t.DueToDate.HasValue && t.DueToDate!.Value!.AddHours(6).Date ==
                        filterRules!.DueToDate.Right!.Value.Date!);
            }

            if (filterRules.DueToDate!.Operator == Operator.Between
                && !string.IsNullOrEmpty(filterRules.DueToDate?.Left?.ToString())
                && !string.IsNullOrEmpty(filterRules.DueToDate?.Right?.ToString()))
            {
                var left = filterRules.DueToDate!.Left!.Value.Date;
                var right = filterRules.DueToDate!.Right!.Value.Date;
                if (left > right)
                {
                    DateTime t = left;
                    left = right;
                    right = t;
                }

                right = right.AddDays(1);
                dbSet = dbSet.Where(t =>
                    t.DueToDate.HasValue && left <= t.DueToDate!.Value.Date && t.DueToDate!.Value.Date <= right);
            }
        }

        if (filterRules.StartDate?.Operator == Operator.Between
            && !string.IsNullOrEmpty(filterRules.StartDate?.Left?.ToString())
            && !string.IsNullOrEmpty(filterRules.StartDate?.Right?.ToString()))
        {
            var left = filterRules!.StartDate?.Left!.Value.Date;
            var right = filterRules!.StartDate?.Right!.Value.Date;
            if (left > right)
            {
                DateTime? t = left;
                left = right;
                right = t;
            }

            right = right?.AddDays(1);
            dbSet = dbSet.Where(t => left <= t.StartDate.Date && t.StartDate.Date <= right);
        }

        if (!string.IsNullOrEmpty(filterRules.InitiatorName?.Operand) &&
            filterRules.InitiatorName?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.InitiatorName != null && t.InitiatorName!.Contains(filterRules.InitiatorName.Operand!));

        //if (!string.IsNullOrEmpty(filterRules.InitiatorCode?.Operand) && filterRules.InitiatorCode?.Operator == Operator.Contains)
        //    dbSet = dbSet.Where(t => t.InitiatorCode != null && t.InitiatorCode!.Contains(filterRules.InitiatorCode.Operand!));

        if (!string.IsNullOrEmpty(filterRules.CurrentStep?.Operand) &&
            filterRules.CurrentStep?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.CurrentStep != null && t.CurrentStep!.Contains(filterRules.CurrentStep.Operand!));


        if (!string.IsNullOrEmpty(filterRules.ProcessStatus?.Operand) &&
            filterRules.ProcessStatus?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.ProcessStatus!.ToLower().Contains(filterRules.ProcessStatus.Operand!));


        if (!string.IsNullOrEmpty(filterRules.RegNum?.Operand) && filterRules.RegNum?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.RegNum != null && t.RegNum!.Contains(filterRules.RegNum.Operand!));

        if (!string.IsNullOrEmpty(filterRules.FeedBackTool?.Operand) &&
            filterRules.FeedBackTool?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.FeedBackTool != null && t.FeedBackTool!.Contains(filterRules.FeedBackTool.Operand!));

        if (!string.IsNullOrEmpty(filterRules.ClientName?.Operand) &&
            filterRules.ClientName?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.ClientName != null && t.ClientName!.ToLower().Contains(filterRules.ClientName.Operand!.ToLower()!));


        if (!string.IsNullOrEmpty(filterRules.Language?.Operand) && filterRules.Language?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.Language != null && t.Language.Contains(filterRules.Language.Operand!));
        if (!string.IsNullOrEmpty(filterRules.Language?.Operand) && filterRules.Language?.Operator == Operator.Equals)
            dbSet = dbSet.Where(t => t.Language != null && t.Language == filterRules.Language.Operand!);


        if (!string.IsNullOrEmpty(filterRules.Branch?.Operand) && filterRules.Branch?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.Branch != null && t.Branch.Contains(filterRules.Branch.Operand!));

        if (!string.IsNullOrEmpty(filterRules.BranchCode?.Operand) &&
            filterRules.BranchCode?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.BranchCode != null && t.BranchCode.Contains(filterRules.BranchCode.Operand!));


        if (!string.IsNullOrEmpty(filterRules.SignerName?.Operand) &&
            filterRules.SignerName?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.SignerName != null && t.SignerName.Contains(filterRules.SignerName.Operand!));


        if (!string.IsNullOrEmpty(filterRules.RecipientName?.Operand) &&
            filterRules.RecipientName?.Operator == Operator.Contains)
        {
            dbSet = dbSet.Where(t =>
                t.RecipientName != null && t.RecipientName.Contains(filterRules.RecipientName.Operand!));
        }


        if (!string.IsNullOrEmpty(filterRules.ExecutorName?.Operand) &&
            filterRules.ExecutorName?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.ExecutorName != null && t.ExecutorName.Contains(filterRules.ExecutorName.Operand!));


        if (!string.IsNullOrEmpty(filterRules.RegisterCode?.Operand) &&
            filterRules.RegisterCode?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t =>
                t.RegisterCode != null && t.RegisterCode.Contains(filterRules.RegisterCode.Operand!));


        if (!string.IsNullOrEmpty(filterRules.Status?.Operand) && filterRules.Status?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.Status != null && t.Status!.Contains(filterRules.Status.Operand!));

        if (filterRules.StartDate?.Operator == Operator.Contains)
        {
            if (!string.IsNullOrEmpty(filterRules.StartDate?.Operand?.ToString("hh:mm tt", new CultureInfo("kk-KZ"))))
                dbSet = dbSet.Where(t => t.StartDate.Date == filterRules.StartDate.Operand!.Value.Date);
            else if (!string.IsNullOrEmpty(filterRules.StartDate?.Left?.ToString("hh:mm tt", new CultureInfo("kk-KZ"))))
                dbSet = dbSet.Where(t => t.StartDate.Date == filterRules.StartDate.Left!.Value.Date);
            else if (!string.IsNullOrEmpty(
                         filterRules.StartDate?.Right?.ToString("hh:mm tt", new CultureInfo("kk-KZ"))))
                dbSet = dbSet.Where(t => t.StartDate.Date == filterRules.StartDate.Right!.Value.Date);
        }

        if (!string.IsNullOrEmpty(filterRules.Summary?.Operand) && filterRules.Summary?.Operator == Operator.Contains)
            dbSet = dbSet.Where(t => t.Summary!.Contains(filterRules.Summary.Operand!));

        if (filterRules.FinishDate?.Operator == Operator.Contains)
        {
            if (!string.IsNullOrEmpty(filterRules.FinishDate?.Operand?.ToString()))
                dbSet = dbSet.Where(t =>
                    t.FinishDate != null && t.FinishDate.Value.Date == filterRules.FinishDate!.Operand!.Value.Date);
            else if (!string.IsNullOrEmpty(filterRules.FinishDate?.Left?.ToString()))
                dbSet = dbSet.Where(t =>
                    t.FinishDate != null && t.FinishDate.Value.Date == filterRules.FinishDate!.Left!.Value.Date);
            else if (!string.IsNullOrEmpty(filterRules.FinishDate?.Right?.ToString()))
                dbSet = dbSet.Where(t =>
                    t.FinishDate != null && t.FinishDate.Value.Date == filterRules!.FinishDate!.Right!.Value.Date);
        }

        if (filterRules.FinishDate?.Operator == Operator.Between
            && !string.IsNullOrEmpty(filterRules.FinishDate?.Left?.ToString())
            && !string.IsNullOrEmpty(filterRules.FinishDate?.Right?.ToString()))
        {
            var left = filterRules.FinishDate?.Left!.Value.Date;
            var right = filterRules.FinishDate?.Right!.Value.Date;
            if (left > right)
            {
                (left, right) = (right, left);
            }

            right = right?.AddDays(1);
            dbSet = dbSet.Where(t => left <= t.FinishDate!.Value.Date && t.FinishDate!.Value.Date <= right);
        }

        return dbSet;
    }
}