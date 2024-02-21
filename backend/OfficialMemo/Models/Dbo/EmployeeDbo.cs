namespace OfficialMemo.Models.Dbo
{
    public class EmployeeDbo
    {
        public int Id { get; init; } = -1;
        public string Code { get; init; } = null!;
        public string Login { get; init; } = null!;
        public string Iin { get; init; } = null!;
        public string Name { get; init; } = null!;
        public string? Email { get; init; }
        public string? Phones { get; init; }
        public string? LocalPhone { get; set; }
        public string WorkStatus { get; init; } = null!;

        public string? DepartmentCode { get; set; }
        public string? ParentDepartmentCode { get; set; }
        public string? BranchCode { get; set; }
        public bool Deleted { get; set; }

        public bool IsStaff { get; set; }

        public string? PositionKz { get; init; }
        public string? PositionRu { get; init; }
        public string? PositionEn { get; init; }
        public string? PositionCode { get; init; }
    }
}
