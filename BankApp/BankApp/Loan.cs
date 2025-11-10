public class Loan
{
    public enum LoanStatus
    {
        Active,
        PaidOff,
        Defaulted,
        Closed,
    }

    public decimal PrincipalAmount { get; private set; }
    public decimal OutstandingAmount { get; private set; }
    public float InterestRatePercent { get; private set; }
    public int TermYears { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public LoanStatus Status { get; private set; }

    // Constructor that automatically assigns rate and due date based on loan term
    public Loan(decimal principalAmount, int termYears, DateTime startDate)
    {
        if (principalAmount <= 0) throw new ArgumentException("Principal amount must be positive.", nameof(principalAmount));
        if (!LoanPricing.IsSupportedTerm(termYears)) throw new ArgumentException("Loan term must be 1, 2, or 3 years.", nameof(termYears));

        PrincipalAmount = principalAmount;
        OutstandingAmount = principalAmount;
        TermYears = termYears;
        InterestRatePercent = LoanPricing.GetRateForYears(termYears);
        StartDate = startDate;
        DueDate = startDate.AddYears(termYears);
        Status = LoanStatus.Active;
    }
    public Loan(decimal principalamount, float interestratepercent, DateTime startdate, DateTime duedate)
    {
        if (principalamount <= 0) throw new ArgumentException("Principal must be positive.", nameof(principalamount));
        if (interestratepercent < 0) throw new ArgumentException("Interest rate cannot be negative.", nameof(interestratepercent));
        if (duedate <= startdate) throw new ArgumentException("Due date must be after start date.", nameof(duedate));

        PrincipalAmount = principalamount;
        OutstandingAmount = principalamount;
        InterestRatePercent = interestratepercent;
        StartDate = startdate;
        DueDate = duedate;
        Status = LoanStatus.Active;

        // Estimate term length
        TermYears = Math.Max(1, (int)Math.Round((duedate - startdate).TotalDays / 365.0));
    }

    public void ApplyInterest()
    {
        if (Status != LoanStatus.Active) return;
        decimal interest = OutstandingAmount * ((decimal)InterestRatePercent / 100m);
        OutstandingAmount += interest;
    }

    public void MakePayment(decimal amount)
    {
        if (Status != LoanStatus.Active || amount <= 0) return;

        OutstandingAmount -= amount;
        if (OutstandingAmount <= 0)
        {
            OutstandingAmount = 0;
            Status = LoanStatus.PaidOff;
        }
    }

    public void CheckDue(DateTime currentDate)
    {
        // Loan defaults only if past the due date and still unpaid
        if (Status == LoanStatus.Active && currentDate > DueDate && OutstandingAmount > 0)
            Status = LoanStatus.Defaulted;
    }

    public void CloseLoan() => Status = LoanStatus.Closed;
}

// Rate lookup table
internal static class LoanPricing
{
    private static readonly Dictionary<int, float> _ratesByYears = new()
    {
        { 1, 5.00f },
        { 2, 6.25f },
        { 3, 7.50f }
    };

    public static float GetRateForYears(int years)
    {
        if (!_ratesByYears.TryGetValue(years, out var rate))
            throw new ArgumentException("Invalid loan term. Allowed terms are 1, 2, or 3 years.", nameof(years));
        return rate;
    }

    public static bool IsSupportedTerm(int years) => _ratesByYears.ContainsKey(years);
}
