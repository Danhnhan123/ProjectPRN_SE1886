using System;
using System.Collections.Generic;

namespace ProjectPRN_SE1886.Models;

public partial class Household
{
    public int HouseholdId { get; set; }

    public int? HeadOfHouseholdId { get; set; }

    public string Address { get; set; } = null!;

    public DateOnly? CreatedDate { get; set; }

    public string HouseholdNumber { get; set; } = null!;

    public virtual User? HeadOfHousehold { get; set; }

    public virtual ICollection<HouseholdMember> HouseholdMembers { get; set; } = new List<HouseholdMember>();

    public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    public string AddressDisplay => $"{Address} - Chủ hộ: {HeadOfHousehold?.FullName} ({HeadOfHousehold?.Cccd}) - Số hộ: {HouseholdNumber}";

}
