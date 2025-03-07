using System;
using System.Collections.Generic;

namespace ProjectPRN_SE1886.Models;

public partial class Household
{
    public int HouseholdId { get; set; }

    public int? HeadOfHouseholdId { get; set; }

    public string Address { get; set; } = null!;

    public DateOnly? CreatedDate { get; set; }

    public virtual User? HeadOfHousehold { get; set; }

    public virtual ICollection<HouseholdMember> HouseholdMembers { get; set; } = new List<HouseholdMember>();
}
