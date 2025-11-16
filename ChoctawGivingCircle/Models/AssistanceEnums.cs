using System;

namespace ChoctawGivingCircle.Models;

public enum AssistanceStatus
{
    Draft,
    Submitted,
    UnderReview,
    Approved,
    Open,
    FullyFunded,
    Fulfilled,
    Closed
}

public enum AssistanceCategory
{
    Education,
    Housing,
    Utilities,
    Regalia,
    Transportation,
    Health,
    Other
}

public enum AssistancePriority
{
    Low,
    Medium,
    High,
    Critical
}
