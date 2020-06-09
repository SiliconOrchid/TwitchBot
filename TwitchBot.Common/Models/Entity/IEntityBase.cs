using System;

namespace TwitchBot.Common.Models.Entity
{
    public interface IEntityBase
    {
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
    }
}